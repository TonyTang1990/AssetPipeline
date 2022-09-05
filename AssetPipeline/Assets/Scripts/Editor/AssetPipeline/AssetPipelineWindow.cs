/*
 * Description:             AssetPipelineWindow.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetPipelineWindow.cs
    /// Asset管线可配置化配置窗口
    /// </summary>
    public class AssetPipelineWindow : BaseEditorWindow
    {
        #region 公共部分
        [MenuItem("Tools/Asset/Asset管线窗口", priority = 200)]
        static void ShowWindow()
        {
            var assetPipelineWindow = EditorWindow.GetWindow<AssetPipelineWindow>(false, "Asset管线窗口");
            assetPipelineWindow.Show();
        }

        /// <summary>
        /// Asset管线页签
        /// </summary>
        public enum AssetPipelineTag
        {
            AssetPipelineSystem = 0,        // Asset管线设置
            AssetProcessorSystem,           // Asset处理器系统
            AssetCheckSystem,               // Asset检查系统
        }

        /// <summary>
        /// Asset管线页签名字
        /// </summary>
        private string[] mAssetPipelineTagNames = new string[3] { "Asset管线", "Asset处理器", "Asset检查器" };

        /// <summary>
        /// 当前选择页签索引
        /// </summary>
        private int mCurrentSelectedTagIndex;

        /// <summary>
        /// Asset管线面板
        /// </summary>
        public AssetPipelinePanel AssetPipelinePanel
        {
            get;
            private set;
        }

        /// <summary>
        /// Asset处理器面板
        /// </summary>
        public AssetProcessorPanel AssetProcessorPanel
        {
            get;
            private set;
        }

        /// <summary>
        /// Asset检查器面板
        /// </summary>
        public AssetCheckPanel AssetCheckPanel
        {
            get;
            private set;
        }

        /// <summary>
        /// 注册所有面板
        /// </summary>
        protected override void RegisterAllPanels()
        {
            base.RegisterAllPanels();
            AssetPipelinePanel = CreatePanel<AssetPipelinePanel>();
            AssetProcessorPanel = CreatePanel<AssetProcessorPanel>();
            AssetCheckPanel = CreatePanel<AssetCheckPanel>();
        }


        /// <summary>
        /// 加载数据
        /// </summary>
        public override void LoadAllData()
        {
            base.LoadAllData();
            mCurrentSelectedTagIndex = (int)AssetPipelineTag.AssetPipelineSystem;
        }

        /// <summary>
        /// 排序所有数据
        /// </summary>
        public void SortAllData()
        {
            var assetProcessorPanel = GetPanel<AssetProcessorPanel>();
            assetProcessorPanel.SortAllData();
            var assetCheckPanel = GetPanel<AssetCheckPanel>();
            assetCheckPanel.SortAllData();
            Debug.Log($"排序所有Asset管线数据完成!");
        }

        /// <summary>
        /// 刷新成员值
        /// </summary>
        public void RefreshMemberValue()
        {
            var assetProcessorPanel = GetPanel<AssetProcessorPanel>();
            assetProcessorPanel.RefreshMemberValue();
            var assetCheckPanel = GetPanel<AssetCheckPanel>();
            assetCheckPanel.RefreshMemberValue();
            Debug.Log($"刷新所有Asset处理器和检查器成员值完成!");
        }

        /// <summary>
        /// 保存所有数据
        /// </summary>
        public override void SaveAllData()
        {
            // 保存Asset处理器和检查器数据之前必须先把Asset相关路径信息刷新好
            RefreshMemberValue();
            base.SaveAllData();
            Debug.Log($"保存所有Asset管线数据完成!");
            // 强制重载Assset管线数据，确保加载使用最新的Asset管线设置数据
            AssetPipelineSystem.Init();
        }

        /// <summary>
        /// 窗口绘制
        /// </summary>
        private void OnGUI()
        {
            if(AssetPipelinePanel != null && AssetProcessorPanel != null && AssetCheckPanel != null)
            {
                EditorGUILayout.BeginVertical();
                DrawTagArea();
                DrawContentArea();
                EditorGUILayout.EndVertical();
            }
            else
            {
                var preColor = GUI.color;
                GUI.color = Color.green;
                if (GUILayout.Button("重新加载配置数据", GUILayout.ExpandWidth(true)))
                {
                    LoadAllData();
                }
                GUI.color = preColor;
            }
        }

        /// <summary>
        /// 绘制页签区域
        /// </summary>
        private void DrawTagArea()
        {
            mCurrentSelectedTagIndex = GUILayout.SelectionGrid(mCurrentSelectedTagIndex, mAssetPipelineTagNames, mAssetPipelineTagNames.Length);
        }

        /// <summary>
        /// 绘制页签内容区域
        /// </summary>
        private void DrawContentArea()
        {
            if (mCurrentSelectedTagIndex == (int)AssetPipelineTag.AssetPipelineSystem)
            {
                AssetPipelinePanel?.OnGUI();
            }
            else if (mCurrentSelectedTagIndex == (int)AssetPipelineTag.AssetProcessorSystem)
            {
                AssetProcessorPanel?.OnGUI();
            }
            else if (mCurrentSelectedTagIndex == (int)AssetPipelineTag.AssetCheckSystem)
            {
                AssetCheckPanel?.OnGUI();
            }
        }
        #endregion
    }
}