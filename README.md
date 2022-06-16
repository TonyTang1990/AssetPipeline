# AssetPipeline
## 工具
1. Unity版本(2012 3.34f1c2)
2. Visual Studio 2019

## 最终目标
日常项目中，我们对于Unity资源导入会做一些**自动化的设置和检查，确保导入到项目里的资源是正确符合要求的，确保游戏资源最优化**。而这些自动化检查和处理离不开**AssetPostProcess**接口，常规的方式是通过重写AssetPostProcess相关接口进行硬编码的方式实现资源导入自动化检查和设置。但这样的实现方式显得不灵活，当我们需要改变或新增一些资源时，我们需要硬编码去实现需求。

所以本Git的目标就是实现一个**高效高度扩展性可配置化的Asset管线系统。**

## AssetPipeline

### Asset管线设计

1. 使用ScriptableObject实现自定义配置数据存储
2. 使用EditorWindow实现自定义配置窗口
3. 使用AssetPostProcess实现Asset后处理系统流程接入
4. 设计AssetPipeline和AssetPipelineSystem实现Asset管线系统流程初始化
5. 设计PresetSystem,AssetProcessorSystem和AssetCheckSystem实现Asset管线的3大系统(Preset选择系统，Asset处理系统和Asset检查系统)
6. 支持配置多个Asset管线策略以及不同平台选用不同策略来实现平台各异化的管线系统配置

### 类说明

- AssetPipelineWindow -- Asset管线可配置化配置窗口

- AssetPipeline -- Asset管线入口(负责接入Asset后处理流程以及Asset管线系统初始化)
- AssetPipelineSystem -- Asset管线系统(负责初始化Asset管线里各系统以及各系统调用)
- PresetSystem -- Preset系统(负责Preset自动化相关配置，选择和处理)
- AssetProcessorSystem -- Asset处理系统(负责Asset自动化后处理相关配置，选择和处理)
- AssetCheckSystem -- Asset检查系统(负责Asset自动化检查相关配置，选择和处理)
- PresetSettingData -- Preset设置数据(AssetPipelineWindow 窗口配置)
- PresetConfigData -- Preset配置数据(AssetPipelineWindow 窗口配置)
- ProcessorSettingData -- Asset处理器设置数据(AssetPipelineWindow 窗口配置)
- ProcessorConfigData -- Asset处理器配置数据(AssetPipelineWindow 窗口配置)
- AssetCheckSettingData -- Asset检查设置数据(AssetPipelineWindow 窗口配置)
- AssetCheckConfigData -- Asset检查配置数据(AssetPipelineWindow 窗口配置)
- BaseProcessor -- Asset处理器抽象类(负责抽象Asset处理器流程和数据，继承至ScriptableObject实现自定义数据配置处理)
- BaseCheck -- Asset检查抽象类(负责抽象Asset检查器流程和数据)
- AssetPipelineUtilities -- Asset管线工具类(负责提供Asset管线相关工具方法)

### 实战实现





## 重点知识

1. 

## 博客