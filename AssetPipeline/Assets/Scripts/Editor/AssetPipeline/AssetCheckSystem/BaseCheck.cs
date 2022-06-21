/*
 * Description:             BaseCheck.cs
 * Author:                  TONYTANG
 * Create Date:             2022/06/17
 */

using System;
using System.Collections;
using System.Collections.Generic;
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
    }
}