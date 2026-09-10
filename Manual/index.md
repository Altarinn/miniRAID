# miniRAID 手册

[返回 README](../README.md)

## 入门

1. [快速上手](Getting-Started.md)：环境、依赖、场景和首次运行检查。
2. [项目结构](Project-Structure.md)：系统分工、启动顺序、常见修改入口与当前限制。

这两页是基于当前源码整理的基础入口。核对版本与验证范围见 [README](../README.md)。

## 已有专题

以下文档保留较详细的说明，尚未在本次基础文档整理中逐条复核。阅读代码示例时，以当前源码签名为准；其中的测试或性能结论不代表本次重新验证。

| 主题 | 文档 |
| --- | --- |
| 配置、运行时状态与显示 | [三层架构](Claude/Three-Level-Architecture.md) |
| 战斗顺序、TurnSlice 与敌人行动 | [战斗调度](Claude/TurnSlice-and-Combat-Scheduling-System.md)、[制作 Boss](Instructions%20on%20making%20Bosses.md) |
| 状态与序列化 | [BackendState and Serialization](BackendState%20and%20Serialization.md) |
| 技能帮助类 | [SpellHelpers](SpellHelpers.md) |
| 地图与坐标 | [Map System](Claude/Map-System-Architecture.md)、[Full 3D Maps](Full%203D%20Maps.md) |
| 自动攻击目标选择 | [目标选取重构说明](自动攻击目标选取系统重构.md) |
| 移动性能分析 | [Movement Performance Analysis](Claude/Movement-Performance-Analysis.md) |
| 摄像机吸附 | [Camera snap algorithm](../camera_snap_algorithm.md) |

## 计划与历史记录

这些页面描述设计讨论、迁移方案或早期实现，不能据此认定功能已经完成。

- [角色成长与 Demo 计划](Claude/Plans/Character-Progression-And-Demo-Strategy.md)
- [地图 Addressables 迁移计划](Claude/Plans/MapSystem-Addressables-Migration.md)
- [TODO 优先级分析](Claude/TODO-Priority-Analysis.md)
- [浮点位置重构记录](../FLOATING_POINT_POSITION_REFACTOR.md)
- [BlockIntrude 实现计划](../BlockIntrude_Implementation_Plan.md)
- [早期入门教程与截图](Legacy-Introduction.md)

## 更新文档

修改环境或启动场景时更新“快速上手”；调整系统职责或新增主要目录时更新“项目结构”。深入的接口说明放在对应专题内，并注明源码核对与实际运行验证的区别。
