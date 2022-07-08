/*
 * Description:             BaseCheck.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace TAssetPipeline
{
    /// <summary>
    /// BaseCheck.cs
    /// Asset检查抽象类
    /// </summary>
    public abstract class BaseCheck : ScriptableObject
    {
        /// <summary>
        /// 检查器名(子类重写定义)
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// 自定义描述(方便同类型添加自定义介绍)
        /// </summary>
        [Header("自定义描述")]
        public string CustomDes;

        /// <summary>
        /// 目标Asset类型(子类重定义指定)
        /// </summary>
        public abstract AssetType TargetAssetType
        {
            get;
        }

        /// <summary>
        /// 检查器类型名
        /// </summary>
        public string TypeName
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Asset路径
        /// </summary>
        public string AssetPath
        {
            get
            {
                if (string.IsNullOrEmpty(mAssetPath))
                {
                    mAssetPath = AssetDatabase.GetAssetPath(this);
                }
                return mAssetPath;
            }
        }
        private string mAssetPath;

        /// <summary>
        /// 是否是目标处理Asset类型
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        protected bool IsValideAssetType(AssetType assetType)
        {
            if (TargetAssetType == AssetType.Object && assetType != AssetType.None)
            {
                return true;
            }
            return TargetAssetType == assetType;
        }

        /// <summary>
        /// 指定Asset路径是否是目标处理Asset类型
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        protected bool IsValideAssetByPath(string assetPath)
        {
            var assetType = AssetPipelineSystem.GetAssetTypeByPath(assetPath);
            if (!IsValideAssetType(assetType))
            {
                Debug.LogError($"检查器:{Name}不支持的Asset类型:{assetType},Asset:{assetPath}不应该进入这里!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 执行检查器检查
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        public bool ExecuteCheck(AssetPostprocessor assetPostProcessor)
        {
            if (!IsValideAssetByPath(assetPostProcessor.assetPath))
            {
                return false;
            }
            AssetPipelineLog.Log($"#Asset:{assetPostProcessor.assetPath}执行检查器:{Name}!".WithColor(Color.green));
            return DoCheck(assetPostProcessor);
        }

        /// <summary>
        /// 执行指定Asset路径检查器检查
        /// </summary>
        /// <param name="assetPath"></param>
        public bool ExecuteCheckByPath(string assetPath)
        {
            if (!IsValideAssetByPath(assetPath))
            {
                return false;
            }
            AssetPipelineLog.Log($"@Asset:{assetPath}执行检查器:{Name}!".WithColor(Color.green));
            return DoCheckByPath(assetPath);
        }

        /// <summary>
        /// 执行检查器处理(子类重写实现自定义检查器)
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        protected abstract bool DoCheck(AssetPostprocessor assetPostProcessor);

        /// <summary>
        /// 执行指定路径的检查器处理(子类重写实现自定义检查器)
        /// </summary>
        /// <param name="assetPath"></param>
        protected abstract bool DoCheckByPath(string assetPath);
    }
}