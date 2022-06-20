/*
 * Description:             EditorUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2021//04/11
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// EditorUtilities.cs
/// 编辑器静态工具类
/// </summary>
public static class EditorUtilities
{
    private static MethodInfo _clearConsoleMethod;
    private static MethodInfo ClearConsoleMethod
    {
        get
        {
            if (_clearConsoleMethod == null)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
                System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
                _clearConsoleMethod = logEntries.GetMethod("Clear");
            }
            return _clearConsoleMethod;
        }
    }

    #region 路径相关
    /// <summary>
    /// 获取项目工程路径
    /// </summary>
    public static string GetProjectPath()
    {
        string projectPath = Path.GetDirectoryName(Application.dataPath);
        return PathUtilities.GetRegularPath(projectPath);
    }

    /// <summary>
    /// 获取项目Asset相对路径
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    public static string GetAssetRelativePath(string fullPath)
    {
        fullPath = PathUtilities.GetRegularPath(fullPath);
        var projectPath = GetProjectPath();
        if(!fullPath.StartsWith(projectPath))
        {
            Debug.LogError($"全路径:{fullPath}没有在项目目录下:{projectPath},获取相对路径失败!");
            return fullPath;
        }
        return fullPath.Substring(projectPath.Length, fullPath.Length - projectPath.Length);
    }

    /// <summary>
    /// 清空文件夹
    /// </summary>
    /// <param name="folderPath">要清理的文件夹路径</param>
    public static void ClearFolder(string directoryPath)
    {
        if (Directory.Exists(directoryPath) == false)
            return;

        // 删除文件
        string[] allFiles = Directory.GetFiles(directoryPath);
        for (int i = 0; i < allFiles.Length; i++)
        {
            File.Delete(allFiles[i]);
        }

        // 删除文件夹
        string[] allFolders = Directory.GetDirectories(directoryPath);
        for (int i = 0; i < allFolders.Length; i++)
        {
            Directory.Delete(allFolders[i], true);
        }
    }
    #endregion

    #region 平台相关
    /// <summary>
    /// 平台类型和平台名映射Map
    /// </summary>
    private static Dictionary<BuildTarget, string> PlatformTargetNameMap = new Dictionary<BuildTarget, string>
    {
        { BuildTarget.StandaloneWindows, "Standalone" },
        { BuildTarget.StandaloneWindows64, "Standalone64" },    // 待确定
        { BuildTarget.Android, "Android" },
        { BuildTarget.iOS, "iPhone" },
        { BuildTarget.StandaloneOSX, "StandaloneOS" },          // 待确定
    };

    /// <summary>
    /// 获取指定平台的平台名
    /// </summary>
    /// <param name="buildTarget"></param>
    public static string GetPlatformNameByTarget(BuildTarget buildTarget)
    {
        string platformName = null;
        if (!PlatformTargetNameMap.TryGetValue(buildTarget, out platformName))
        {
            Debug.LogError($"找不到平台:{buildTarget}的平台名!");
            return null;
        }
        return platformName;
    }
    #endregion

    /// <summary>
    /// 清空控制台
    /// </summary>
    public static void ClearUnityConsole()
    {
        ClearConsoleMethod.Invoke(new object(), null);
    }

    /// <summary>
    /// 是否是数字
    /// </summary>
    public static bool IsNumber(string content)
    {
        if (string.IsNullOrEmpty(content))
            return false;
        string pattern = @"^\d*$";
        return Regex.IsMatch(content, pattern);
    }
}