﻿/*
 * Description:             AssetPipelineGUIContent.cs
 * Author:                  TONYTANG
 * Create Date:             2022/07/07
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TAssetPipeline
{
    /// <summary>
    /// AssetPipelineGUIContent.cs
    /// Asset管线的GUIContent
    /// </summary>
    public static class AssetPipelineGUIContent
    {
        /// <summary>
        /// Favorite Icon
        /// </summary>
        public static GUIContent FavoriteIcon = EditorGUIUtility.IconContent("Favorite Icon");

        /// <summary>
        /// 纹理Icon
        /// </summary>
        public static GUIContent Texture2DIcon = EditorGUIUtility.IconContent("Texture2D Icon");

        /// <summary>
        /// 材质Icon
        /// </summary>
        public static GUIContent MaterialIcon = EditorGUIUtility.IconContent("Material Icon");

        /// <summary>
        /// SpriteAtlas Icon
        /// </summary>
        public static GUIContent SpriteAtlasIcon = EditorGUIUtility.IconContent("SpriteAtlas Icon");

        /// <summary>
        /// 模型Icon
        /// </summary>
        public static GUIContent FBXIcon = EditorGUIUtility.IconContent("PrefabModel Icon");

        /// <summary>
        /// 音效Icon
        /// </summary>
        public static GUIContent AudioClipIcon = EditorGUIUtility.IconContent("AudioClip Icon");

        /// <summary>
        /// 字体Icon
        /// </summary>
        public static GUIContent FontIcon = EditorGUIUtility.IconContent("Font Icon");

        /// <summary>
        /// Shader Icon
        /// </summary>
        public static GUIContent ShaderIcon = EditorGUIUtility.IconContent("Shader Icon");

        /// <summary>
        /// 预制件Icon
        /// </summary>
        public static GUIContent PrefabIcon = EditorGUIUtility.IconContent("Prefab Icon");

        /// <summary>
        /// ScriptableObject Icon
        /// </summary>
        public static GUIContent ScriptableObjectIocn = EditorGUIUtility.IconContent("ScriptableObject Icon");

        /// <summary>
        /// 文本Asset Icon
        /// </summary>
        public static GUIContent TextAssetIcon = EditorGUIUtility.IconContent("TextAsset Icon");

        /// <summary>
        /// 场景Icon
        /// </summary>
        public static GUIContent SceneIcon = EditorGUIUtility.IconContent("SceneAsset Icon");

        /// <summary>
        /// 动画Clip Icon
        /// </summary>
        public static GUIContent AnimationClipIcon = EditorGUIUtility.IconContent("AnimationClip Icon");

        /// <summary>
        /// 脚本Icon
        /// </summary>
        public static GUIContent ScriptIcon = EditorGUIUtility.IconContent("cs Script Icon");

        /// <summary>
        /// 帮助Icon
        /// </summary>
        public static GUIContent HelpIcon = EditorGUIUtility.IconContent("_Help@2x");

        /// <summary>
        /// 动画控制器Icon
        /// </summary>
        public static GUIContent AnimatorControllerIcon = EditorGUIUtility.IconContent("AnimatorController Icon");

        /// <summary>
        /// 网格Icon
        /// </summary>
        public static GUIContent MeshIcon = EditorGUIUtility.IconContent("Mesh Icon");
    }
}