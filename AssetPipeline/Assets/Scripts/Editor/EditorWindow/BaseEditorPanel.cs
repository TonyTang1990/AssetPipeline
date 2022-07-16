/*
 * Description:             BaseEditorPanel.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BaseEditorPanel.cs
/// Editor面板抽象
/// </summary>
public abstract class BaseEditorPanel
{
    /// <summary>
    /// 面板名
    /// </summary>
    public string PanelName
    {
        get;
        private set;
    }

    /// <summary>
    /// 所属Editor窗口
    /// </summary>
    public BaseEditorWindow OwnerEditorWindow
    {
        get;
        private set;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseEditorPanel()
    {

    }

    /// <summary>
    /// 带参构造函数
    /// </summary>
    /// <param name="ownerEditorWindow"></param>
    public BaseEditorPanel(BaseEditorWindow ownerEditorWindow, string panelName = null)
    {
        InitPanleInfo(ownerEditorWindow, panelName);
    }

    /// <summary>
    /// 初始化面板信息
    /// </summary>
    /// <param name="ownerEditorWindow"></param>
    /// <param name="panelName"></param>
    public void InitPanleInfo(BaseEditorWindow ownerEditorWindow, string panelName = null)
    {
        if(ownerEditorWindow == null)
        {
            Debug.LogError($"构建面板不允许不传所属窗口,构建面板失败!");
            return;
        }
        OwnerEditorWindow = ownerEditorWindow;
        PanelName = string.IsNullOrEmpty(panelName) ? this.GetType().Name : panelName;
        LoadAllData();
    }

    /// <summary>
    /// 获取所属窗口
    /// </summary>
    /// <returns></returns>
    protected T GetOwnerEditorWindow<T>() where T : BaseEditorWindow
    {
        return OwnerEditorWindow as T;
    }

    /// <summary>
    /// 加载所有数据
    /// </summary>
    public virtual void LoadAllData()
    {

    }

    /// <summary>
    /// 响应激活
    /// </summary>
    public void OnEnable()
    {

    }

    /// <summary>
    /// 响应不激活
    /// </summary>
    public void OnDisable()
    {

    }

    /// <summary>
    ///  响应销毁
    /// </summary>
    public virtual void OnDestroy()
    {

    }

    /// <summary>
    /// 保存所有数据
    /// </summary>
    public virtual void SaveAllData()
    {

    }

    /// <summary>
    /// 响应绘制
    /// </summary>
    public abstract void OnGUI();
}