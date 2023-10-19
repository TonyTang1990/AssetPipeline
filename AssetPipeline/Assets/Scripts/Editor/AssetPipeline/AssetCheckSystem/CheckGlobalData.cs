﻿/*
 * Description:             CheckGlobalData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAssetPipeline
{
    /// <summary>
    /// CheckGlobalData.cs
    /// Asset检查器全局数据
    /// </summary>
    [Serializable]
    public class CheckGlobalData
    {
        /// <summary>
        /// 检查器设置列表
        /// </summary>
        public List<BaseCheck> CheckList = new List<BaseCheck>();

        /// <summary>
        /// 检查器Asset路径列表(保存时刷新导出，Asset管线运行时用，和CheckList一一对应)
        /// </summary>
        [Header("检查器Asset路径列表")]
        public List<string> CheckAssetPathList = new List<string>();

        /// <summary>
        /// 检查器选择列表(只使用第一个)
        /// </summary>
        [NonSerialized]
        public List<BaseCheck> CheckChosenList = new List<BaseCheck>(1) { null };
    }

}
