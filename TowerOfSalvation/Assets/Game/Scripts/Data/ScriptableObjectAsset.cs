#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Extentions
{
    public static class ScriptableObjectAsset
    {
        /// <summary>
        /// This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateAsset<T>() where T : ScriptableObject
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
            {
                path = "Assets/Resources/";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(path), "") + "/";
            }

            if (!path.EndsWith("\\") && !path.EndsWith("/"))
            {
                path += "/";
            }

            var asset = ScriptableObject.CreateInstance<T>();
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + typeof(T).Name + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
            return asset;
        }
    }
}
#endif