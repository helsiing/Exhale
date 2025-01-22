using BrunoMikoski.ScriptableObjectCollections;
using Unity.Entities;
using UnityEngine;

namespace Exhale.Scripts.Data
{
    public struct PieceTemplateCollectionBlob
    {
        public BlobArray<PieceTemplate> Templates;
    }

    [CreateAssetMenu(menuName = "Exhale/Data/HexPieceTemplateCollection", fileName = "HexPieceTemplateCollection",
        order = 0)]
    public class HexPieceTemplateCollection : ScriptableObjectCollection<HexPieceTemplate>
    {
    }
}