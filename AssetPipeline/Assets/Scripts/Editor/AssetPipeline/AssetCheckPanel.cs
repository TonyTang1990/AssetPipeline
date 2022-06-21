/*
 * Description:             AssetCheckPanel.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetCheckPanel.cs
    /// Asset检查器面板
    /// </summary>
    public class AssetCheckPanel : BaseEditorPanel
    {
        /// <summary>
        /// Asset检查器页签
        /// </summary>
        public enum AssetCheckTag
        {
            Global = 0,        // 全局处理器
            Local,             // 局部处理器
            Preview,           // 预览
        }

        /// <summary>
        /// Asset检查器页签名字
        /// </summary>
        private string[] mAssetCheckTagNames = new string[3] { "全局配置", "局部配置", "预览" };

        /// <summary>
        /// 当前选择页签索引
        /// </summary>
        private int mSelectedTagIndex;

        /// <summary>
        /// 全局Asset检查器子页签名字
        /// </summary>
        private string[] mGlobalSubTagNames = new string[2] { "预处理器", "后处理器" };

        /// <summary>
        /// 全局当前选择子页签索引
        /// </summary>
        private int mGlobalSelectedSubTagIndex;

        /// <summary>
        /// 局部Asset检查器子页签名字
        /// </summary>
        private string[] mLocalSubTagNames = new string[2] { "预处理器", "后处理器" };

        /// <summary>
        /// 局部当前选择子页签索引
        /// </summary>
        private int mLocalSelectedSubTagIndex;

        /// <summary>
        /// Asset检查器系统UI滚动位置
        /// </summary>
        private Vector2 mAssetCheckScrollPos;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AssetCheckPanel() : base()
        {

        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="ownerEditorWindow"></param>
        /// <param name="panelName"></param>
        public AssetCheckPanel(BaseEditorWindow ownerEditorWindow, string panelName) : base(ownerEditorWindow, panelName)
        {

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void InitData()
        {
            base.InitData();
            mSelectedTagIndex = (int)AssetCheckTag.Global;
            LoadAssetCheckPrefDatas();
            InitAssetCheckData();
        }

        /// <summary>
        /// 加载Asset检查器数据
        /// </summary>
        private void LoadAssetCheckPrefDatas()
        {

        }

        /// <summary>
        /// 初始化Asset检查器数据
        /// </summary>
        private void InitAssetCheckData()
        {

        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public override void SaveData()
        {
            base.SaveData();
            SaveAssetCheckPrefDatas();
            SaveAssetCheckData();
            Debug.Log($"保存Asset检查器数据完成!");
        }

        /// <summary>
        /// 保存Asset检查器本地数据
        /// </summary>
        private void SaveAssetCheckPrefDatas()
        {

        }

        /// <summary>
        /// 保存Asset检查器数据
        /// </summary>
        private void SaveAssetCheckData()
        {

        }

        /// <summary>
        /// 响应绘制
        /// </summary>

        public override void OnGUI()
        {
            DrawAssetCheckTagArea();
            mAssetCheckScrollPos = GUILayout.BeginScrollView(mAssetCheckScrollPos);
            DrawAssetCheckContentArea();
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制Asset检查器页签区域
        /// </summary>
        private void DrawAssetCheckTagArea()
        {
            mSelectedTagIndex = GUILayout.SelectionGrid(mSelectedTagIndex, mAssetCheckTagNames, mAssetCheckTagNames.Length);
        }

        /// <summary>
        /// 绘制Asset检查器内容区域
        /// </summary>
        private void DrawAssetCheckContentArea()
        {
            if (mSelectedTagIndex == (int)AssetCheckTag.Global)
            {
                DrawGlobalAssetCheckArea();
            }
            else if (mSelectedTagIndex == (int)AssetCheckTag.Local)
            {
                DrawLocalAssetCheckArea();
            }
            else if (mSelectedTagIndex == (int)AssetCheckTag.Preview)
            {
                DrawPreviewAssetCheckArea();
            }
        }

        /// <summary>
        /// 绘制全局Asset检查器区域
        /// </summary>
        private void DrawGlobalAssetCheckArea()
        {
            DrawGlobalTagArea();
        }

        /// <summary>
        /// 绘制全局Asset检查器页签区域
        /// </summary>
        private void DrawGlobalTagArea()
        {
            mGlobalSelectedSubTagIndex = GUILayout.SelectionGrid(mGlobalSelectedSubTagIndex, mGlobalSubTagNames, mGlobalSubTagNames.Length);
        }

        /// <summary>
        /// 绘制局部Asset检查器区域
        /// </summary>
        private void DrawLocalAssetCheckArea()
        {
            DrawLocalTagArea();
        }

        /// <summary>
        /// 绘制局部Asset检查器页签区域
        /// </summary>
        private void DrawLocalTagArea()
        {
            mLocalSelectedSubTagIndex = GUILayout.SelectionGrid(mLocalSelectedSubTagIndex, mLocalSubTagNames, mLocalSubTagNames.Length);
        }

        /// <summary>
        /// 绘制预览Asset检查器区域
        /// </summary>
        private void DrawPreviewAssetCheckArea()
        {

        }
    }
}