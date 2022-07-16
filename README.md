# AssetPipeline
## 工具
1. Unity版本(2020 3.34f1c2)
2. Visual Studio 2019

## 最终目标
日常项目中，我们对于Unity资源导入会做一些**自动化的设置和检查，确保导入到项目里的资源是正确符合要求的，确保游戏资源最优化**。而这些自动化检查和处理离不开**AssetPostProcess**接口，常规的方式是通过重写AssetPostProcess相关接口进行硬编码的方式实现资源导入自动化检查和设置。但这样的实现方式显得不灵活，当我们需要改变或新增一些资源时，我们需要硬编码去实现需求。

所以本Git的目标就是实现一个**高效高度扩展性可配置化的Asset管线系统。**

## AssetPipeline

### Asset管线设计

1. 使用ScriptableObject实现自定义配置数据存储
2. 使用EditorWindow实现自定义配置窗口
3. 使用AssetPostProcess实现Asset后处理系统流程接入
4. 设计AssetPipeline和AssetPipelineSystem实现Asset管线系统流程初始化(利用InitializeOnLoadMethodAttribute标签实现Asset管线提前初始化)
5. 设计AssetProcessorSystem和AssetCheckSystem实现Asset管线的2大系统(Asset处理系统和Asset检查系统)
6. 支持配置多个Asset管线策略以及不同平台选用不同策略来实现平台各异化的管线系统配置
7. 支持Asset管线预处理，后处理，移动处理和删除处理器配置
8. 支持全局和局部两种大的类型处理器和检查器配置
9. 支持Asset管线预检查和后检查检查器配置
10. 支持局部Asset处理器和检查器黑名单目录配置
11. 处理器和检查器支持指定Asset类型级别设置
12. 处理器和检查器支持指定Asset管线处理类型(AssetPostProcess相关流程接口)级别设置
13. 支持所有处理器和检查器UI预览查看

Note:

1. **Asset管线先执行全局后局部,局部Asset处理由外到内执行(覆盖关系)**

2. **Asset管线移动Asset当做重新导入Asset处理，确保Asset移动后得到正确的Asset管线处理**

3.  **Asset管线不满足条件的不会触发(比如: 1. 不在目标资源目录下 2. 在黑名单列表里)**

4.  **配置符合的触发顺序如下: **

   ​	**触发Asset导入全局预检查 -> 触发Asset导入局部预检查 -> 触发Asset导入全局预处理 -> 触发Asset导入局部预处理 -> 触发Asset导入全局后处理 -> 触发Asset导入局部后处理 -> 触发Asset导入全局后检查 -> 触发Asset导入局部后检查**

5. **处理器和检查器支持指定需要处理的Asset类型组合，但Asset管线处理类型只能选择一个**
6. **通用后处理类型包含后处理，移动处理和删除处理**

### 类说明

Asset管线核心框架部分:

- AssetType -- Asset类型定义
- AssetProcessType -- Asset管线处理类型定义
- LocaDetailWindow -- Asset管线局部数据详情配置窗口(用于配置局部数据黑名单路径配置)
- AssetPipeline -- Asset管线入口(负责接入Asset后处理流程以及Asset管线系统初始化)
- AssetPipelineSystem -- Asset管线系统(负责初始化Asset管线里各系统以及各系统调用)
- AssetProcessorSystem -- Asset处理系统(负责Asset自动化后处理相关配置)
- AssetCheckSystem -- Asset检查系统(负责Asset自动化检查相关配置)
- ProcessorSettingData -- Asset处理器设置数据(AssetPipelineWindow 窗口配置)
- ProcessorConfigData -- Asset处理器配置数据(AssetPipelineWindow 窗口配置)
- AssetCheckSettingData -- Asset检查设置数据(AssetPipelineWindow 窗口配置)
- AssetCheckConfigData -- Asset检查配置数据(AssetPipelineWindow 窗口配置)
- BaseProcessor -- Asset处理器抽象类(负责抽象Asset处理器流程和数据，继承至ScriptableObject实现自定义数据配置处理)
- BasePreProcessor -- Asset预处理器抽象类(负责抽象Asset预处理器流程和数据，继承至BaseProcessor )
- BasePostProcessor -- Asset预处理器抽象类(负责抽象Asset后处理器流程和数据，继承至BaseProcessor )
- BaseCheck -- Asset检查抽象类(负责抽象Asset检查器流程和数据，继承至ScriptableObject实现自定义数据配置处理)
- BasePreCheck -- Asset预检查抽象类(负责抽象Asset预检查器流程和数据，继承至BaseCheck )
- BasePostCheck -- Asset后检查抽象类(负责抽象Asset后检查器流程和数据，继承至BaseCheck )

