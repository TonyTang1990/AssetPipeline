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
        Folder = 1 << 14,                 // 文件夹
        Other = 1 << 15,                  // 其他
        All = Int32.MaxValue,             // 所有类型
    }
}