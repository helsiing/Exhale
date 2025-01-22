using Exhale.Scripts.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Exhale.ECS.Authoring
{
    public class PieceTemplatesAuthoring : MonoBehaviour
    {
        [SerializeField] private HexPieceTemplateCollection pieceTemplateCollection;

        public class Baker : Baker<PieceTemplatesAuthoring>
        {
            public override void Bake(PieceTemplatesAuthoring authoring)
            {
                if (authoring.pieceTemplateCollection == null || authoring.pieceTemplateCollection.Count == 0)
                {
                    Debug.LogWarning("HexPrefabCollection is missing or empty.");
                    return;
                }

                using var blobBuilder = new BlobBuilder(Allocator.Temp);
                ref var pieceTemplateCollectionBlob =
                    ref blobBuilder.ConstructRoot<PieceTemplateCollectionBlob>();

                var templatesArray = blobBuilder.Allocate(ref pieceTemplateCollectionBlob.Templates,
                    authoring.pieceTemplateCollection.Count);

                for (var i = 0; i < authoring.pieceTemplateCollection.Items.Count; i++)
                {
                    var hexPieceData = (HexPieceTemplate)authoring.pieceTemplateCollection.Items[i];
                    if (hexPieceData.PiecePrefab == null)
                        continue;

                    var piecePrefab = GetEntity(hexPieceData.PiecePrefab, TransformUsageFlags.Dynamic);
                    templatesArray[i] = new PieceTemplate
                    {
                        PieceId = authoring.pieceTemplateCollection[i].GUID.GetHashCode(),
                        PiecePrefabEntity = piecePrefab
                    };
                    Debug.Log($"[ECS] Loaded piece: {hexPieceData.name}");
                }

                var blobReference =
                    blobBuilder.CreateBlobAssetReference<PieceTemplateCollectionBlob>(Allocator.Persistent);

                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PieceTemplateCollectionComponent
                {
                    Blob = blobReference
                });
            }
        }
    }

    // ECS component to store the BlobAssetReference
    public struct PieceTemplateCollectionComponent : IComponentData
    {
        public BlobAssetReference<PieceTemplateCollectionBlob> Blob;
    }
}