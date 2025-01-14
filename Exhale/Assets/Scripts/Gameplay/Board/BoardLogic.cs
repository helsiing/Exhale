using Exhale.Scripts.Data;
using Exhale.Scripts.External.ServiceLocators;
using Exhale.Scripts.Services;

namespace Exhale.Scripts.Gameplay
{
    public class BoardLogic
    {
        private HexTileData[,] tilesData;
        public HexTileData[,] TilesData => tilesData;
        
        private HexPieceData[,] piecesData;
        public HexPieceData[,] PiecesData => piecesData;
        
        private readonly ServiceReference<IBoardService> boardService = new();
        
        public void InitBoard(int width, int height)
        {
            //TODO: get the services from the constructor
            
            tilesData = new HexTileData[width, height];
            piecesData = new HexPieceData[width, height];
            for (int row = 0; row < width; row++)
            {
                for (int col = 0; col < height; col++)
                {
                    HexTileData hexTileData = new HexTileData(row, col, false);
                    tilesData[row, col] = hexTileData;
                }
            }
        }
        
        public HexPieceData PlacePiece(int row, int col, HexPieceTemplate pieceTemplate = null)
        {
            if (!BoardHelper.IsWithinBounds(piecesData.GetLength(0), piecesData.GetLength(1), row, col))
            {
                return null;
            }

            // check if it's a building and if so update the tiles with the building unlock requirements
            /*if (pieceTemplate.TryGetTrait(out Building building))
            {
                foreach (var buildingRequirement in building.UnlockRequirementsData)
                {
                    int positionX = (int) (row + buildingRequirement.PositionIndex.x);
                    int positionY = (int) (col + buildingRequirement.PositionIndex.y);
                    
                    if(BoardHelper.IsWithinBounds(tilesData.GetLength(0), tilesData.GetLength(1), positionX, positionY))
                    {
                        HexTileData hexTileData = new HexTileData(positionX, positionY, true);
                        tilesData[positionX, positionY] = hexTileData;
                    }
                }
            }*/
            
            HexPieceData hexPieceData = new HexPieceData(row, col, pieceTemplate);
            piecesData[row, col] = hexPieceData;
            
            boardService.Reference.OnPiecePlaced?.Invoke(hexPieceData);
            return hexPieceData;

        }
    }
}