/*
 * Description:             AssetInfo.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/28
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AssetInfo.cs
/// Asset信息(用于记录Asset路径和对应的Asset类型信息)
/// </summary>
[Serializable]
public class AssetInfo
{
    /// <summary>
    /// Asset路径
    /// </summary>
    [Header("Asset路径")]
    public string AssetPath;

    /// <summary>
    /// Asset类型全名
    /// </summary>
    [Header("Asset类型全名")]
    public string AssetTypeFullName;

    public AssetInfo(string assetPath, string assetTypeFullName)
    {
        AssetPath = assetPath;
        AssetTypeFullName = assetTypeFullName;
    }
}