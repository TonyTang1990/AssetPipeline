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
        LoadAllData();
    }

    /// <summary>
    /// 加载所有数据
    /// </summary>
    public virtual void LoadAllData()
    {
        RegisterAllPanels();
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
        if(mEditorPanelMap != null)
        {
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
        if (mEditorPanelMap != null)
        {
            foreach (var editorPanel in mEditorPanelMap)
            {
                editorPanel.Value.OnDisable();
            }
        }
    }

    /// <summary>
    /// 响应销毁
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (mEditorPanelMap != null)
        {
            foreach (var editorPanel in mEditorPanelMap)
            {
                editorPanel.Value.OnDestroy();
            }
        }
        SaveAllData();
    }

    /// <summary>
    /// 保存所有数据
    /// </summary>
    public virtual void SaveAllData()
    {
        if (mEditorPanelMap != null)
        {
            foreach (var editorPanel in mEditorPanelMap)
            {
                editorPanel.Value.SaveAllData();
            }
        }
    }

    /// <summary>
    /// 创建子面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    protected T CreatePanel<T>() where T : BaseEditorPanel, new()
    {
        string panelName = typeof(T).Name;
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
    protected T GetPanel<T>() where T : BaseEditorPanel, new()
    {
        string panelName = typeof(T).Name;
        if (IsCreatePanel(panelName))
        {
            return mEditorPanelMap[panelName] as T;
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
        return mEditorPanelMap != null ? mEditorPanelMap.ContainsKey(panelName) : false;
    }

    /// <summary>
    /// 刷新所有面板
    /// </summary>
    public void RefreshAllPanels()
    {
        if(mEditorPanelMap != null)
        {
            foreach (var panel in mEditorPanelMap)
            {
                panel.Value.LoadAllData();
            }
        }
    }
}