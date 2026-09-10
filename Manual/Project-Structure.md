# 项目结构

[手册目录](index.md) · [快速上手](Getting-Started.md)

## 目录导览

| 目录 | 内容 |
| --- | --- |
| [Scripts/Backend](../Assets/Scripts/Backend/) | 单位状态、属性、战斗调度、地图、碰撞、移动和序列化 |
| [Scripts/Spells](../Assets/Scripts/Spells/) | 技能目标、目标验证、伤害与效果帮助类 |
| [Scripts/Buffs](../Assets/Scripts/Buffs/)、[Weapons](../Assets/Scripts/Weapons/) | Buff、被动技能和装备的通用实现 |
| [Scripts/MobBehaviour](../Assets/Scripts/MobBehaviour/) | 敌人行为、蓄力行动与回合片段 |
| [Scripts/Presentation](../Assets/Scripts/Presentation/)、[UI](../Assets/Scripts/UI/) | 角色显示、特效、音频、交互状态与战斗界面 |
| [Scripts/PixelArtRenderer](../Assets/Scripts/PixelArtRenderer/) | Billboard 角色、摄像机辅助、自定义 URP 放大 Pass 与 Shader |
| [Scripts/Editor](../Assets/Scripts/Editor/) | 地图编辑器、技能与 Buff 创建工具、监视窗口、表格工具 |
| [GameContent](../Assets/GameContent/) | 角色、敌人、技能、装备和调度配置，也包含具体内容的 C# 代码 |
| [Scenes](../Assets/Scenes/) | 标题、战斗和实验场景；标题控制脚本也在这里 |
| [Localization](../Assets/Localization/)、[Resources](../Assets/Resources/)、[UI Toolkit](../Assets/UI%20Toolkit/) | 本地化、运行时资源与 UI 资源 |
| [AddressableAssetsData](../Assets/AddressableAssetsData/)、[URPAssets](../Assets/URPAssets/) | 资源寻址与渲染配置 |
| [Plugins](../Assets/Plugins/)、[DOTween](../Assets/DOTween/)、[NuGet](../Assets/NuGet/)、[SODatabase](../Assets/SODatabase/) | 插件和依赖快照，需与游戏自身逻辑区分 |
| [Numericals](../Numericals/)、[data.xlsx](../data.xlsx)、[Actions.csv](../Actions.csv) | 数值设置和表格资料；表格工具与数值计算仍有未完成入口 |
| [RenderingTests](../RenderingTests/)、[WebGLPlugins](../WebGLPlugins/) | 渲染实验与 Lua / XLua 的 C 源码资料 |

上表中的游戏代码、内容与插件目录位于 `Assets/` 下；`Numericals`、表格、`RenderingTests` 和 `WebGLPlugins` 位于仓库根目录。

主要程序集为 [miniRAID.Core](../Assets/Scripts/miniRAID.Core.asmdef)、[miniRAID.GameContent](../Assets/GameContent/GameContent.asmdef) 和仅限编辑器的 [miniRAID.Editor](../Assets/Scripts/Editor/miniRAID.Editor.asmdef)。`GameContent` 引用 `Core`；`Assets/Scripts` 外还有场景脚本与实验脚本。

## 数据如何变成战斗画面

项目常见的组织方式是“配置 → 运行时状态 → 显示”：

| 层 | 例子 | 职责 |
| --- | --- | --- |
| ScriptableObject 配置 | `ActionDataSO<T>`、`BuffSO`、`MobDescriptorSO` | 保存可复用的数值、资源引用和行为配置 |
| 运行时状态 | `RuntimeAction<T>`、`Buff`、`MobData` | 保存每个单位或效果的独立状态，执行规则和事件 |
| 显示 | `MobRenderer`、`IStateRenderer`、UI 控制器 | 同步位置、状态、提示和动画 |

共享的 SO 不适合存储某一个单位的临时状态。运行时对象通过 `Wrap` 等工厂方法建立，实际方法与类型见 [ActionDataSO.cs](../Assets/Scripts/Backend/SOCore/Action/ActionDataSO.cs)、[BuffSO.cs](../Assets/Scripts/Buffs/BuffSO.cs) 和 [BackendState.cs](../Assets/Scripts/Backend/BackendCore/BackendState.cs)。

[Globals](../Assets/Scripts/Utils/Globals.cs) 提供后端、UI、调度器和当前协程上下文的常用入口。后端与显示有职责分工，但运行时仍直接使用 Unity 类型与全局服务，不能假定它可以脱离 Unity 独立运行。

## 战斗从哪里开始

1. [CombatSchedulerCoroutine.Start](../Assets/Scripts/Backend/CombatSchedulerCoroutine/CombatSchedulerCoroutine.cs) 读取 `SceneConfig`，创建包含动画设置和 RNG 的协程上下文。
2. `Combat()` 等待本地化和 [Databackend.Initialize](../Assets/Scripts/Backend/Databackend.cs)；后者按地图名和起点加载地图块。
3. 调度器准备战斗，初始化行动队列，然后依次执行 `TurnSlice.Turn()`。被标记为 `muted` 的片段会跳过执行。
4. [SerialCoroutine](../Assets/Scripts/Backend/CombatSchedulerCoroutine/Essentials/SerialCoroutine.cs) 与 `JumpIn` 串联规则、事件和动画等待；行动完成后移除当前片段并补充后续调度。

