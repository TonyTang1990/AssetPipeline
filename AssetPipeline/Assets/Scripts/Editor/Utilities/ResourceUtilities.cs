/*
 * Description:             ResourceUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2022/07/10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ResourceUtilities.cs
/// 资源静态工具类
/// </summary>
public static class ResourceUtilities
{
    /// <summary>
    /// 指定格式是否是ASTC格式
    /// </summary>
    /// <param name="textureFormat"></param>
    /// <returns></returns>
    public static bool IsASTCFormat(TextureImporterFormat textureFormat)
    {
        return textureFormat >= TextureImporterFormat.ASTC_4x4 && textureFormat <= TextureImporterFormat.ASTC_12x12;
    }
}