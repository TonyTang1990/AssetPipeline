/*
 * Description:             AssetPipelinePanel.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/18
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TAssetPipeline.AssetPipelineSettingData;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetPipelinePanel.cs
    /// Asset管线面板
    /// </summary>
    public class AssetPipelinePanel : BaseEditorPanel
    {
        /// <summary>
        /// Asset管线系统UI滚动位置
        /// </summary>
        private Vector2 mAssetPipelineScrollPos;

        /// <summary>
        /// 当前配置策略
        /// </summary>
        public string CurrentConfigStrategy
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前配置策略索引
        /// </summary>
        private int mCurrentConfigStrategyIndex;

        /// <summary>
        /// Asset管线设置数据
        /// </summary>
        private AssetPipelineSettingData mSettingData;

        /// <summary>
        /// 添加的策略名
        /// </summary>
        private string mStrategyAdded;

        /// <summary>
        /// 所有的平台策略名数组
        /// </summary>
        private string[] mAllStrategyNames;

        /// <summary>
        /// 平台策略选择索引Map<平台, 策略索引>
        /// </summary>
        private Dictionary<BuildTarget, int> mPlatformStrategySelectedIndexMap;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AssetPipelinePanel() : base()
        {

        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="ownerEditorWindow"></param>
        /// <param name="panelName"></param>
        public AssetPipelinePanel(BaseEditorWindow ownerEditorWindow, string panelName) : base(ownerEditorWindow, panelName)
        {

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void LoadAllData()
        {
            base.LoadAllData();
            InitAssetPipelineData();
            LoadAssetPipelinePrefDatas();
        }

        /// <summary>
        /// 更新策略名
        /// </summary>
        /// <param name="strategyName"></param>
        private void UpdateConfigStrategy(string strategyName)
        {
            Debug.Log($"更新配置策略从:{CurrentConfigStrategy}到:{strategyName}");
            CurrentConfigStrategy = strategyName;
            mCurrentConfigStrategyIndex = Array.IndexOf<string>(mAllStrategyNames, CurrentConfigStrategy);
            SaveAssetConfigStrategyPrefData();
            GetOwnerEditorWindow<AssetPipelineWindow>().RefreshAllPanels();
        }

        /// <summary>
        /// 加载Asset管线本地数据
        /// </summary>
        private void LoadAssetPipelinePrefDatas()
        {
            CurrentConfigStrategy = EditorPrefs.GetString(AssetPipelinePrefKey.CURRENT_CONFIG_STRATEGY, AssetPipelineConst.DEFAULT_STRATEGY_NAME);
            mCurrentConfigStrategyIndex = Array.IndexOf<string>(mAllStrategyNames, CurrentConfigStrategy);
            if (mCurrentConfigStrategyIndex == -1)
            {
                UpdateConfigStrategy(AssetPipelineConst.DEFAULT_STRATEGY_NAME);
            }
        }

        /// <summary>
        /// 初始化Asset管线数据
        /// </summary>
        private void InitAssetPipelineData()
        {
            mSettingData = AssetPipelineSystem.LoadSettingData();
            UpdateStrategyNames();
            InitPlatformStrategySelectedInfo();
        }

        /// <summary>
        /// 初始化平台策略选择信息
        /// </summary>
        private void InitPlatformStrategySelectedInfo()
        {
            mPlatformStrategySelectedIndexMap = new Dictionary<BuildTarget, int>();
            foreach (var platformStrategyData in mSettingData.PlatformStrategyDataList)
            {
                var strategyIndex = mSettingData.StrategyList.IndexOf(platformStrategyData.StrategyName);
                if(strategyIndex == -1)
                {
                    Debug.LogError($"平台:{platformStrategyData.Target}配置的策略:{platformStrategyData.StrategyName}不存在了,恢复到默认设置策略:{AssetPipelineConst.DEFAULT_STRATEGY_NAME}!");
                    platformStrategyData.StrategyName = AssetPipelineConst.DEFAULT_STRATEGY_NAME;
                    strategyIndex = mSettingData.StrategyList.IndexOf(platformStrategyData.StrategyName);
                }
                if(strategyIndex != -1)
                {
                    mPlatformStrategySelectedIndexMap.Add(platformStrategyData.Target, strategyIndex);
                    Debug.Log($"初始化平台:{platformStrategyData.Target}策略选择索引:{strategyIndex}");
                }
            }
        }

        /// <summary>
        /// 更新平台策略名
        /// </summary>
        private void UpdateStrategyNames()
        {
            mAllStrategyNames = mSettingData.StrategyList.ToArray();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public override void SaveAllData()
        {
            base.SaveAllData();
            SaveAssetPipelineData();
            SaveAssetPipelinePrefDatas();
            Debug.Log($"保存Asset管线数据完成!");
        }

        /// <summary>
        /// 保存当前配置策略本地数据
        /// </summary>
        private void SaveAssetConfigStrategyPrefData()
        {
            EditorPrefs.SetString(AssetPipelinePrefKey.CURRENT_CONFIG_STRATEGY, CurrentConfigStrategy);
            Debug.Log($"保存当前配置策略:{CurrentConfigStrategy}");
        }

        /// <summary>
        /// 保存Asset管线本地数据
        /// </summary>
        private void SaveAssetPipelinePrefDatas()
        {
            SaveAssetConfigStrategyPrefData();
        }

        /// <summary>
        /// 保存Asset管线数据
        /// </summary>
        private void SaveAssetPipelineData()
        {
            EditorUtility.SetDirty(mSettingData);
            AssetDatabase.SaveAssetIfDirty(mSettingData);
        }

        /// <summary>
        /// 响应绘制
        /// </summary>
        public override void OnGUI()
        {
            if(mSettingData != null)
            {
                DrawPlatformInfoArea();
                mAssetPipelineScrollPos = EditorGUILayout.BeginScrollView(mAssetPipelineScrollPos);
                DrawAssetPipelineContentArea();
                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// 绘制平台信息区域
        /// </summary>
        private void DrawPlatformInfoArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前激活平台", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("当前配置策略", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            var preColor = GUI.color;
            GUI.color = Color.yellow;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(EditorUserBuildSettings.activeBuildTarget.ToString(), AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(CurrentConfigStrategy, AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            GUI.color = preColor;
        }

        /// <summary>
        /// 绘制Asset管线内容区域
        /// </summary>
        private void DrawAssetPipelineContentArea()
        {
            DrawConfigStrategyArea();
            DrawResourceFolderArea();
            EditorGUILayout.Space();
            DrawStrategyArea();
            EditorGUILayout.Space();
            DrawPlatformStrategyArea();
            DrawOtherArea();
        }

        /// <summary>
        /// 绘制策略配置区域
        /// </summary>
        private void DrawConfigStrategyArea()
        {
            GUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("切换配置策略:", GUILayout.Width(100.0f));
            EditorGUI.BeginChangeCheck();
            mCurrentConfigStrategyIndex = EditorGUILayout.Popup(mCurrentConfigStrategyIndex, mAllStrategyNames, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                UpdateConfigStrategy(mAllStrategyNames[mCurrentConfigStrategyIndex]);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制资源目录区域
        /// </summary>
        private void DrawResourceFolderArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("资源目录:", GUILayout.Width(100.0f));
            EditorGUILayout.TextField(mSettingData.ResourceFolder, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("选择资源目录", GUILayout.Width(150.0f)))
            {
                mSettingData.ResourceFolder = EditorUtilities.ChoosenProjectFolder(mSettingData.ResourceFolder);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制策略区域
        /// </summary>
        private void DrawStrategyArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Asset管线策略", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            DrawStrategyTitleArea();
            for(int i = 0; i < mSettingData.StrategyList.Count; i++)
            {
                DrawOneStartegyByIndex(mSettingData.StrategyList, i);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("新增策略名:", GUILayout.Width(100f));
            mStrategyAdded = EditorGUILayout.TextField(mStrategyAdded, GUILayout.ExpandWidth(true));
            if(GUILayout.Button("+", GUILayout.Width(100f)))
            {
                AddStrategy(mStrategyAdded);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制平台策略区域
        /// </summary>
        private void DrawPlatformStrategyArea()
        {
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("平台策略配置", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            DrawPlatformStrategyTitleArea();
            for (int i = 0, length = mSettingData.PlatformStrategyDataList.Count; i < length; i++)
            {
                DrawOnePlatformStartegy(mSettingData.PlatformStrategyDataList[i]);
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制其他UI区域
        /// </summary>
        private void DrawOtherArea()
        {
            var preColor = GUI.color;
            GUI.color = Color.green;
            if (GUILayout.Button("保存Asset管线数据", GUILayout.ExpandWidth(true)))
            {
                GetOwnerEditorWindow<AssetPipelineWindow>().SaveAllData();
            }
            GUI.color = preColor;
        }

        /// <summary>
        /// 绘制策略标题区域
        /// </summary>
        private void DrawStrategyTitleArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("索引", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("策略名", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("操作", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定索引的策略名
        /// </summary>
        /// <param name="strategyList"></param>
        /// <param name="index"></param>
        private void DrawOneStartegyByIndex(List<string> strategyList, int index)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index.ToString(), AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField(strategyList[index], AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            if (!string.Equals(strategyList[index], AssetPipelineConst.DEFAULT_STRATEGY_NAME))
            {
                if (GUILayout.Button("-", GUILayout.Width(100f)))
                {
                    RemoveStrategyByIndex(index);
                }
            }
            else
            {
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(100f));
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制平台策略配置标题区域
        /// </summary>
        private void DrawPlatformStrategyTitleArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("平台名", AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(200f));
            EditorGUILayout.LabelField("策略名", AssetPipelineStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制一个平台策略数据
        /// </summary>
        private void DrawOnePlatformStartegy(PlatformStrategyData platformStrategyData)
        {
            if(platformStrategyData != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(platformStrategyData.Target.ToString(), AssetPipelineStyles.TabMiddleStyle, GUILayout.Width(200f));
                EditorGUI.BeginChangeCheck();
                mPlatformStrategySelectedIndexMap[platformStrategyData.Target] = EditorGUILayout.Popup(mPlatformStrategySelectedIndexMap[platformStrategyData.Target], mAllStrategyNames, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    var newStrategyName = mAllStrategyNames[mPlatformStrategySelectedIndexMap[platformStrategyData.Target]];
                    if (EditorUtility.DisplayDialog("平台策略切换", $"确认切换平台:{platformStrategyData.Target}的打包策略从:{platformStrategyData.StrategyName}到{newStrategyName}吗？切换后对应平台需要重新导入触发对应策略方案!", "确认", "取消"))
                    {
                        Debug.Log($"更新平台:{platformStrategyData.Target}的打包策略从:{platformStrategyData.StrategyName}到{newStrategyName}".WithColor(Color.yellow));
                        platformStrategyData.StrategyName = newStrategyName;
                        if (EditorUserBuildSettings.activeBuildTarget == platformStrategyData.Target)
                        {
                            Debug.Log($"当前切换目标平台:{platformStrategyData.Target}和当前激活平台一致,强制保存重新加载最新配置数据!".WithColor(Color.yellow));
                            GetOwnerEditorWindow<AssetPipelineWindow>().SaveAllData();
                        }
                    }
                    else
                    {
                        var preIndex = Array.IndexOf<string>(mAllStrategyNames, platformStrategyData.StrategyName);
                        mPlatformStrategySelectedIndexMap[platformStrategyData.Target] = preIndex;
                        Debug.Log($"取消更新平台:{platformStrategyData.Target}的打包策略");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 添加指定策略
        /// </summary>
        /// <param name="strategyName"></param>
        private bool AddStrategy(string strategyName)
        {
            if(mSettingData == null)
            {
                Debug.LogError($"未加载有效配置数据,添加策略失败!");
                return false;
            }
            else if (string.IsNullOrEmpty(mStrategyAdded))
            {
                Debug.LogError($"不允许添加空策略名!");
                return false;
            }
            else if (mSettingData.StrategyList.Contains(mStrategyAdded))
            {
                Debug.LogError($"不允许添加重复的策略名:{mStrategyAdded}");
                return false;
            }
            mSettingData.StrategyList.Add(mStrategyAdded);
            UpdateStrategyNames();
            Debug.Log($"添加策略名:{mStrategyAdded}");
            return true;
        }

        /// <summary>
        /// 移除指定索引策略配置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool RemoveStrategyByIndex(int index)
        {
            if(mSettingData != null)
            {
                if (index >= 0 && index < mSettingData.StrategyList.Count)
                {
                    mSettingData.StrategyList.RemoveAt(index);
                    UpdateStrategyNames();
                    Debug.Log($"移除指定索引:{index}的策略名配置!");
                    return true;
                }
                Debug.LogError($"移除指定索引:{index}超出有效索引范围:{0}-{mSettingData.StrategyList.Count},移除失败!");
                return false;
            }
            else
            {
                Debug.LogError($"未加载有效配置数据,移除指定索引:{index}的策略名配置失败!");
                return false;
            }
        }
    }
}