窗口部分:

- AssetPipelineWindow -- Asset管线可配置化配置窗口
- AssetPipelinePanel -- Asset管线面板
- AssetProcessorPanel -- Asset处理器面板
- AssetCheckPanel -- Asset检查器面板

其他部分:

- AssetPipelineUtilities -- Asset管线工具类(负责提供Asset管线相关工具方法)
- AssetPipelineStyles -- Asset管线用到的GUI显示风格定义
- AssetPipelineGUIContent -- Asset管线用到的显示图标定义
- AssetPipelineConst -- Asset管线用到的常量定义
- AssetPipelineLog -- Asset管线自定义Log
- AssetPipelinePrefKey -- Asset管线本地存储Key定义

### 实战实现

在了解详细的细节实现之前，先了解下Asset管线是如何定义Asset类型和Asset管线处理类型的，这个对于后续实战配置理解会有帮助。

- **Asset类型定义**

  ```CS
  /// <summary>
  /// AssetType.cs
  /// Asset类型
  /// </summary>
  [Flags]
  public enum AssetType : Int32
  {
      None = 0,                         // 无效类型
      Texture = 1 << 0,                 // 图片
      Material = 1 << 1,                // 材质
      SpriteAtlas = 1 << 2,             // 图集
      FBX = 1 << 3,                     // 模型文件
      AudioClip = 1 << 4,               // 音效
      Font = 1 << 5,                    // 字体
      Shader = 1 << 6,                  // Shader
      Prefab = 1 << 7,                  // 预制件
      ScriptableObject = 1 << 8,        // ScriptableObject
      TextAsset = 1 << 9,               // 文本Asset
      Scene = 1 << 10,                  // 场景
      AnimationClip = 1 << 11,          // 动画文件
      Mesh = 1 << 12,                   // Mesh文件
      Script = 1 << 13,                 // 脚本(e.g. cs, dll等)
      Video = 1 << 14,                  // 视频
      Folder = 1 << 15,                 // 文件夹
      Other = 1 << 16,                  // 其他
      All = Int32.MaxValue,             // 所有类型
  }
  ```

  ​	**上面的Asset类型只所以定义成Flags是为了处理器和检查器可以定义需要支持的Asset类型组合**

- Asset管线处理类型定义

  ```CS
  /// <summary>
  /// AssetProcessorType.cs
  /// Asset管线处理类型
  /// </summary>
  public enum AssetProcessType
  {
      CommonPreprocess = 1,               // 通用预处理
      PreprocessAnimation,                // 动画预处理
      PreprocessAudio,                    // 音效预处理
      PreprocessModel,                    // 模型预处理
      PreprocessTexture,                  // 纹理预处理
      CommonPostprocess,                  // 通用后处理
      PostprocessAnimation,               // 动画后处理
      PostprocessModel,                   // 模型后处理
      PostprocessMaterial,                // 材质后处理
      PostprocessPrefab,                  // 预制件后处理
      PostprocessTexture,                 // 纹理后处理
      PostprocessAudio,                   // 音效后处理
  }
  ```

  上面的类型分别对应**AssetPostprocessor**里的Asset处理接口，这里就不详细介绍了，具体看代码。

#### 配置窗口

![Asset管线策略配置面板](/Images/AssetPipelineConfigPanel.PNG)

此窗口主要负责配置Asset管线策略以及不同平台Asset管线策略的使用。同时也负责选择当前配置的策略和设置全局的资源处理目录。

![Asset管线全局预处理器配置面板](/Images/AssetGlobalPreProcessorConfigPanel.PNG)

![Asset管线全局后处理器配置面板](/Images/AssetGlobalPostProcessorConfigPanel.PNG)

