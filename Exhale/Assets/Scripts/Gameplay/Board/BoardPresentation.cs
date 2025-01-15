using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using Exhale.Services;
using Exhale.Utils;
using UnityEngine;

namespace Exhale.Gameplay
{
    public class BoardPresentation : MonoBehaviour
    {
        [SerializeField] private GameObject emptyTilePrefab;
        [SerializeField] private Transform tilesRoot;
        [SerializeField] private Transform piecesRoot;
        
        private readonly ServiceReference<IBoardService> boardService = new();
        private IBoard board;

        private void Start()
        {
            boardService.Reference.OnPiecePlaced += OnPiecePlaced;
        }

        private void OnDestroy()
        {
            boardService.Reference.OnPiecePlaced -= OnPiecePlaced;
        }

        public void Init(IBoard board)
        {
            this.board = board;
            tilesRoot.gameObject.DestroyChildObjects();
            piecesRoot.gameObject.DestroyChildObjects();
        }

        public GameObject SetTileGameObject(HexTileData hexTileData)
        {
            if (hexTileData != null)
            {
                GameObject tileGameObject = Instantiate(emptyTilePrefab);
                if (tileGameObject != null)
                {
                    SetObjectInBoard(hexTileData.PositionIndex, tileGameObject, "[HexTile]");
                    tileGameObject.transform.SetParent(tilesRoot);
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
                    pieceGameObject.transform.SetParent(piecesRoot);
                    return pieceGameObject;
                }
            }

            return null;
        }

        public void ShowBoard(IHexPiece[,] pieces)
        {
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    if (pieces[x, y] != null)
                    {
                        pieces[x, y].Show();
                    }
                }
            }
        }
        
        void OnPiecePlaced(HexPieceData hexPieceData)
        {
            
        }
        
        private void SetObjectInBoard(Vector2 positionIndex, GameObject boardObject, string prefix = "")
        {
            boardObject.transform.position = BoardHelper.FromCoordinatesToWorldPosition(positionIndex, board.Width, board.Height);
            boardObject.name = $"{prefix} [{positionIndex.x}, {positionIndex.y}]";
        }
    }
}