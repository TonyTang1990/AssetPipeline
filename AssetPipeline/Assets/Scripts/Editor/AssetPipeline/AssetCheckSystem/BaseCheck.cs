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
        /// 目标Asset管线处理类型(子类重定义指定)
        /// </summary>
        public abstract AssetProcessType TargetAssetProcessType
        {
            get;
        }

        /// <summary>
        /// 处理器触发排序Order(值越大越靠后)
        /// </summary>
        public virtual int Order
        {
            get
            {
                return 100;
            }
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
        public BaseCheck()
        {

        }

        /// <summary>
        /// 是否是目标处理Asset类型
        /// </summary>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public bool IsValideAssetType(AssetType assetType)
        {
            return (TargetAssetType & assetType) != AssetType.None;
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
        /// 是否是目标处理Asset管线处理类型
        /// </summary>
        /// <param name="assetProcessType"></param>
        /// <returns></returns>
        public bool IsValideAssetProcessType(AssetProcessType assetProcessType)
        {
            return TargetAssetProcessType == assetProcessType;
        }

        /// <summary>
        /// 执行检查器检查
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表</param>
        public bool ExecuteCheck(AssetPostprocessor assetPostProcessor, params object[] paramList)
        {
            if (!IsValideAssetByPath(assetPostProcessor.assetPath))
            {
                return false;
            }
            AssetPipelineLog.Log($"#Asset:{assetPostProcessor.assetPath}执行检查器:{Name},检查器描述:{CustomDes}!".WithColor(Color.green));
            return DoCheck(assetPostProcessor, paramList);
        }

        /// <summary>
        /// 执行指定Asset路径检查器检查
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表</param>
        public bool ExecuteCheckByPath(string assetPath, params object[] paramList)
        {
            if (!IsValideAssetByPath(assetPath))
            {
                return false;
            }
            AssetPipelineLog.Log($"@Asset:{assetPath}执行检查器:{Name},检查器描述:{CustomDes}!".WithColor(Color.green));
            return DoCheckByPath(assetPath, paramList);
        }

        /// <summary>
        /// 执行检查器处理(子类重写实现自定义检查器)
        /// </summary>
        /// <param name="assetPostProcessor"></param>
        /// <param name="paramList">不定长参数列表(未来用于支持Unity更多的AssetPostprocessor接口传参)</param>
        protected abstract bool DoCheck(AssetPostprocessor assetPostProcessor, params object[] paramList);

        /// <summary>
        /// 执行指定路径的检查器处理(子类重写实现自定义检查器)
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="paramList">不定长参数列表(未来用于支持Unity更多的AssetPostprocessor接口传参)</param>
        protected abstract bool DoCheckByPath(string assetPath, params object[] paramList);
    }
}