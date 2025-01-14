using Exhale.Scripts.Utils;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class BoardPresentation : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private Transform gridRoot;
        
        private int totalRows;
        private int totalColumns;
        
        public void InitBoard(HexTileData[,] tiles)
        {
            gridRoot.gameObject.DestroyChildObjects();
            totalRows = tiles.GetLength(0);
            totalColumns = tiles.GetLength(1);
        }

        public GameObject GetTileGameObject(HexTileData hexTileData)
        {
            if (hexTileData != null)
            {
                GameObject tileGameObject = Instantiate(emptyTilePrefab);
                if (tileGameObject != null)
                {
                    SetObjectInBoard(hexTileData.PositionIndex, tileGameObject, "[HexTile]");
                    return tileGameObject;
                }
            }

            return null;
        }

        public GameObject GetPieceGameObject(HexPieceData hexPieceData)
        {
            if (hexPieceData != null)
            {
                GameObject pieceGameObject = HexPieceFactory.GetPiece(hexPieceData.PieceTemplate);
                if (pieceGameObject != null)
                {
                    SetObjectInBoard(hexPieceData.PositionIndex, pieceGameObject, "[HexPiece]");
                    return pieceGameObject;
                }
            }

            return null;
        }

        public void ShowBoard(IHexPiece[,] pieces)
        {
            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < totalColumns; col++)
                {
                    if (pieces[row, col] != null)
                    {
                        pieces[row, col].Show();
                    }
                }
            }
        }
        
        private void SetObjectInBoard(Vector2 positionIndex, GameObject boardObject, string prefix = "")
        {
            boardObject.transform.SetParent(gridRoot);
            boardObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(positionIndex, totalRows, totalColumns);
            boardObject.name = $"{prefix} [{positionIndex.x}, {positionIndex.y}]";
        }
    }
}