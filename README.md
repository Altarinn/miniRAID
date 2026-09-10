# miniRAID

miniRAID 是一个使用 Unity 开发的战术 RPG 原型：在三维格子地图上进行回合战斗，以像素角色、3D 场景和自定义 URP 渲染呈现画面。角色、技能、Buff、装备与敌人行为通过 ScriptableObject 配置和 C# 运行时逻辑组合。

## 从这里开始

- [快速上手](Manual/Getting-Started.md)：打开项目、恢复依赖、选择场景与检查运行情况。
- [项目结构](Manual/Project-Structure.md)：主要目录、战斗流程，以及修改功能时应查看的代码。
- [手册目录](Manual/index.md)：已有专题、设计计划与历史教程。

## 开发环境

项目记录的编辑器版本为 **Unity 6000.2.0f1**，见 [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt)。从 Unity Hub 添加本目录并使用对应版本打开。

主要依赖包括 URP、Cinemachine、Input System、Localization、Addressables，以及仓库中的 Odin、DOTween、XLua 和 NuGet for Unity。包版本与首次导入说明见[快速上手](Manual/Getting-Started.md)。

构建场景以 [Title](Assets/Scenes/Title.unity) 为入口，当前按键后进入 `CombatBase`。也可直接打开 [AlphaWolf](Assets/Scenes/OpenTest/AlphaWolf.unity) 检查具体战斗内容。

## 当前范围

项目包含战斗调度、技能与 Buff、敌人行动、地图碰撞与移动、UI、本地化和像素渲染。战斗结束判定、持久存档等仍有未完成部分，见[当前限制](Manual/Project-Structure.md#当前限制)。

本次基础文档基于 `pixelart` 分支 `aed61c498f293fe08504eec17f6a5af6276aa6c0` 的源码、包配置与场景配置整理（2026-09-06）。核对范围为项目结构和主要系统入口；未进行 Unity 编译、Play Mode 或构建验证。已有日志改动和未跟踪的 `Assets/_Recovery/` 场景未作为正式使用流程的依据。