**全局后处理器，移动处理器和删除处理器配置面板类似就不一一截图了，他们分别对应通用后处理里的imported，moved和deleted流程。**

![Asset管线局部预处理器配置面板](/Images/AssetLocalPreProcessorConfigPanel.PNG)

**从上面可以看到我们对于局部处理器的配置方式，是通过制定目录，选择处理器列表的方式实现的，同时每个处理器还支持制定黑名单目录(用于部分处理器子目录不生效的配置)**

**通过点击数量(*)我们可以打开指定局部处理器配置指定处理器的黑名单详情设置窗口**

![Asset管线局部数据黑名单目录配置窗口](/Images/AssetLocalBlackListConfigWindow.PNG)

通过上面的黑名单目录配置结合前面的局部预处理配置我们可以看出，我们希望ETC2设置对于Assets/Res/textures目录生效但不希望对Assets/Res/textures/TestBlackList目录生效，对于TestBlackList目录我们单独设置了ASTC处理器设置。

![Asset管线局部后处理器配置面板](/Images/AssetLocalPostProcessorConfigPanel.PNG)

关于移动处理器和删除处理器和预处理器和后处理器配置类似的，只是对应不同处理流程，这里就不一一截图举例了。

![Asset管线处理器预览面板](/Images/AssetProcessorPreviewPanel.PNG)

**检查器窗口配置和处理器窗口配置是类似的，只是检查器只支持了预检查器和后检查器，这里我就只简单截一张效果图看看，就不详细说明了**

![Asset管线全局预检查器配置面板](/Images/AssetGlobalCheckConfigPanel.PNG)

![Asset管线检查器预览面板](/Images/AssetCheckPreviewPanel.PNG)

了解了配置窗口，接下来看看我们是如何在Asset管线框架基础上，实现定义这些自定义的处理器和检查器的。

#### 自定义处理器和检查器

**自定义的检查器需要通过继承BasePreProcessor或BasePostProcessor或BasePreCheck或BasePostCheck来实现的**

文件名预检查器定义:

```CS
/// <summary>
/// CheckFileName.cs
/// 检查文件名
/// </summary>
[CreateAssetMenu(fileName = "CheckFileName", menuName = "ScriptableObjects/AssetPipeline/AssetCheck/PreCheck/CheckFileName", order = 2001)]
public class CheckFileName : BasePreCheck
{
    /// <summary>
    /// 检查器名
    /// </summary>
    public override string Name
    {
        get
        {
            return "检查文件名";
        }
    }

    /// <summary>
    /// 目标Asset类型
    /// </summary>
    public override AssetType TargetAssetType
    {
        get
        {
            return AssetType.All;
        }
    }

    /// <summary>
    /// 目标Asset管线处理类型
    /// </summary>
    public override AssetProcessType TargetAssetProcessType
    {
        get
        {
            return AssetProcessType.CommonPreprocess;
        }
    }

    /// <summary>
    /// 文件名正则匹配
    /// </summary>
    private Regex mFileNameRegex = new Regex("~[!@#$%^&*()_+-=|]");

    /// <summary>
    /// 执行检查器处理
    /// </summary>
    /// <param name="assetPostProcessor"></param>
    /// <param name="paramList">不定长参数列表</param>
    protected override bool DoCheck(AssetPostprocessor assetPostProcessor, params object[] paramList)
    {
        return DoCheckFileName(assetPostProcessor.assetPath);
    }

    /// <summary>
    /// 执行指定路径的检查器处理
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="paramList">不定长参数列表</param>
    protected override bool DoCheckByPath(string assetPath, params object[] paramList)
    {
        return DoCheckFileName(assetPath);
    }

    /// <summary>
    /// 检查文件名
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    private bool DoCheckFileName(string assetPath)
    {
        var fileName = Path.GetFileName(assetPath);
        var result = mFileNameRegex.IsMatch(fileName);
        AssetPipelineLog.Log($"检查AssetPath:{assetPath}文件名匹配结果:{result}".WithColor(Color.yellow));
        return result;
    }
}
```

​	**从上面可以看到我们通过继承BasePreCheck表明我们是实现一个预检查器。通过声明AssetType为AssetType.All表明我们要支持的所有的Asset类型处理。通过声明TargetAssetProcessType为AssetProcessType.CommonPreprocess表示我们要支持通用预处理流程。**

