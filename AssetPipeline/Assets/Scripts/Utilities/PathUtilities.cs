﻿/*
 * Description:             PathUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2021//04/11
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// PathUtilities.cs
/// 路径静态工具类
/// </summary>
public static class PathUtilities
{
    /// <summary>
    /// 获取规范的路径
    /// </summary>
    public static string GetRegularPath(string path)
    {
        return path.Replace('\\', '/').Replace("\\", "/"); //替换为Linux路径格式
    }

    /// <summary>
    /// 获取指定路径的目录名
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public static string GetFolderName(string path)
    {
        var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
        return GetRegularPath(directoryInfo.Name);
    }

    /// <summary>
    /// 获取项目目录全路径
    /// </summary>
    /// <returns></returns>
    public static string GetProjectFullPath()
    {
        var dataPath = Application.dataPath;
        return GetRegularPath(dataPath.Substring(0, dataPath.LastIndexOf("Assets")));
    }

    /// <summary>
    /// 获取资源相对工程目录路径
    /// </summary>
    /// <param name="folderfullpath"></param>
    /// <returns></returns>
    public static string GetProjectRelativeFolderPath(string folderfullpath)
    {
        var projectpathprefix = GetProjectFullPath();
        if (folderfullpath.StartsWith(projectpathprefix))
        {
            var relativefolderpath = folderfullpath.Replace(projectpathprefix, string.Empty);
            return GetRegularPath(relativefolderpath);
        }
        else
        {
            Debug.LogError($"目录:{folderfullpath}不是项目有效路径,获取相对路径失败!");
            return string.Empty;
        }
    }

    /// <summary>
    /// 获取工程Asset的全路径
    /// </summary>
    /// <param name="assetpath">Asset相对路径</param>
    /// <returns></returns>
    public static string GetAssetFullPath()
    {
        return GetRegularPath(Path.GetFullPath(Application.dataPath));
    }

    /// <summary>
    /// 获取指定Asset路径的全路径
    /// </summary>
    /// <param name="assetpath">Asset相对路径</param>
    /// <returns></returns>
    public static string GetAssetFullPath(string assetpath)
    {
        var projectFullPath = GetProjectFullPath();
        return GetRegularPath($"{projectFullPath}{assetpath}");
    }

    /// <summary>
    /// 获取指定路径移除指定后缀名的路径
    /// </summary>
    /// <param name="path"></param>
    /// <param name="postFix"></param>
    /// <returns></returns>
    public static string GetPathWithoutPostFix(string path, string postFix)
    {
        var hasPostFix = !string.IsNullOrEmpty(postFix);
        if(!hasPostFix)
        {
            return path;
        }
        if(!path.EndsWith(postFix))
        {
            Debug.LogError($"Path:{path}没有以后缀:{postFix}结尾,移除失败!");
            return path;
        }
        var postFixIndex = path.LastIndexOf(postFix);
        return GetRegularPath(path.Substring(0, postFixIndex));
    }
}