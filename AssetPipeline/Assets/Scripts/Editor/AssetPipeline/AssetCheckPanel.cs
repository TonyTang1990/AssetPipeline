/*
 * Description:             AssetCheckPanel.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/18
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static TAssetPipeline.AssetCheckGlobalData;
using static TAssetPipeline.AssetCheckLocalData;

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
            Global = 0,        // 全局检查器
            Local,             // 局部检查器
            Preview,           // 预览
        }

        /// <summary>
        /// Asset检查器全局页签
        /// </summary>
        public enum AssetCheckGlobalTag
        {
            PreProcessor = 0,  // 预检查器
            PostProcessor,     // 后检查器
        }

        /// <summary>
        /// Asset检查器局部页签
        /// </summary>
        public enum AssetCheckLocalTag
        {
            PreProcesser = 0,  // 预检查器
            PostProcessor,     // 后检查器
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
        private string[] mGlobalSubTagNames = new string[2] { "预检查器", "后检查器" };

        /// <summary>
        /// 全局当前选择子页签索引
        /// </summary>
        private int mGlobalSelectedSubTagIndex;

        /// <summary>
        /// 局部Asset检查器子页签名字
        /// </summary>
        private string[] mLocalSubTagNames = new string[2] { "预检查器", "后检查器" };

        /// <summary>
        /// 局部当前选择子页签索引
        /// </summary>
        private int mLocalSelectedSubTagIndex;

        /// <summary>
        /// Asset检查器系统UI滚动位置
        /// </summary>
        private Vector2 mAssetCheckScrollPos;

        /// <summary>
        /// Asset检查器全局数据
        /// </summary>
        private AssetCheckGlobalData mGlobalData;

        /// <summary>
        /// Asset检查器局部数据
        /// </summary>
        private AssetCheckLocalData mLocalData;

        /// <summary>
        /// 所有预检查器列表
        /// </summary>
        private List<BasePreCheck> mAllPreChecks;

        /// <summary>
        /// 所有后检查器列表
        /// </summary>
        private List<BasePostCheck> mAllPostChecks;

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
        /// 加载所有数据
        /// </summary>
        public override void LoadAllData()
        {
            base.LoadAllData();
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
            var currentConfigStrategy = GetOwnerEditorWindow<AssetPipelineWindow>().AssetPipelinePanel.CurrentConfigStrategy;
            AssetCheckSystem.MakeSureStrategyFolderExistByStrategy(currentConfigStrategy);
            mGlobalData = AssetCheckSystem.LoadGlobalDataByStrategy(currentConfigStrategy);
            mLocalData = AssetCheckSystem.LoadLocalDataByStrategy(currentConfigStrategy);
            mLocalData.UpdateAllCheckIconDatas();
            UpdateAllCheck();
        }

        /// <summary>
        /// 更新所有处理器列表
        /// </summary>
        private void UpdateAllCheck()
        {
            mAllPreChecks = AssetCheckSystem.GetAllChecks<BasePreCheck>();
            mAllPostChecks = AssetCheckSystem.GetAllChecks<BasePostCheck>();
        }

        /// <summary>
        /// 排序所有数据
        /// </summary>
        public void SortAllData()
        {
            mGlobalData.SortAllData();
            mLocalData.SortAllData();
            mAllPreChecks.Sort(AssetPipelineUtilities.SortCheck);
            mAllPostChecks.Sort(AssetPipelineUtilities.SortCheck);
        }

        /// <summary>
        /// 刷新成员值
        /// </summary>
        public void RefreshMemberValue()
        {
            mGlobalData.RefreshMemberValue();
            mLocalData.RefreshMemberValue();
        }

        /// <summary>
        /// 保存所有数据
        /// </summary>
        public override void SaveAllData()
        {
            base.SaveAllData();
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
            var currentConfigStrategy = GetOwnerEditorWindow<AssetPipelineWindow>().AssetPipelinePanel.CurrentConfigStrategy;
            if (mGlobalData != null)
            {
                EditorUtility.SetDirty(mGlobalData);
                AssetDatabase.SaveAssetIfDirty(mGlobalData);
                AssetCheckSystem.SaveGlobalDataToJsonByStrategy(mGlobalData, currentConfigStrategy);
            }
            if (mLocalData != null)
            {
                EditorUtility.SetDirty(mLocalData);
                AssetDatabase.SaveAssetIfDirty(mLocalData);
                AssetCheckSystem.SaveLocalDataToJsonByStrategy(mLocalData, currentConfigStrategy);
            }
            // 确保保存所有最新的
            UpdateAllCheck();
            SaveAllCheckToJson();
            SaveAllCheckInfoToJson();
        }

        /// <summary>
        /// 保存所有检查器到Json
        /// </summary>
        private void SaveAllCheckToJson()
        {
            SaveAllPreCheckToJson();
            SaveAllPostCheckToJson();
            Debug.Log($"保存所有检查器的Json数据完成!".WithColor(Color.green));
        }

        /// <summary>
        /// 保存所有预检查器到Json
        /// </summary>
        private void SaveAllPreCheckToJson()
        {
            foreach (var preCheck in mAllPreChecks)
            {
                SaveCheckToJson(preCheck);
            }
        }

        /// <summary>
        /// 保存所有后检查器到Json
        /// </summary>
        private void SaveAllPostCheckToJson()
        {
            foreach (var postCheck in mAllPostChecks)
            {
                SaveCheckToJson(postCheck);
            }
        }

        /// <summary>
        /// 保存检查器到Json
        /// </summary>
        /// <param name="check"></param>
        private void SaveCheckToJson(BaseCheck check)
        {
            AssetCheckSystem.SaveCheckToJson(check);
        }

        /// <summary>
        /// 保存所有检查器信息到Json
        /// </summary>
        private void SaveAllCheckInfoToJson()
        {
            var assetCheckInfoData = new AssetCheckInfoData();
            foreach (var preCheck in mAllPreChecks)
            {
                assetCheckInfoData.AddCheckInfo(preCheck);
            }
            foreach (var postCheck in mAllPostChecks)
            {
                assetCheckInfoData.AddCheckInfo(postCheck);
            }
            AssetCheckSystem.SaveAssetCheckInfoData(assetCheckInfoData);
        }

        /// <summary>
        /// 响应绘制
        /// </summary>

        public override void OnGUI()
        {
            if(mGlobalData != null && mLocalData != null)
            {
                DrawAssetCheckTagArea();
                mAssetCheckScrollPos = GUILayout.BeginScrollView(mAssetCheckScrollPos);
                DrawAssetCheckContentArea();
                GUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField($"未加载有效配置数据!", GUILayout.ExpandWidth(true));
            }
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
            DrawGlobalCheckContentArea();
        }

        /// <summary>
        /// 绘制全局Asset检查器页签区域
        /// </summary>
        private void DrawGlobalTagArea()
        {
            mGlobalSelectedSubTagIndex = GUILayout.SelectionGrid(mGlobalSelectedSubTagIndex, mGlobalSubTagNames, mGlobalSubTagNames.Length);
        }

        /// <summary>
        /// 绘制全局Asset检查器内容区域
        /// </summary>
        private void DrawGlobalCheckContentArea()
        {
            if (mGlobalSelectedSubTagIndex == (int)AssetCheckGlobalTag.PreProcessor)
            {
                DrawGlobalPreCheckArea();
            }
            else if (mGlobalSelectedSubTagIndex == (int)AssetCheckGlobalTag.PostProcessor)
            {
                DrawGlobalPostCheckArea();
            }
        }

        /// <summary>
        /// 绘制全局预检查器区域
        /// </summary>
        private void DrawGlobalPreCheckArea()
        {
            DrawChecksArea(mGlobalData.PreCheckData, AssetPipelineConst.BASE_PRE_CHECK_TYPE);
        }

        /// <summary>
        /// 绘制全局后检查器区域
        /// </summary>
        private void DrawGlobalPostCheckArea()
        {
            DrawChecksArea(mGlobalData.PostCheckData, AssetPipelineConst.BASE_POST_CHECK_TYPE);
        }

        /// <summary>
        /// 绘制局部Asset检查器区域
        /// </summary>
        private void DrawLocalAssetCheckArea()
        {
            DrawLocalTagArea();
            DrawLocalCheckContentArea();
        }

        /// <summary>
        /// 绘制局部Asset检查器页签区域
        /// </summary>
        private void DrawLocalTagArea()
        {
            mLocalSelectedSubTagIndex = GUILayout.SelectionGrid(mLocalSelectedSubTagIndex, mLocalSubTagNames, mLocalSubTagNames.Length);
        }

        /// <summary>
        /// 绘制局部Asset检查器内容区域
        /// </summary>
        private void DrawLocalCheckContentArea()
        {
            if (mLocalSelectedSubTagIndex == (int)AssetCheckLocalTag.PreProcesser)
            {
                DrawLocalPreCheckArea();
            }
            else if (mLocalSelectedSubTagIndex == (int)AssetCheckLocalTag.PostProcessor)
            {
                DrawLocalPostCheckArea();
            }
        }


        /// <summary>
        /// 绘制局部预检查器区域
        /// </summary>
        private void DrawLocalPreCheckArea()
        {
            DrawLocalChecksArea(mLocalData.PreCheckDataList, AssetPipelineConst.BASE_PRE_CHECK_TYPE);
        }

        /// <summary>
        /// 绘制局部后检查器区域
        /// </summary>
        private void DrawLocalPostCheckArea()
        {
            DrawLocalChecksArea(mLocalData.PostCheckDataList, AssetPipelineConst.BASE_POST_CHECK_TYPE);
        }

        /// <summary>
        /// 绘制预览Asset检查器区域
        /// </summary>
        private void DrawPreviewAssetCheckArea()
        {
            EditorGUILayout.BeginVertical("box");
            DrawPreviewCheckTitleArea("预检查器");
            for (int i = 0; i < mAllPreChecks.Count; i++)
            {
                DrawOneCheck(mAllPreChecks[i]);
            }
            EditorGUILayout.Space();
            DrawPreviewCheckTitleArea("后检查器");
            for (int i = 0; i < mAllPostChecks.Count; i++)
            {
                DrawOneCheck(mAllPostChecks[i]);
            }
            var preColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("刷新预览", GUILayout.ExpandWidth(true)))
            {
                UpdateAllCheck();
            }
            GUI.color = preColor;
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制预览检查器标题区域
        /// </summary>
        /// <param name="title"></param>
        private void DrawPreviewCheckTitleArea(string title)
        {
            var preColor = GUI.color;
            GUI.color = Color.yellow;
            EditorGUILayout.LabelField(title, AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            GUI.color = preColor;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("检查器名", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("目标Asset类型", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("Asset处理类型", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("排序Order", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(80f));
            EditorGUILayout.LabelField("检查器Asset", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("自定义描述", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定检查器
        /// </summary>
        /// <param name="check"></param>
        private void DrawOneCheck(BaseCheck check)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(check.Name, AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField(check.TargetAssetType.ToString(), AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField(check.TargetAssetProcessType.ToString(), AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(150f));
            EditorGUILayout.IntField(check.Order, AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(80f));
            EditorGUILayout.ObjectField(check, AssetPipelineConst.BASE_CHECK_TYPE, false, GUILayout.Width(250f));
            EditorGUILayout.LabelField(check.CustomDes, AssetPipelineStyles.ButtonMidStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定局部检查器区域
        /// </summary>
        /// <param name="checkLocalDataList"></param>
        /// <param name="checkType"></param>
        private void DrawLocalChecksArea(List<CheckLocalData> checkLocalDataList, Type checkType)
        {
            EditorGUILayout.BeginVertical("box");
            for (int i = 0; i < checkLocalDataList.Count; i++)
            {
                DrawOneLocalChecksByIndex(checkLocalDataList, i, checkType);
            }
            var preColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                checkLocalDataList.Add(new CheckLocalData(true));
                checkLocalDataList.Sort(SortLocalCheckData);
                Debug.Log($"添加局部检查器数据成功!");
            }
            if (GUILayout.Button("折叠所有", GUILayout.ExpandWidth(true)))
            {
                FoldAllLocalCheckData(checkLocalDataList);
            }
            GUI.color = preColor;
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制指定索引的局部检查器数据
        /// </summary>
        /// <param name="checkLocalDataList"></param>
        /// <param name="index"></param>
        /// <param name="checkType"></param>
        private void DrawOneLocalChecksByIndex(List<CheckLocalData> checkLocalDataList, int index, Type checkType)
        {
            var checkLocalData = checkLocalDataList[index];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            checkLocalData.IsUnFold = EditorGUILayout.Foldout(checkLocalData.IsUnFold, checkLocalData.FolderPath, true);
            DrawCheckLocalDataIcons(checkLocalData);
            var preColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                checkLocalDataList.RemoveAt(index);
            }
            GUI.color = preColor;
            EditorGUILayout.EndHorizontal();
            if (checkLocalData.IsUnFold)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目录路径:", GUILayout.Width(100f));
                EditorGUILayout.LabelField(checkLocalData.FolderPath, AssetPipelineStyles.ButtonLeftStyle, GUILayout.ExpandWidth(true));
                var preColor2 = GUI.color;
                GUI.color = Color.green;
                if (GUILayout.Button("选择目录路径", GUILayout.Width(150.0f)))
                {
                    var newFolderPath = EditorUtilities.ChoosenProjectFolder(checkLocalData.FolderPath);
                    if (!newFolderPath.Equals(checkLocalData.FolderPath))
                    {
                        if (!CheckFolderPathExist(checkLocalDataList, newFolderPath))
                        {
                            checkLocalData.FolderPath = newFolderPath;
                            checkLocalDataList.Sort(SortLocalCheckData);
                        }
                        else
                        {
                            Debug.LogError($"局部目录:{newFolderPath}配置已存在，请勿设置重复局部目录!!");
                        }
                    }
                }
                GUI.color = preColor2;
                EditorGUILayout.EndHorizontal();
                DrawCheckLocalDatasArea(checkLocalData, checkType);
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制本地检查器数据所有Icon
        /// </summary>
        /// <param name="checkLocalData"></param>
        private void DrawCheckLocalDataIcons(CheckLocalData checkLocalData)
        {
            for (int i = 0; i < checkLocalData.CheckIconList.Count; i++)
            {
                EditorGUILayout.LabelField(checkLocalData.CheckIconList[i], GUILayout.Width(20f));
            }
        }

        /// <summary>
        /// 绘制指定检查器列表
        /// </summary>
        /// <param name="checkGlobalData"></param>
        /// <param name="checkType"></param>
        private void DrawChecksArea(CheckGlobalData checkGlobalData, Type checkType)
        {
            EditorGUILayout.BeginVertical("box");
            var checkList = checkGlobalData.CheckList;
            var chosenList = checkGlobalData.CheckChosenList;
            DrawCheckTitleArea();
            for (int i = 0; i < checkList.Count; i++)
            {
                DrawOneCheckByIndex(checkList, i, checkType);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("检查器:", GUILayout.Width(100f));
            chosenList[0] = (BaseCheck)EditorGUILayout.ObjectField(chosenList[0], AssetPipelineConst.BASE_CHECK_TYPE, false, GUILayout.ExpandWidth(true));
            var preColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("+", GUILayout.Width(100f)))
            {
                if (chosenList[0] == null)
                {
                    Debug.LogError($"不允许添加空检查器,添加检查器失败!");
                }
                else
                {
                    var chosenType = chosenList[0].GetType();
                    var findCheck = checkList.Find(check => check != null && check.GetType() == chosenType);
                    if (findCheck != null)
                    {
                        Debug.LogError($"不允许添加重复的检查器类型:{findCheck.GetType().Name},检查器名;{findCheck.Name},添加检查器失败!");
                    }
                    else
                    {
                        checkList.Add(chosenList[0]);
                        checkList.Sort(AssetPipelineUtilities.SortCheck);
                        Debug.Log($"添加检查器;{chosenList[0].Name}成功!");
                    }
                }
            }
            GUI.color = preColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制指定检查器本地数据列表
        /// </summary>
        /// <param name="checkLocalData"></param>
        /// <param name="checkType"></param>
        private void DrawCheckLocalDatasArea(CheckLocalData checkLocalData, Type checkType)
        {
            EditorGUILayout.BeginVertical("box");
            var chosenList = checkLocalData.CheckChosenList;
            DrawCheckTitleArea(false);
            for (int i = 0; i < checkLocalData.CheckDataList.Count; i++)
            {
                DrawOneCheckLocalDataByIndex(checkLocalData, i, checkType);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("检查器:", GUILayout.Width(100f));
            chosenList[0] = (BaseCheck)EditorGUILayout.ObjectField(chosenList[0], AssetPipelineConst.BASE_CHECK_TYPE, false, GUILayout.ExpandWidth(true));
            var preColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("+", GUILayout.Width(100f)))
            {
                if (chosenList[0] == null)
                {
                    Debug.LogError($"不允许添加空检查器,添加检查器失败!");
                }
                else
                {
                    var chosenType = chosenList[0].GetType();
                    var findCheckData = checkLocalData.CheckDataList.Find(checkData => checkData.Check != null && checkData.Check.GetType() == chosenType);
                    if (findCheckData != null)
                    {
                        Debug.LogError($"不允许添加重复的检查器类型:{findCheckData.Check.TypeName},检查器名;{findCheckData.Check.Name},添加检查器失败!");
                    }
                    else
                    {
                        if(checkLocalData.AddCheckData(chosenList[0]))
                        {
                            Debug.Log($"添加检查器;{chosenList[0].Name}成功!");
                        }
                    }
                }
            }
            GUI.color = preColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制检查器标题区域
        /// </summary>
        /// <param name="isGlobalProcessor">是否是全局检查器</param>
        private void DrawCheckTitleArea(bool isGlobalCheck = true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("索引", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("检查器名", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("目标Asset类型", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("管线处理类型", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("检查器Asset", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField("自定义描述", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            if (!isGlobalCheck)
            {
                EditorGUILayout.LabelField("黑名单目录", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(150));
            }
            EditorGUILayout.LabelField("操作", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定索引的单个检查器
        /// </summary>
        /// <param name="checkList"></param>
        /// <param name="index"></param>
        /// <param name="checkType"></param>
        private void DrawOneCheckByIndex(List<BaseCheck> checkList, int index, Type checkType)
        {
            var check = checkList[index];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index.ToString(), AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField(check != null ? check.Name : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField(check != null ? check.TargetAssetType.ToString() : "无", AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField(check != null ? check.TargetAssetProcessType.ToString() : "无", AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(150f));
            EditorGUILayout.ObjectField(check, checkType, false, GUILayout.Width(250f));
            EditorGUILayout.LabelField(check.CustomDes, AssetPipelineStyles.ButtonMidStyle, GUILayout.ExpandWidth(true));
            var preColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                checkList.RemoveAt(index);
            }
            GUI.color = preColor;
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定索引的单个检查数据
        /// </summary>
        /// <param name="checkLocalData"></param>
        /// <param name="index"></param>
        /// <param name="checkType"></param>
        private void DrawOneCheckLocalDataByIndex(CheckLocalData checkLocalData, int index, Type checkType)
        {
            var checkData = checkLocalData.CheckDataList[index];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index.ToString(), AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField(checkData.Check != null ? checkData.Check.Name : "无", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(250f));
            EditorGUILayout.LabelField(checkData.Check != null ? checkData.Check.TargetAssetType.ToString() : "无", AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField(checkData.Check != null ? checkData.Check.TargetAssetProcessType.ToString() : "无", AssetPipelineStyles.ButtonMidStyle, GUILayout.Width(150f));
            EditorGUILayout.ObjectField(checkData.Check, checkType, false, GUILayout.Width(250f));
            EditorGUILayout.LabelField(checkData.Check != null ? checkData.Check.CustomDes : "无", AssetPipelineStyles.ButtonMidStyle, GUILayout.ExpandWidth(true));
            var preColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button($"数量({checkData.BlackListFolderPathList.Count})", GUILayout.Width(150f)))
            {
                LocalDetailWindow.ShowCheckDetailWindow(checkLocalData.FolderPath, checkData, checkType);
            }
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                checkLocalData.RemoveCheckDataByIndex(index);
            }
            GUI.color = preColor;
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 折叠所有本地检查器数据配置
        /// </summary>
        /// <param name="checkLocalDataList"></param>
        private void FoldAllLocalCheckData(List<CheckLocalData> checkLocalDataList)
        {
            foreach (var checkLocalData in checkLocalDataList)
            {
                checkLocalData.IsUnFold = false;
            }
        }

        /// <summary>
        /// 本地检查器数据排序
        /// </summary>
        /// <param name="localData1"></param>
        /// <param name="localData2"></param>
        /// <returns></returns>
        private int SortLocalCheckData(CheckLocalData localData1, CheckLocalData localData2)
        {
            return localData1.FolderPath.CompareTo(localData2.FolderPath);
        }

        /// <summary>
        /// 检查指定局部目录配置是否已存在
        /// </summary>
        /// <param name="checkLocalDataList"></param>
        /// <param name="newFolderPath"></param>
        /// <returns></returns>
        private bool CheckFolderPathExist(List<CheckLocalData> checkLocalDataList, string newFolderPath)
        {
            if (checkLocalDataList == null)
            {
                return false;
            }
            foreach (var checkLocalData in checkLocalDataList)
            {
                if (checkLocalData.FolderPath.Equals(newFolderPath))
                {
                    return true;
                }
            }
            return false;
        }
    }
}