ASTC格式设置预处理器定义:

```CS
/// <summary>
/// ASTCSet.cs
/// ASTC设置
/// </summary>
[CreateAssetMenu(fileName = "ASTCSet", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PreProcessor/ASTCSet", order = 1001)]
public class ASTCSet : BasePreProcessor
{
    /// <summary>
    /// 检查器名
    /// </summary>
    public override string Name
    {
        get
        {
            return "ASTC设置";
        }
    }

    /// <summary>
    /// 目标Asset类型
    /// </summary>
    public override AssetType TargetAssetType
    {
        get
        {
            return AssetType.Texture;
        }
    }

    /// <summary>
    /// 目标Asset管线处理类型
    /// </summary>
    public override AssetProcessType TargetAssetProcessType
    {
        get
        {
            return AssetProcessType.PreprocessTexture;
        }
    }

    /// <summary>
    /// 目标纹理格式
    /// </summary>
    [Header("目标纹理格式")]
    public TextureImporterFormat TargetTextureFormat = TextureImporterFormat.ASTC_4x4;

    /// <summary>
    /// 执行处理器处理
    /// </summary>
    /// <param name="assetPostProcessor"></param>
    /// <param name="paramList">不定长参数列表</param>
    protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
    {
        var assetImporter = assetPostProcessor.assetImporter;
        DoASTCSet(assetImporter);
    }

    /// <summary>
    /// 执行指定路径的处理器处理
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="paramList">不定长参数列表</param>
    protected override void DoProcessorByPath(string assetPath, params object[] paramList)
    {
        var assetImporter = AssetImporter.GetAtPath(assetPath);
        DoASTCSet(assetImporter);
    }

    /// <summary>
    /// 执行ASTC设置
    /// </summary>
    /// <param name="assetImporter"></param>
    private void DoASTCSet(AssetImporter assetImporter)
    {
        var textureImporter = assetImporter as TextureImporter;
        var actiivePlatformName = EditorUtilities.GetPlatformNameByTarget(EditorUserBuildSettings.activeBuildTarget);
        var platformTextureSettings = textureImporter.GetPlatformTextureSettings(actiivePlatformName);
        var automaticFormat = textureImporter.GetAutomaticFormat(actiivePlatformName);
        var isAutomaticASTC = ResourceUtilities.IsASTCFormat(automaticFormat);
        var textureFormat = isAutomaticASTC ? automaticFormat : TargetTextureFormat;
        platformTextureSettings.overridden = true;
        platformTextureSettings.format = textureFormat;
        textureImporter.SetPlatformTextureSettings(platformTextureSettings);
        AssetPipelineLog.Log($"设置AssetPath:{assetImporter.assetPath}纹理压缩格式:{textureFormat}".WithColor(Color.yellow));
    }
}
```

设置AB名后处理器定义:

