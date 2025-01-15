using System.Collections.Generic;
using Exhale.Scripts.Data;
using Exhale.Utils;
using UnityEngine;

namespace Exhale.Gameplay
{
    public class BoardPresentation : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private Transform tilesRoot;
        [SerializeField] private Transform piecesRoot;

        private Dictionary<Vector2, GameObject> tiles;
        private Dictionary<Vector2, GameObject> pieces;
        
        private IBoard board;

        public void Init(IBoard board)
        {
            this.board = board;
            tilesRoot.gameObject.DestroyChildObjects();
            tiles = new Dictionary<Vector2, GameObject>();
            piecesRoot.gameObject.DestroyChildObjects();
            pieces = new Dictionary<Vector2, GameObject>();
        }

        public GameObject SetTileGameObject(HexTileData hexTileData)
        {
            if (hexTileData == null) return null;
            GameObject tileGameObject = Instantiate(emptyTilePrefab, tilesRoot, true);
            if (tileGameObject == null) return null;
            SetObjectInBoard(hexTileData.PositionIndex, tileGameObject, "[HexTile]");
            
            tiles.Add(hexTileData.PositionIndex, tileGameObject);
            
            return tileGameObject;

        }

        public IHexPiece SetPieceGameObject(HexPieceData hexPieceData)
        {
            if (hexPieceData == null) return null;
            
            GameObject pieceGameObject = HexPieceFactory.GetPiece(hexPieceData.PieceTemplate);
                
            if (pieceGameObject == null || !pieceGameObject.TryGetComponent<IHexPiece>(out var hexPiece))
                return null;
            
            hexPiece.Init(hexPieceData);
            SetObjectInBoard(hexPieceData.PositionIndex, pieceGameObject, "[HexPiece]");
            pieceGameObject.transform.SetParent(piecesRoot);
            pieces.Add(hexPieceData.PositionIndex, pieceGameObject);
            
            if (!hexPiece.PieceTemplate.TryGetTrait(out Building building)) return hexPiece;
                    
            foreach (BuildingUnlockRequirementsData unlockRequirementsData in building.UnlockRequirementsData)
            {
                GameObject unlockRequirementGameObject =
                    HexPieceFactory.GetPiece(unlockRequirementsData.PieceTemplate, false);
                
                Vector2 unlockRequirementPositionIndex = hexPieceData.PositionIndex + unlockRequirementsData.PositionIndex;
                SetObjectInBoard(unlockRequirementPositionIndex, unlockRequirementGameObject, "[HexBuildingUnlockRequirement]");
            }

            return hexPiece;
        }
        
        private void SetObjectInBoard(Vector2 positionIndex, GameObject boardObject, string prefix = "")
        {
            boardObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(positionIndex, board.Width, board.Height);
            boardObject.name = $"{prefix} [{positionIndex.x}, {positionIndex.y}]";
        }
    }
}