/*
 * Description:             LocalDetailWindow.cs
 * Author:                  TONYTANG
 * Create Date:             2022/07/08
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TAssetPipeline.AssetCheckLocalData;
using static TAssetPipeline.AssetProcessorLocalData;

namespace TAssetPipeline
{
    /// <summary>
    /// LocalDetailWindow.cs
    /// Asset管线局部数据详情配置窗口
    /// </summary>
    public class LocalDetailWindow : BaseEditorWindow
    {
        /// <summary>
        /// 局部数据详情类型
        /// </summary>
        public enum LocalDetailType
        {
            None = 0,                        // 无效类型
            ProcessorLocalDetail,            // 处理器局部详情
            CheckLocalDetail,                // 检查器局部详情
        }

        /// <summary>
        /// Asset管线处理器局部数据详情配置窗口
        /// </summary>
        /// <param name="processorData"></param>
        /// <returns></returns>
        public static void ShowProcessorDetailWindow(ProcessorSettingData processorData)
        {
            var localDetailWindow = EditorWindow.GetWindow<LocalDetailWindow>(false, "局部数据详情配置窗口");
            localDetailWindow.Show();
            localDetailWindow.SetProcessorData(processorData);
        }

        /// <summary>
        /// Asset管线检查器局部数据详情配置窗口
        /// </summary>
        /// <param name="checkData"></param>
        /// <returns></returns>
        public static void ShowCheckDetailWindow(CheckSettingData checkData)
        {
            var localDetailWindow = EditorWindow.GetWindow<LocalDetailWindow>(false, "局部数据详情配置窗口");
            localDetailWindow.Show();
            localDetailWindow.SetCheckData(checkData);
        }

        /// <summary>
        /// 关闭局部详情窗口
        /// </summary>
        public static void CloseLocalDetailWindow()
        {
            if (EditorWindow.HasOpenInstances<LocalDetailWindow>())
            {
                var localDetailWindow = EditorWindow.GetWindow<LocalDetailWindow>(false, "局部数据详情配置窗口");
                localDetailWindow.Close();
            }
        }

        /// <summary>
        /// 局部数据详情类型
        /// </summary>
        private LocalDetailType LocalType = LocalDetailType.None;

        /// <summary>
        /// 当前处理数据
        /// </summary>
        private ProcessorSettingData ProcessorData;

        /// <summary>
        /// 当前检查数据
        /// </summary>
        private CheckSettingData CheckData;

        /// <summary>
        /// UI滚动位置
        /// </summary>
        private Vector2 mScrollPos;

        /// <summary>
        /// 新的目录路径
        /// </summary>
        private string mNewFolerPath = string.Empty;

        /// <summary>
        /// 重置数据
        /// </summary>
        private void ResetData()
        {
            LocalType = LocalDetailType.None;
            ProcessorData = null;
            mNewFolerPath = string.Empty;
        }

        /// <summary>
        /// 设置处理器数据
        /// </summary>
        /// <param name="processorData"></param>
        private void SetProcessorData(ProcessorSettingData processorData)
        {
            ResetData();
            LocalType = LocalDetailType.ProcessorLocalDetail;
            ProcessorData = processorData;
        }

        /// <summary>
        /// 设置检查器数据
        /// </summary>
        /// <param name="checkData"></param>
        private void SetCheckData(CheckSettingData checkData)
        {
            ResetData();
            LocalType = LocalDetailType.CheckLocalDetail;
            CheckData = checkData;
        }

        /// <summary>
        /// 窗口绘制
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            DrawTitleArea();
            mScrollPos = GUILayout.BeginScrollView(mScrollPos);
            DrawContentArea();
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制标题区域
        /// </summary>
        private void DrawTitleArea()
        {
            EditorGUILayout.LabelField($"当前选择局部详情类型:{LocalType}", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
        }

        /// <summary>
        /// 绘制内容区域
        /// </summary>
        private void DrawContentArea()
        {
            if (LocalType == LocalDetailType.None)
            {
                DrawNoneContentArea();
            }
            else if (LocalType == LocalDetailType.ProcessorLocalDetail)
            {
                DrawProcessorContentArea();
            }
            else if (LocalType == LocalDetailType.CheckLocalDetail)
            {
                DrawCheckContentArea();
            }
        }

        /// <summary>
        /// 绘制无效局部详情类型内容区域
        /// </summary>
        private void DrawNoneContentArea()
        {
            EditorGUILayout.LabelField($"未选中有效局部数据对象!", GUILayout.ExpandWidth(true));
        }

        /// <summary>
        /// 绘制处理器局部详情类型内容区域
        /// </summary>
        private void DrawProcessorContentArea()
        {
            DrawProcessorTitleArea();
            DrawProcessorLocalDataArea();
            DrawProcessorBlackListArea();
        }

        /// <summary>
        /// 绘制检查器局部详情类型内容区域
        /// </summary>
        private void DrawCheckContentArea()
        {
            DrawCheckTitleArea();
            DrawCheckLocalDataArea();
            DrawCheckBlackListArea();
        }

        /// <summary>
        /// 绘制处理器标题区域
        /// </summary>
        private void DrawProcessorTitleArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("处理器名", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("目标Asset类型", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("处理器Asset", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("自定义描述", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制处理器局部数据区域
        /// </summary>
        private void DrawProcessorLocalDataArea()
        {
            EditorGUILayout.BeginHorizontal();
            if (ProcessorData != null)
            {
                EditorGUILayout.LabelField(ProcessorData.Processor != null ? ProcessorData.Processor.Name : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
                EditorGUILayout.LabelField(ProcessorData.Processor != null ? ProcessorData.Processor.TargetAssetType.ToString() : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150f));
                EditorGUILayout.ObjectField(ProcessorData.Processor, AssetPipelineConst.BASE_PROCESSOR_TYPE, false, GUILayout.Width(250f));
                EditorGUILayout.LabelField(ProcessorData.Processor != null ? ProcessorData.Processor.CustomDes : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            }
            else
            {
                EditorGUILayout.LabelField($"没有有效的局部处理器数据对象!", GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制处理器黑名单区域
        /// </summary>
        private void DrawProcessorBlackListArea()
        {
            DrawBlackListFolderArea(ProcessorData.BlackListFolderPathList);
        }

        /// <summary>
        /// 绘制检查器标题区域
        /// </summary>
        private void DrawCheckTitleArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("检查器名", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("目标Asset类型", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("检查器Asset", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("自定义描述", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制检查器局部数据区域
        /// </summary>
        private void DrawCheckLocalDataArea()
        {
            EditorGUILayout.BeginHorizontal();
            if (CheckData != null)
            {
                EditorGUILayout.LabelField(CheckData.Check != null ? CheckData.Check.Name : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
                EditorGUILayout.LabelField(CheckData.Check != null ? CheckData.Check.TargetAssetType.ToString() : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150f));
                EditorGUILayout.ObjectField(CheckData.Check, AssetPipelineConst.BASE_CHECK_TYPE, false, GUILayout.Width(250f));
                EditorGUILayout.LabelField(CheckData.Check != null ? CheckData.Check.CustomDes : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            }
            else
            {
                EditorGUILayout.LabelField($"没有有效的局部检查器数据对象!", GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制检查器黑名单区域
        /// </summary>
        private void DrawCheckBlackListArea()
        {
            DrawBlackListFolderArea(CheckData.BlackListFolderPathList);
        }

        /// <summary>
        /// 绘制黑名单目录区域
        /// </summary>
        /// <param name="blackListFolderPathList"></param>
        private void DrawBlackListFolderArea(List<string> blackListFolderPathList)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"黑名单目录", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            DrawBlackListTitleArea();
            for (int i = 0; i < blackListFolderPathList.Count; i++)
            {
                DrawOneBlackListFolder(blackListFolderPathList, i);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("目录路径:", GUILayout.Width(100f));
            EditorGUILayout.TextField(mNewFolerPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择目录路径", GUILayout.Width(150.0f)))
            {
                mNewFolerPath = EditorUtilities.ChoosenProjectFolder(mNewFolerPath);
            }
            if (GUILayout.Button("+", GUILayout.Width(100f)))
            {
                if (!string.IsNullOrEmpty(mNewFolerPath))
                {
                    if (!blackListFolderPathList.Contains(mNewFolerPath))
                    {
                        blackListFolderPathList.Add(mNewFolerPath);
                        blackListFolderPathList.Sort(SortBlackListFolder);
                        Debug.Log($"添加黑名单目录成功!");
                    }
                    else
                    {
                        Debug.LogError($"黑名单目录:{mNewFolerPath}配置已存在，请勿添加重复目录!");
                    }
                }
                else
                {
                    Debug.LogError($"不允许添加空的黑名单目录!");
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制黑名单标题区域
        /// </summary>
        private void DrawBlackListTitleArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("索引", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("黑名单路径", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("操作", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定索引的黑名单目录
        /// </summary>
        /// <param name="blackListFolderPathList"></param>
        /// <param name="index"></param>
        private void DrawOneBlackListFolder(List<string> blackListFolderPathList, int index)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index.ToString(), AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField(blackListFolderPathList[index], AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                blackListFolderPathList.RemoveAt(index);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 黑名单目录排序
        /// </summary>
        /// <param name="folderPath1"></param>
        /// <param name="folderPath2"></param>
        /// <returns></returns>
        private int SortBlackListFolder(string folderPath1, string folderPath2)
        {
            return folderPath1.CompareTo(folderPath2);
        }
    }
}