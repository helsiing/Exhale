using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.ScriptableObjectCollections;
using Exhale.ECS.Authoring;
using UnityEditor;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    [CreateAssetMenu(menuName = "Exhale/Data/HexPieceTemplateCollection", fileName = "HexPieceTemplateCollection",
        order = 0)]
    public class HexPieceTemplateCollection : ScriptableObjectCollection<HexPieceTemplate>
    {
        [MenuItem("Exhale/Tools/Pieces/Setup prefabs")]
        public static void SetupPrefabs()
        {
            IEnumerable<HexPieceTemplate> pieceTemplates = AssetDatabase.FindAssets("t:HexPieceTemplate")
                .Select(guid => AssetDatabase.LoadAssetAtPath<HexPieceTemplate>(AssetDatabase.GUIDToAssetPath(guid)));

            foreach (HexPieceTemplate hexPieceTemplate in pieceTemplates)
            {
                if (!hexPieceTemplate.TryGetTrait(out BoardObject boardObject))
                {
                    Debug.Log($"Piece {hexPieceTemplate.name} does not have a BoardObject trait");;
                    return;
                }
                
                if (boardObject.Prefab == null)
                {
                    Debug.LogError($"Piece {hexPieceTemplate.name} does not have a prefab assigned");
                }
                else
                {
                    // Create an instance of the prefab in memory to modify
                    GameObject instance = (GameObject) PrefabUtility.InstantiatePrefab(boardObject.Prefab);

                    if (!instance.TryGetComponent(out PieceAuthoring pieceAuthoring))
                    {
                        pieceAuthoring = instance.AddComponent<PieceAuthoring>();
                    }

                    pieceAuthoring.SetPieceTemplate(hexPieceTemplate);
                    // Apply changes back to the prefab
                    PrefabUtility.SaveAsPrefabAsset(instance, AssetDatabase.GetAssetPath(boardObject.Prefab));
                    DestroyImmediate(instance);
                }

                // Save all assets
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}