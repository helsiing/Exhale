using System;
using Exhale.Scripts.Data;
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
        
        public void DrawBoard(HexTileData[,] tiles, HexPieceData[,] pieces)
        {
            gridRoot.gameObject.DestroyChildObjects();
            totalRows = tiles.GetLength(0);
            totalColumns = tiles.GetLength(1);
            
            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < totalColumns; col++)
                {
                    DrawTile(tiles[row, col]);
                    DrawPiece(pieces[row, col]);
                }
            }
        }

        public HexTile DrawTile(HexTileData hexTileData)
        {
            GameObject tileGameObject = Instantiate(emptyTilePrefab);
            if (tileGameObject != null)
            {
                SetObjectInBoard(hexTileData.PositionIndex, tileGameObject, "[HexTile]");
                if (tileGameObject.TryGetComponent(out HexTile hexTile))
                {
                    hexTile.Init(hexTileData);
                    return hexTile;    
                }
            }
            return null;
        }

        public HexPiece DrawPiece(HexPieceData hexPieceData)
        {
            if (hexPieceData != null && hexPieceData.PieceTemplate.TryGetTrait(out BoardObject boardObject))
            {
                GameObject pieceGameObject = Instantiate(boardObject.Prefab);
                if (pieceGameObject != null)
                {
                    SetObjectInBoard(hexPieceData.PositionIndex, pieceGameObject, "[HexPiece]");
                    if (pieceGameObject.TryGetComponent(out HexPiece hexPiece))
                    {
                        hexPiece.Init(hexPieceData);
                        return hexPiece;    
                    }
                }
            }

            return null;
        }
        
        private void SetObjectInBoard(Vector2 positionIndex, GameObject gameObject, string prefix = "")
        {
            gameObject.transform.SetParent(gridRoot);
            gameObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(positionIndex, totalRows, totalColumns);
            gameObject.name = $"{prefix} [{positionIndex.x}, {positionIndex.y}]";
        }
    }
}