/*
 * Description:             CheckGlobalData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/10/19
 */

using System;
using System.Collections.Generic;
using UnityEngine;

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

        /// <summary>
        /// 检查是否有无效检查器配置
        /// </summary>
        /// <returns></returns>
        public bool CheckInvalideCheckConfig()
        {
            // 删除检查器Asset会导致引用丢失，配置检查器Asset找不到的情况
            foreach (var check in CheckList)
            {
                if (check == null)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
