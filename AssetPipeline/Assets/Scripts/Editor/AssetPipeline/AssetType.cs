/*
 * Description:             AssetType.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetType.cs
    /// Asset类型
    /// </summary>
    public enum AssetType
    {
        None = 0,               // 无效资源类型
        Object,                 // 不包含Script类型的其他Asset
        Texture,                // 图片
        Material,               // 材质
        SpriteAtlas,            // 图集
        FBX,                    // 模型文件
        AudioClip,              // 音效
        Font,                   // 字体
        Shader,                 // Shader
        Prefab,                 // 预制件
        ScriptableObject,       // ScriptableObject
        TextAsset,              // 文本Asset
        Scene,                  // 场景
        AnimationClip,          // 动画文件
        Mesh,                   // Mesh文件
        Script,                 // 脚本(e.g. cs, dll等)
        Other,                  // 其他
    }
}