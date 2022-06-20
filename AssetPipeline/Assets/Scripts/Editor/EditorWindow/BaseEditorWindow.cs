/*
 * Description:             BaseEditorWindow.cs
 * Author:                  TONYTANG
 * Create Date:             2019/12/14
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// BaseEditorWindow.cs
/// 编辑器窗口基类抽象
/// </summary>
public class BaseEditorWindow : EditorWindow
{
    /// <summary>
    /// Panel映射Map<Panel名, Panel对象>
    /// </summary>
    protected Dictionary<string, BaseEditorPanel> mEditorPanelMap;

    /// <summary>
    /// 窗口激活
    /// </summary>
    protected virtual void Awake()
    {
    }

    /// <summary>
    /// 注册所有面板
    /// </summary>
    protected virtual void RegisterAllPanels()
    {
        mEditorPanelMap = new Dictionary<string, BaseEditorPanel>();
    }

    /// <summary>
    /// 激活
    /// </summary>
    protected virtual void OnEnable()
    {
        InitData();
        // 避免没有脚本编译丢失时依然触发面板重新创建
        if(mEditorPanelMap == null)
        {
            RegisterAllPanels();
            foreach (var editorPanel in mEditorPanelMap)
            {
                editorPanel.Value.OnEnable();
            }
        }
    }

    /// <summary>
    /// 不激活
    /// </summary>
    protected virtual void OnDisable()
    {
        SaveData();
        foreach (var editorPanel in mEditorPanelMap)
        {
            editorPanel.Value.OnDisable();
        }
    }

    protected virtual void OnDestroy()
    {
        SaveData();
    }

    /// <summary>
    /// 初始化窗口数据
    /// </summary>
    protected virtual void InitData()
    {

    }

    /// <summary>
    /// 保存数据
    /// </summary>
    protected virtual void SaveData()
    {

    }

    /// <summary>
    /// 创建子面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="panelName"></param>
    protected T CreatePanel<T>(string panelName = null) where T : BaseEditorPanel, new()
    {
        panelName = string.IsNullOrEmpty(panelName) ? typeof(T).Name : panelName;
        if (IsCreatePanel(panelName))
        {
            Debug.LogError($"已经创建了面板:{panelName}类型:{typeof(T).Name},创建面板失败!");
            return null;
        }
        var panel = new T();
        panel.InitPanleInfo(this, panelName);
        mEditorPanelMap.Add(panel.PanelName, panel);
        return panel;
    }

    /// <summary>
    /// 获取指定名字面板
    /// </summary>
    /// <param name="panelName"></param>
    /// <returns></returns>
    protected BaseEditorPanel GetPanel(string panelName)
    {
        if(IsCreatePanel(panelName))
        {
            return mEditorPanelMap[panelName];
        }
        Debug.LogError($"未创建面板:{panelName},获取面板失败!");
        return null;
    }

    /// <summary>
    /// 是否创建了指定窗口
    /// </summary>
    /// <param name="panelName"></param>
    /// <returns></returns>
    protected bool IsCreatePanel(string panelName)
    {
        return mEditorPanelMap.ContainsKey(panelName);
    }

    /// <summary>
    /// 刷新所有面板
    /// </summary>
    public void RefreshAllPanels()
    {
        foreach(var panel in mEditorPanelMap)
        {
            panel.Value.InitData();
        }
    }
}