场景放置的单位可由 [MobRenderer.Init](../Assets/Scripts/Presentation/MobRenderer.cs) 在 `handleDataInit` 开启时初始化。单位状态见 [MobData](../Assets/Scripts/Backend/MobData/MobData.cs)，动作执行入口见 [MobData.actions](../Assets/Scripts/Backend/MobData/MobData.actions.cs)。

## 修改功能时从哪里看

| 想做的事 | 起点与现有例子 |
| --- | --- |
| 添加技能 | 从 [BasicProjectile](../Assets/GameContent/Allies/Actions/Essentials/Scripts/BasicProjectile.cs) 看 `ActionDataSO<SingleMobTarget>` 和带类型的 `OnPerform`；用 `Assets > Create Action` 创建对应配置 |
| 添加 Buff | 从 [WindBuffSO](../Assets/GameContent/Allies/Actions/Essentials/Scripts/WindBuffSO.cs) 看 `Wrap`、`OnAttach` 与移除时解除事件监听；用 `Assets > Create Buff` 创建配置 |
| 配置敌人行动 | 查看 [MobBehaviour](../Assets/Scripts/MobBehaviour/) 和 [AlphaWolf 内容](../Assets/GameContent/Enemies/OpenTest/AlphaWolf/)，再读战斗调度专题 |
| 让蓄力行动被打断 | 查看 [TurnSliceBuffSO](../Assets/Scripts/MobBehaviour/TurnSlices/TurnSliceBuffSO.cs) 与 [RoarOverloadBuffSO](../Assets/GameContent/Enemies/OpenTest/AlphaWolf/AoERoar/RoarOverloadBuffSO.cs) |
| 修改移动或碰撞 | 查看 [Databackend](../Assets/Scripts/Backend/Databackend.cs)、[Movement](../Assets/Scripts/Backend/BackendCore/Movement/)、[Colliders](../Assets/Scripts/Backend/BackendCore/Colliders/) 与 [MapSystem](../Assets/Scripts/Backend/Map/MapSystem.cs) |
| 编辑地图规则 | `miniRAID > Map Editor`，实现见 [MapEditor](../Assets/Scripts/Editor/MapEditor.cs)；块数据位于 `GameContent/MapChunks` |
| 调整画面 | 查看 [PixelArtRendererFeature](../Assets/Scripts/PixelArtRenderer/PixelArtRendererFeature.cs)、[PixelArtUpscalingPass](../Assets/Scripts/PixelArtRenderer/PixelArtUpscalingPass.cs)、[CameraSnapHelper](../Assets/Scripts/PixelArtRenderer/Scripts/CameraSnapHelper.cs) 与场景中的材质、摄像机配置 |
| 修改本地化文本 | 查看 [LocalizationManager](../Assets/Scripts/Utils/LocalizationManager.cs) 与 `Assets/Localization`；技能名称和说明使用 `LocalizedString` |
| 查看调试数据 | `miniRAID > Mob Monitor`、`miniRAID > Combat Monitor`，实现位于 [Editor/Utils](../Assets/Scripts/Editor/Utils/) |

创建 Action / Buff 的菜单实现见 [ActionEditor](../Assets/Scripts/Editor/ActionEditor.cs)。创建配置后仍需在 Inspector 中设置参数，并将资源接入对应角色、装备或敌人行动配置。

## 地图、坐标与存档

[MapChunk](../Assets/Scripts/Backend/Map/MapChunk.cs) 保存 `32 × 32 × 32` 个格子的实体、可站立、可通行等规则；[MapSystem](../Assets/Scripts/Backend/Map/MapSystem.cs) 负责 Addressables 加载与地图查询。[MapRenderer](../Assets/Scripts/Backend/Map/MapRenderer.cs) 可显示这些块的状态，场景美术资源仍需结合场景本身查看。

单位位置使用 `Vector3`，格子索引使用 `Vector3Int`。转换与居中偏移集中在 `Databackend` 的坐标帮助方法中；修改显示位置时同时检查 `MobRenderer.SyncRendererPosition`，避免重复添加半格偏移。

[SaveDataSerializer](../Assets/Scripts/Backend/Serialization/SaveDataSerializer.cs) 使用 Odin 将后端、调度信息、战斗统计和 RNG 打包到快照。恢复时会重新初始化地图，并恢复状态与显示关联；具体过程见 [Databackend.serialization](../Assets/Scripts/Backend/Serialization/Databackend.serialization.cs)。

## 当前限制

- **战斗结束尚未接通：** `CombatSchedulerCoroutine.IsCombatFinished()` 固定返回 `false`，不能把胜负结束流程视为已完成。
- **地图加载范围有限：** `UpdateChunkLoadingAsync` 实际遍历的是值为零的 `LoadingOffset`，每次选择一个当前块。文件中的较大加载范围注释不是当前行为。
- **地图写回依赖编辑器：** `SaveChunkToAddressable` 在编辑器中保存资源，构建版本仅记录不支持写回的警告。快照恢复中的地图修改恢复也有 TODO。
- **快照保存在内存：** `saveSlot` / `saveSlotBackup` 是静态字段；此入口没有磁盘存档流程，不能保证退出游戏后恢复。
- **部分工具仍是占位：** 例如 `FundamentalNumericalsSO.ComputeNumericals()` 与 `SyncExcelWindow.ApplyFilters()` 为空，表格和数值工具需按具体入口核实。

以上结论来自源码核对。性能、视觉效果、完整战斗可玩性和各平台兼容性仍需 Unity 中的运行验证；已有专题里的历史分析不能替代这些验证。
