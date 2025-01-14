using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using Exhale.Scripts.Services;
using Exhale.Scripts.Utils;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public class BoardPresentation : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private Transform gridRoot;
        
        private readonly ServiceReference<IBoardService> boardService = new();
        
        private int totalRows;
        private int totalColumns;

        private void Start()
        {
            boardService.Reference.OnPiecePlaced += OnPiecePlaced;
        }

        private void OnDestroy()
        {
            boardService.Reference.OnPiecePlaced -= OnPiecePlaced;
        }

        public void InitBoard(HexTileData[,] tiles)
        {
            gridRoot.gameObject.DestroyChildObjects();
            totalRows = tiles.GetLength(0);
            totalColumns = tiles.GetLength(1);
        }

        public GameObject SetTileGameObject(HexTileData hexTileData)
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

        public GameObject SetPieceGameObject(HexPieceData hexPieceData)
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
        
        void OnPiecePlaced(HexPieceData hexPieceData)
        {
            Debug.Log($"Piece placed on ({hexPieceData.PositionIndex.x}, {hexPieceData.PositionIndex.y}) with template {hexPieceData.PieceTemplate}");
        }
        
        private void SetObjectInBoard(Vector2 positionIndex, GameObject boardObject, string prefix = "")
        {
            boardObject.transform.SetParent(gridRoot);
            boardObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(positionIndex, totalRows, totalColumns);
            boardObject.name = $"{prefix} [{positionIndex.x}, {positionIndex.y}]";
        }
    }
}