```CS
/// <summary>
/// GenerateABName.cs
/// 生成AB名处理器
/// </summary>
[CreateAssetMenu(fileName = "GenerateABName", menuName = "ScriptableObjects/AssetPipeline/AssetProcessor/PostProcessor/GenerateABName", order = 1101)]
public class GenerateABName : BasePostProcessor
{
    /// <summary>
    /// 检查器名
    /// </summary>
    public override string Name
    {
        get
        {
            return "生成AB名";
        }
    }

    /// <summary>
    /// 目标Asset类型
    /// </summary>
    public override AssetType TargetAssetType
    {
        get
        {
            return AssetPipelineSystem.GetAllCommonAssetType();
        }
    }

    /// <summary>
    /// 目标Asset管线处理类型
    /// </summary>
    public override AssetProcessType TargetAssetProcessType
    {
        get
        {
            return AssetProcessType.CommonPostprocess;
        }
    }

    /// <summary>
    /// 执行处理器处理
    /// </summary>
    /// <param name="assetPostProcessor"></param>
    /// <param name="paramList">不定长参数列表</param>
    protected override void DoProcessor(AssetPostprocessor assetPostProcessor, params object[] paramList)
    {
        MarkAssetBundleName(assetPostProcessor.assetPath);
    }

    /// <summary>
    /// 执行指定路径的处理器处理
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="paramList">不定长参数列表</param>
    protected override void DoProcessorByPath(string assetPath, params object[] paramList)
    {
        MarkAssetBundleName(assetPath);
    }

    /// <summary>
    /// 标记指定Asset路径的AB名
    /// </summary>
    /// <param name="assetPath"></param>
    private void MarkAssetBundleName(string assetPath)
    {
        var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
        if(asset == null)
        {
            Debug.LogError($"AssetPath:{assetPath}的Asset不存在,标记AB名失败!");
            return;
        }
        var md5 = FileUtilities.GetFilePathMD5(assetPath).ToLower();
        var assetImporter = AssetImporter.GetAtPath(assetPath);
        // 每次修改MD5后会导致Asset处于未保存状态
        // 会导致再次出发PostImported导入流程
        // 为避免不断循环触发PostImpoerted导入流程，MD5不变不触发修改
        // Note:
        // assetBundleName赋值后都是小写
        if (!string.Equals(assetImporter.assetBundleName, md5))
        {
            AssetPipelineLog.Log($"标记AssetPath:{assetPath} MD5:{md5} 原MD5:{assetImporter.assetBundleName}".WithColor(Color.yellow));
            assetImporter.assetBundleName = md5;
        }
        else
        {
            AssetPipelineLog.Log($"AssetPath:{assetPath} MD5相同，不需要更改!".WithColor(Color.yellow));
        }
    }
}
```

​		ASTC设置预处理器和设置AB名后处理器和检查文件名预处理器类似，这里就不一一说明了。

但从上面都可以看到每一个处理器和检查器我们都定义了**CreateAssetMenu和一些自定义序列化属性**，这样我们就可以通过右键创建指定处理器和检查器Asset了。

![自定义处理器Asset创建](/Images/ProcessorCreateOperation.PNG)

![自定义处理器Asset](/Images/CustomProcessorAsset.PNG)

![处理器自定义参数设置](/Images/ProcessorCustomProperty.PNG)

**从上面可以看到同一个类型定义的处理器，我们可以通过创建多个然后改变处理器参数实现细节不同处理效果的处理器需求。**

#### Asset管线处理

接下来让我们看看根据实际Asset管线配置，我们的Asset导入是如何触发一系列配置的。

![Mix文件夹局部预处理器配置](/Images/MixFolderLocalPreProcessorConfig.PNG)

![Mix文件夹Asset导入纹理Log](/Images/MixFolderAssetPipelineLog.PNG)

![Mix文件夹导入纹理处理设置](/Images/MixFolderTextureSettingAfterImport.PNG)

**可以看到通过配置好的Asset管线流程，我们导入到Mix文件夹的纹理图片，成功触发了一系列的处理器和检查器配置，并最终实现了可配置化检查和处理Asset后处理设置的需求。**

## 重点知识

1. **在DoProcessor和DoProcessorByPath流程接口里有params object[] paramList参数，这个参数就是为了兼容不同Asset管线处理流程数据传递问题的。指定处理器流程的处理器或检查器可以通过强转paramList得到Asset管线流程一开始传递的原始数据。**
2. **Asset类型是通过后缀名和类型映射硬编码实现的，如有不支持的文件后置和文件类型，请自行添加。**
3. **检查器只支持了预检查器和后检查器，如果未来有移动和删除检查器的需求也是可以支持的**
4. **检查器目前检查不符合只是打印日志警告，未来想做更多的处理(e.g. 不满足检查设置的自动移动到指定目录去)**

## 注意事项

1. **默认Editor/AssetPipeline/Config/AssetProcessors和Editor/AssetPipeline/Config/AssetChecks目录下才会生成预览，请创建在这两个目录下**
2. **生成AB名处理器会在后处理设置完成后导致Asset处理未保存状态，导致相同Asset设置AB名会重复触发Post Import流程，暂时通过判定AB名是否相同避免重复设置AB名避免重复进入Post Import流程**

## 博客

[可配置Asset管线系统](http://tonytang1990.github.io/2022/07/16/%E5%8F%AF%E9%85%8D%E7%BD%AEAsset%E7%AE%A1%E7%BA%BF%E7%B3%BB%E7%BB%9F/)