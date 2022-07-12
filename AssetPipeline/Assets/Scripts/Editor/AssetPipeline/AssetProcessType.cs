/*
 * Description:             AssetProcessorType.cs
 * Author:                  TONYTANG
 * Create Date:             2022/07/11
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{

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
}