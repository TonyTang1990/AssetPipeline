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

    #region Editor GUI相关
    /// <summary>
    /// 选择一个项目目录下的目录
    /// </summary>
    /// <param name="originalFolderPath"></param>
    /// <returns>返回相对工程目录并以/结尾的相对目录(e.g. Assets/)</returns>
    public static string ChoosenProjectFolder(string originalFolderPath)
    {
        var preFolderPath = originalFolderPath;
        var newFolderPath = EditorUtility.OpenFolderPanel("目录路径", "请选择目录路径!", originalFolderPath);
        if (string.IsNullOrEmpty(newFolderPath))
        {
            return preFolderPath;
        }
        newFolderPath = $"{newFolderPath}/";
        var relativePath = PathUtilities.GetProjectRelativeFolderPath(newFolderPath);
        if (string.IsNullOrEmpty(relativePath))
        {
            Debug.LogError($"选择的目录:{newFolderPath}不在项目目录下，设置目录失败!");
            return preFolderPath;
        }
        return relativePath;
    }
    #endregion
}