/*
 * Description:             AssetProcessorPanel.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/18
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TAssetPipeline.AssetProcessorLocalData;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetProcessorPanel.cs
    /// Asset处理器器面板
    /// </summary>
    public class AssetProcessorPanel : BaseEditorPanel
    {
        /// <summary>
        /// Asset处理器页签
        /// </summary>
        public enum AssetProcessorTag
        {
            Global = 0,                 // 全局处理器
            Local,                      // 局部处理器
            Preview,                    // 预览
        }

        /// <summary>
        /// Asset处理器全局页签
        /// </summary>
        public enum AssetProcessorGlobalTag
        {
            PreProcessor = 0,           // 预处理器
            PostProcessor,              // 后处理器
            MovedProcessor,             // 移动处理器
            DeletedProcessor,           // 删除处理器
        }

        /// <summary>
        /// Asset处理器局部页签
        /// </summary>
        public enum AssetProcessorLocalTag
        {
            PreProcessor = 0,           // 预处理器
            PostProcessor,              // 后处理器
            MovedProcessor,             // 移动处理器
            DeletedProcessor,           // 删除处理器
        }

        /// <summary>
        /// Asset处理器页签名字
        /// </summary>
        private string[] mAssetProcessorTagNames = new string[3] { "全局配置", "局部配置", "预览" };

        /// <summary>
        /// 当前选择页签索引
        /// </summary>
        private int mSelectedTagIndex;

        /// <summary>
        /// 全局Asset处理器子页签名字
        /// </summary>
        private string[] mGlobalSubTagNames = new string[4] { "预处理器", "后处理器", "移动处理器", "删除处理器" };

        /// <summary>
        /// 全局当前选择子页签索引
        /// </summary>
        private int mGlobalSelectedSubTagIndex;

        /// <summary>
        /// 局部Asset处理器子页签名字
        /// </summary>
        private string[] mLocalSubTagNames = new string[4] { "预处理器", "后处理器", "移动处理器", "删除处理器" };

        /// <summary>
        /// 局部当前选择子页签索引
        /// </summary>
        private int mLocalSelectedSubTagIndex;

        /// <summary>
        /// Asset处理器系统UI滚动位置
        /// </summary>
        private Vector2 mAssetProcessorScrollPos;

        /// <summary>
        /// Asset处理器全局数据
        /// </summary>
        private AssetProcessorGlobalData mGlobalData;

        /// <summary>
        /// Asset处理器局部数据
        /// </summary>
        private AssetProcessorLocalData mLocalData;

        /// <summary>
        /// 所有处理器列表
        /// </summary>
        private List<BaseProcessor> mAllProcessors;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AssetProcessorPanel() : base()
        {

        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="ownerEditorWindow"></param>
        /// <param name="panelName"></param>
        public AssetProcessorPanel(BaseEditorWindow ownerEditorWindow, string panelName) : base(ownerEditorWindow, panelName)
        {

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void InitData()
        {
            base.InitData();
            mSelectedTagIndex = (int)AssetProcessorTag.Global;
            LoadAssetProcessorPrefDatas();
            InitAssetProcessorData();
        }

        /// <summary>
        /// 加载Asset处理器数据
        /// </summary>
        private void LoadAssetProcessorPrefDatas()
        {

        }

        /// <summary>
        /// 初始化Asset处理器数据
        /// </summary>
        private void InitAssetProcessorData()
        {
            var currentConfigStrategy = GetOwnerEditorWindow<AssetPipelineWindow>().AssetPipelinePanel.CurrentConfigStrategy;
            AssetProcessorSystem.MakeSureStrategyFolderExistByStrategy(currentConfigStrategy);
            mGlobalData = AssetProcessorSystem.LoadGlobalDataByStrategy(currentConfigStrategy);
            mLocalData = AssetProcessorSystem.LoadLocalDataByStrategy(currentConfigStrategy);
            mLocalData.UpdateAllProcessorIconDatas();
            mAllProcessors = AssetProcessorSystem.GetAllProcessors();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public override void SaveData()
        {
            base.SaveData();
            SaveAssetProcessorPrefDatas();
            SaveAssetProcessorData();
            Debug.Log($"保存Asset处理器数据完成!");
        }

        /// <summary>
        /// 保存Asset处理器本地数据
        /// </summary>
        private void SaveAssetProcessorPrefDatas()
        {

        }

        /// <summary>
        /// 保存Asset处理器数据
        /// </summary>
        private void SaveAssetProcessorData()
        {
            AssetDatabase.SaveAssetIfDirty(mGlobalData);
            AssetDatabase.SaveAssetIfDirty(mLocalData);
        }

        /// <summary>
        /// 响应绘制
        /// </summary>

        public override void OnGUI()
        {
            DrawAssetProcessorTagArea();
            mAssetProcessorScrollPos = GUILayout.BeginScrollView(mAssetProcessorScrollPos);
            DrawAssetProcessorContentArea();
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制Asset处理器页签区域
        /// </summary>
        private void DrawAssetProcessorTagArea()
        {
            mSelectedTagIndex = GUILayout.SelectionGrid(mSelectedTagIndex, mAssetProcessorTagNames, mAssetProcessorTagNames.Length);
        }

        /// <summary>
        /// 绘制Asset处理器内容区域
        /// </summary>
        private void DrawAssetProcessorContentArea()
        {
            if(mSelectedTagIndex == (int)AssetProcessorTag.Global)
            {
                DrawGlobalAssetProcessorArea();
            }
            else if(mSelectedTagIndex == (int)AssetProcessorTag.Local)
            {
                DrawLocalAssetProcessorArea();
            }
            else if(mSelectedTagIndex == (int)AssetProcessorTag.Preview)
            {
                DrawPreviewAssetProcessorArea();
            }
        }

        /// <summary>
        /// 绘制全局Asset处理器区域
        /// </summary>
        private void DrawGlobalAssetProcessorArea()
        {
            DrawGlobalTagArea();
            DrawGlobalProcessorContentArea();
        }

        /// <summary>
        /// 绘制全局Asset处理器页签区域
        /// </summary>
        private void DrawGlobalTagArea()
        {
            mGlobalSelectedSubTagIndex = GUILayout.SelectionGrid(mGlobalSelectedSubTagIndex, mGlobalSubTagNames, mGlobalSubTagNames.Length);
        }

        /// <summary>
        /// 绘制全局Asset处理器内容区域
        /// </summary>
        private void DrawGlobalProcessorContentArea()
        {
            if (mGlobalSelectedSubTagIndex == (int)AssetProcessorGlobalTag.PreProcessor)
            {
                DrawGlobalPreProcessorArea();
            }
            else if (mGlobalSelectedSubTagIndex == (int)AssetProcessorGlobalTag.PostProcessor)
            {
                DrawGlobalPostProcessorArea();
            }
            else if (mGlobalSelectedSubTagIndex == (int)AssetProcessorGlobalTag.MovedProcessor)
            {
                DrawGlobalMovedProcessorArea();
            }
            else if (mGlobalSelectedSubTagIndex == (int)AssetProcessorGlobalTag.DeletedProcessor)
            {
                DrawGlobalDeletedProcessorArea();
            }
        }

        /// <summary>
        /// 绘制全局预处理器区域
        /// </summary>
        private void DrawGlobalPreProcessorArea()
        {
            DrawProcessorsArea(mGlobalData.PreProcessorData.ProcessorList, mGlobalData.PreProcessorData.ProcessorChosenList);
        }

        /// <summary>
        /// 绘制全局后处理器区域
        /// </summary>
        private void DrawGlobalPostProcessorArea()
        {
            DrawProcessorsArea(mGlobalData.PostProcessorData.ProcessorList, mGlobalData.PostProcessorData.ProcessorChosenList);
        }

        /// <summary>
        /// 绘制全局移动处理器区域
        /// </summary>
        private void DrawGlobalMovedProcessorArea()
        {
            DrawProcessorsArea(mGlobalData.MovedProcessorData.ProcessorList, mGlobalData.MovedProcessorData.ProcessorChosenList);
        }

        /// <summary>
        /// 绘制全局删除处理器区域
        /// </summary>
        private void DrawGlobalDeletedProcessorArea()
        {
            DrawProcessorsArea(mGlobalData.DeletedProcessorData.ProcessorList, mGlobalData.DeletedProcessorData.ProcessorChosenList);
        }

        /// <summary>
        /// 绘制局部Asset处理器区域
        /// </summary>
        private void DrawLocalAssetProcessorArea()
        {
            DrawLocalTagArea();
            DrawLocalProcessorContentArea();
        }

        /// <summary>
        /// 绘制局部Asset处理器页签区域
        /// </summary>
        private void DrawLocalTagArea()
        {
            mLocalSelectedSubTagIndex = GUILayout.SelectionGrid(mLocalSelectedSubTagIndex, mLocalSubTagNames, mLocalSubTagNames.Length);
        }

        /// <summary>
        /// 绘制局部Asset处理器内容区域
        /// </summary>
        private void DrawLocalProcessorContentArea()
        {
            if (mLocalSelectedSubTagIndex == (int)AssetProcessorLocalTag.PreProcessor)
            {
                DrawLocalPreProcessorArea();
            }
            else if (mLocalSelectedSubTagIndex == (int)AssetProcessorLocalTag.PostProcessor)
            {
                DrawLocalPostProcessorArea();
            }
            else if (mLocalSelectedSubTagIndex == (int)AssetProcessorLocalTag.MovedProcessor)
            {
                DrawLocalMovedProcessorArea();
            }
            else if (mLocalSelectedSubTagIndex == (int)AssetProcessorLocalTag.DeletedProcessor)
            {
                DrawLocalDeletedProcessorArea();
            }
        }

        /// <summary>
        /// 绘制局部预处理器区域
        /// </summary>
        private void DrawLocalPreProcessorArea()
        {
            DrawLocalProcessorsArea(mLocalData.PreProcessorDataList);
        }

        /// <summary>
        /// 绘制局部后处理器区域
        /// </summary>
        private void DrawLocalPostProcessorArea()
        {
            DrawLocalProcessorsArea(mLocalData.PostProcessorDataList);
        }

        /// <summary>
        /// 绘制局部移动处理器区域
        /// </summary>
        private void DrawLocalMovedProcessorArea()
        {
            DrawLocalProcessorsArea(mLocalData.MovedProcessorDataList);
        }

        /// <summary>
        /// 绘制局部删除处理器区域
        /// </summary>
        private void DrawLocalDeletedProcessorArea()
        {
            DrawLocalProcessorsArea(mLocalData.DeletedProcessorDataList);
        }

        /// <summary>
        /// 绘制预览Asset处理器区域
        /// </summary>
        private void DrawPreviewAssetProcessorArea()
        {
            EditorGUILayout.BeginVertical("box");
            DrawPreviewProcessorTitleArea();
            for (int i = 0; i < mAllProcessors.Count; i++)
            {
                DrawOneProcessor(mAllProcessors[i]);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制预览处理器标题区域
        /// </summary>
        private void DrawPreviewProcessorTitleArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("处理器名", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("目标Asset类型", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("处理器Asset", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("自定义描述", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定处理器列表
        /// </summary>
        /// <param name="processorList"></param>
        /// <param name="chosenList"></param>
        private void DrawProcessorsArea(List<BaseProcessor> processorList, List<BaseProcessor> chosenList)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProcessorTitleArea();
            for (int i = 0; i < processorList.Count; i++)
            {
                DrawOneProcessorByIndex(processorList, i);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("处理器:", GUILayout.Width(100f));
            chosenList[0] = (BaseProcessor)EditorGUILayout.ObjectField(chosenList[0], AssetPipelineConst.BASE_PROCESSOR_TYPE, false, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("+", GUILayout.Width(100f)))
            {
                if (chosenList[0] == null)
                {
                    Debug.LogError($"不允许添加空处理器,添加处理器失败!");
                }
                else
                {
                    var processorType = chosenList[0].GetType();
                    var findProcessor = processorList.Find(processor => processor.GetType() == processorType);
                    if (findProcessor != null)
                    {
                        Debug.LogError($"不允许添加重复的处理器类型:{findProcessor.GetType().Name},处理器名;{findProcessor.Name},添加处理器失败!");
                    }
                    else
                    {
                        processorList.Add(chosenList[0]);
                        processorList.Sort(AssetPipelineUtilities.SortProcessor);
                        Debug.Log($"添加处理器;{chosenList[0].Name}成功!");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制指定处理器本地数据列表
        /// </summary>
        /// <param name="processorLocalData"></param>
        /// <param name="chosenList"></param>
        private void DrawProcessorsLocalDataArea(ProcessorLocalData processorLocalData, List<BaseProcessor> chosenList)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProcessorTitleArea(false);
            for (int i = 0; i < processorLocalData.ProcessorDataList.Count; i++)
            {
                DrawOneProcessorLocalDataByIndex(processorLocalData, i);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("处理器:", GUILayout.Width(100f));
            chosenList[0] = (BaseProcessor)EditorGUILayout.ObjectField(chosenList[0], AssetPipelineConst.BASE_PROCESSOR_TYPE, false, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("+", GUILayout.Width(100f)))
            {
                if(chosenList[0] == null)
                {
                    Debug.LogError($"不允许添加空处理器,添加处理器失败!");
                }
                else
                {
                    var processorType = chosenList[0].GetType();
                    var findProcessorData = processorLocalData.ProcessorDataList.Find(processorData => processorData.Processor != null && processorData.Processor.TypeName.Equals(processorType.Name));
                    if(findProcessorData != null)
                    {
                        Debug.LogError($"不允许添加重复的处理器类型:{findProcessorData.Processor.TypeName},处理器名;{findProcessorData.Processor.Name},添加处理器失败!");
                    }
                    else
                    {
                        if(processorLocalData.AddProcessorData(chosenList[0]))
                        {
                            Debug.Log($"添加处理器;{chosenList[0].Name}成功!");
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制处理器标题区域
        /// </summary>
        /// <param name="isGlobalProcessor">是否是全局处理器</param>
        private void DrawProcessorTitleArea(bool isGlobalProcessor = true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("索引", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("处理器名", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("目标Asset类型", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("处理器Asset", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("自定义描述", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            if(!isGlobalProcessor)
            {
                EditorGUILayout.LabelField("黑名单目录", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150));
            }
            EditorGUILayout.LabelField("操作", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定索引的单个处理器
        /// </summary>
        /// <param name="processorsList"></param>
        /// <param name="index"></param>
        private void DrawOneProcessorByIndex(List<BaseProcessor> processorsList, int index)
        {
            var processor = processorsList[index];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index.ToString(), AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField(processor.Name, AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField(processor.TargetAssetType.ToString(), AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(250f));
            EditorGUILayout.ObjectField(processor, AssetPipelineConst.BASE_PROCESSOR_TYPE, false, GUILayout.Width(250f));
            EditorGUILayout.LabelField(processor.CustomDes, AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                processorsList.RemoveAt(index);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定索引的单个处理器数据
        /// </summary>
        /// <param name="processorsLocalData"></param>
        /// <param name="index"></param>
        private void DrawOneProcessorLocalDataByIndex(ProcessorLocalData processorsLocalData, int index)
        {
            var processorData = processorsLocalData.ProcessorDataList[index];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index.ToString(), AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField(processorData.Processor != null ? processorData.Processor.Name : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField(processorData.Processor != null ? processorData.Processor.TargetAssetType.ToString() : "无", AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(250f));
            EditorGUILayout.ObjectField(processorData.Processor, AssetPipelineConst.BASE_PROCESSOR_TYPE, false, GUILayout.Width(250f));
            EditorGUILayout.LabelField(processorData.Processor != null ? processorData.Processor.CustomDes : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            if (GUILayout.Button($"数量({processorData.BlackListFolderPathList.Count})", GUILayout.Width(150f)))
            {
                LocalDetailWindow.ShowProcessorDetailWindow(processorsLocalData.FolderPath, processorData);
            }
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                processorsLocalData.RemoveProcessorDataByIndex(index);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定处理器
        /// </summary>
        /// <param name="processor"></param>
        private void DrawOneProcessor(BaseProcessor processor)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(processor.Name, AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField(processor.TargetAssetType.ToString(), AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(250f));
            EditorGUILayout.ObjectField(processor, AssetPipelineConst.BASE_PROCESSOR_TYPE, false, GUILayout.Width(250f));
            EditorGUILayout.LabelField(processor.CustomDes, AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定局部处理器区域
        /// </summary>
        /// <param name="processorLocalDataList"></param>
        private void DrawLocalProcessorsArea(List<ProcessorLocalData> processorLocalDataList)
        {
            EditorGUILayout.BeginVertical("box");
            for(int i = 0; i < processorLocalDataList.Count; i++)
            {
                DrawOneLocalProcessorsByIndex(processorLocalDataList, i);
            }
            if (GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                processorLocalDataList.Add(new ProcessorLocalData());
                processorLocalDataList.Sort(SortLocalProcesorData);
                Debug.Log($"添加局部处理器数据成功!");
            }
            if (GUILayout.Button("折叠所有", GUILayout.ExpandWidth(true)))
            {
                FoldAllLocalProcessorData(processorLocalDataList);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制指定索引的局部处理器数据
        /// </summary>
        /// <param name="processorLocalDataList"></param>
        /// <param name="index"></param>
        private void DrawOneLocalProcessorsByIndex(List<ProcessorLocalData> processorLocalDataList, int index)
        {
            var processorLocalData = processorLocalDataList[index];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            processorLocalData.IsUnFold = EditorGUILayout.Foldout(processorLocalData.IsUnFold, processorLocalData.FolderPath, true);
            DrawProcessorLocalDataIcons(processorLocalData);
            if(GUILayout.Button("-", GUILayout.Width(100f)))
            {
                processorLocalDataList.RemoveAt(index);
            }
            EditorGUILayout.EndHorizontal();
            if (processorLocalData.IsUnFold)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目录路径:", GUILayout.Width(100f));
                EditorGUILayout.TextField(processorLocalData.FolderPath, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("选择目录路径", GUILayout.Width(150.0f)))
                {
                    var newFolderPath = EditorUtilities.ChoosenProjectFolder(processorLocalData.FolderPath);
                    if(!newFolderPath.Equals(processorLocalData.FolderPath) &&
                        !CheckFolderPathExist(processorLocalDataList, newFolderPath))
                    {
                        processorLocalData.FolderPath = newFolderPath;
                        processorLocalDataList.Sort(SortLocalProcesorData);
                    }
                    else
                    {
                        Debug.LogError($"局部目录:{newFolderPath}配置已存在，请勿设置重复局部目录!!");
                    }
                }
                EditorGUILayout.EndHorizontal();
                DrawProcessorsLocalDataArea(processorLocalData, processorLocalData.ProcessorChosenList);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制本地处理器数据所有Icon
        /// </summary>
        /// <param name="processorLocalData"></param>
        private void DrawProcessorLocalDataIcons(ProcessorLocalData processorLocalData)
        {
            for(int i = 0; i < processorLocalData.ProcessorIconList.Count; i++)
            {
                EditorGUILayout.LabelField(processorLocalData.ProcessorIconList[i], GUILayout.Width(20f));
            }
        }

        /// <summary>
        /// 折叠所有本地处理器数据配置
        /// </summary>
        /// <param name="processorLocalDataList"></param>
        private void FoldAllLocalProcessorData(List<ProcessorLocalData> processorLocalDataList)
        {
            foreach(var processorLocalData in processorLocalDataList)
            {
                processorLocalData.IsUnFold = false;
            }
        }

        /// <summary>
        /// 本地处理器数据排序
        /// </summary>
        /// <param name="localData1"></param>
        /// <param name="localData2"></param>
        /// <returns></returns>
        private int SortLocalProcesorData(ProcessorLocalData localData1, ProcessorLocalData localData2)
        {
            return localData1.FolderPath.CompareTo(localData2.FolderPath);
        }

        /// <summary>
        /// 检查指定局部目录配置是否已存在
        /// </summary>
        /// <param name="processorLocalDataList"></param>
        /// <param name="newFolderPath"></param>
        /// <returns></returns>
        private bool CheckFolderPathExist(List<ProcessorLocalData> processorLocalDataList, string newFolderPath)
        {
            if(processorLocalDataList == null)
            {
                return false;
            }
            foreach(var processorLocalData in processorLocalDataList)
            {
                if(processorLocalData.FolderPath.Equals(newFolderPath))
                {
                    return true;
                }
            }
            return false;
        }
    }
}