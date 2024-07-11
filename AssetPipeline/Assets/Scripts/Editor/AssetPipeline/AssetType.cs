/*
 * Description:             AssetType.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/18
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetType.cs
    /// Asset类型
    /// </summary>
    [Flags]
    public enum AssetType : Int64
    {
        None = 0,                               // 无效类型
        Texture = 1 << 0,                       // 图片
        Material = 1 << 1,                      // 材质
        SpriteAtlas = 1 << 2,                   // 图集
        FBX = 1 << 3,                           // 模型文件
        AudioClip = 1 << 4,                     // 音效
        Font = 1 << 5,                          // 字体
        Shader = 1 << 7,                        // Shader
        ShaderVariantCollection = 1 << 8,       // Shader变体文件
        Prefab = 1 << 9,                        // 预制件
        ScriptableObject = 1 << 10,             // ScriptableObject
        TextAsset = 1 << 11,                    // 文本Asset
        Scene = 1 << 12,                        // 场景
        AnimationClip = 1 << 13,                // 动画文件
        AnimatorController = 1 << 14,           // 动画控制器文件
        AnimatorOverrideController = 1 << 15,   // 动画子控制器文件
        Mesh = 1 << 16,                         // Mesh文件
        VideoClip = 1 << 17,                    // 视频
        RenderTexture = 1 << 18,                // RenderTexture
        TimelineAsset = 1 << 19,                // Timeline Asset
        LightingSetting = 1 << 20,              // 光照设置
        Script = 1 << 21,                       // 脚本(e.g. cs)
        Folder = 1 << 22,                       // 文件夹
        GUISkin = 1 << 23,                      // GUISkin
        Preset = 1 << 24,                       // Preset
        AssemblyDefinitionAsset = 1 << 25,      // AssemblyDefinitionAsset
        StyleSheet = 1 << 26,                   // Style Sheet(e.g. .uss ......)
        DefaultAsset = 1 << 27,                 // Unity不识别的Asset类型(e.g. dll, exe, so, a......)
        Other = 1 << 28,                        // 其他
        All = Int64.MaxValue,                   // 所有类型
    }
}