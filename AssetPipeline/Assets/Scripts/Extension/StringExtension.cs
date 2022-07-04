/*
 * Description:             StringExtension.cs
 * Author:                  TONYTANG
 * Create Date:             2018/08/08
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StringExtension.cs
/// String扩展方法
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// 颜色字符串值Map<颜色类型, 颜色字符串值>
    /// </summary>
    private static Dictionary<Color, string> ColorStringValueMap = new Dictionary<Color, string>
    {
        { Color.white, "FFFFFF" },
        { Color.black, "000000" },
        { Color.red, "FF0000" },
        { Color.green, "00FF00" },
        { Color.blue, "0000FF" },
        { Color.cyan, "00FFFF" },
        { Color.magenta, "FF00FF" },
        { Color.yellow, "FFFF00" },
        { Color.gray, "7F7F7F" },
    };

    /// <summary>
    /// 判定字符串是否为null或者""
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }

    /// <summary>
    /// 移除首个字符
    /// </summary>
    public static string RemoveFirstChar(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        return str.Substring(1);
    }

    /// <summary>
    /// 移除末尾字符
    /// </summary>
    public static string RemoveLastChar(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        return str.Substring(0, str.Length - 1);
    }

    /// <summary>
    /// 添加颜色信息
    /// </summary>
    /// <param name="str"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string WithColor(this string str, Color color)
    {
        string colorValue;
        if (!ColorStringValueMap.TryGetValue(color, out colorValue))
        {
            Debug.LogError($"不支持的颜色:{color},获取带颜色信息字符串失败!");
            return str;
        }
        return $"<color=#{colorValue}>{str}</color>";
    }
}