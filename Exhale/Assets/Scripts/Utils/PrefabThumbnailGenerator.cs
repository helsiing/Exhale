using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

class PrefabThumbnailGenerator : MonoBehaviour
{
    [SerializeField] private List<Sprite> thumbnailSprites = new();

#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] private List<GameObject> targetPrefabs = new();
    [SerializeField] private string directoryPath = "Thumbnails";

    [ContextMenu("Generate Thumbnails")]
    private void GenerateThumbnails()
    {
        try
        {
            var directoryFullPath = Path.Combine(Application.dataPath, directoryPath);
            Directory.CreateDirectory(directoryFullPath);

            foreach (var prefab in targetPrefabs)
            {
                var thumbnailTexture = AssetPreview.GetAssetPreview(prefab);

                if (thumbnailTexture != null)
                {
                    var imagePath = Path.Combine("Assets", directoryPath, prefab.name + ".png");

                    var imageFullPath = Path.Combine(Application.dataPath, directoryPath, prefab.name + ".png");
                    File.WriteAllBytes(imageFullPath, thumbnailTexture.EncodeToPNG());

                    AssetDatabase.Refresh();
                    
                    var textureImporter = AssetImporter.GetAtPath(imagePath) as TextureImporter;
                    if (textureImporter != null)
                    {
                        textureImporter.textureType = TextureImporterType.Sprite;
                        EditorUtility.SetDirty(textureImporter);
                        textureImporter.SaveAndReimport();

                        var thumbnailSprite = AssetDatabase.LoadAssetAtPath<Sprite>(imagePath);
                        if (thumbnailSprite != null)
                        {
                            thumbnailSprites.Add(thumbnailSprite);
                        }
                        else
                        {
                            Debug.LogError("Failed to load a sprite asset.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to load a texture asset.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to generate a thumbnail. {ex.Message}");
        } 
    }
#endif
}