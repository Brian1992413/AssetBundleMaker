using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder
{
    [MenuItem("Tools/Build Selected AssetBundles")]
    public static void BuildSelectedAssetBundles()
    {
        var selectedObjects = Selection.objects;

        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            Debug.LogError("No assets selected!");
            return;
        }

        string bundleDir = "Assets/AssetBundles";
        if (!Directory.Exists(bundleDir))
            Directory.CreateDirectory(bundleDir);

        foreach (var obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning($"Skipping invalid selection: {obj.name}");
                continue;
            }

            // Check if the asset is a prefab
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Selected asset '{obj.name}' is not a valid prefab. Skipping.");
                continue;
            }

            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                importer.assetBundleName = assetName.ToLower() + ".bundle";
            }
            else
            {
                Debug.LogWarning($"Failed to get AssetImporter for '{obj.name}'. Skipping.");
                continue;
            }
        }

        // Build for current active build target
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        BuildPipeline.BuildAssetBundles(bundleDir, BuildAssetBundleOptions.None, buildTarget);

        Debug.Log($"AssetBundles built successfully in '{bundleDir}'.");
    }
}
