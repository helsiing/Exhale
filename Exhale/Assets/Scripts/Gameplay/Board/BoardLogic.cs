using System.Collections.Generic;
using System.Linq;
using Exhale.Scripts.Data;
using UnityEngine;

namespace Exhale.Gameplay
{
    public interface IBoardLogic
    {
        public void Init(int width, int height);
        public HexTileData GetTileAt(int x, int y);
        public bool SetTileAt(int x, int y, HexTileData hexTileData);
        public HexPieceData GetPieceAt(int x, int y);
        public bool SetPieceAt(int x, int y, HexPieceData hexPieceData);
        public HexPieceData PlacePiece(int x, int y, HexPieceTemplate pieceTemplate = null);
    }
    
    public class BoardLogic: IBoardLogic
    {
        private HexTileData[,] tilesData;
        
        private HexPieceData[,] piecesData;
        private int width;
        private int height;
        
        public void Init(int width, int height)
        {
            this.width = width;
            this.height = height;
            
            tilesData = new HexTileData[width, height];
            piecesData = new HexPieceData[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    SetTileAt(x, y, new HexTileData(x, y));
                }
            }
        }
        
        public HexTileData GetTileAt(int x, int y)
        {
            return BoardHelper.IsWithinBounds(width, height, x, y) ? tilesData[x, y] : null;
        }

        public bool SetTileAt(int x, int y, HexTileData hexTileData)
        {
            if (!BoardHelper.IsWithinBounds(width, height, x, y)) return false;
            tilesData[x, y] = hexTileData;
            return true;
        }

        public HexPieceData GetPieceAt(int x, int y)
        {
            return BoardHelper.IsWithinBounds(width, height, x, y) ? piecesData[x, y] : null;
        }

        public bool SetPieceAt(int x, int y, HexPieceData hexPieceData)
        {
            if (!BoardHelper.IsWithinBounds(width, height, x, y)) return false;
            piecesData[x, y] = hexPieceData;
            tilesData[x, y].SetHasPiece(true);
            
            foreach (var neighbourTileData in BoardHelper.GetNeighbours(new Vector2(x, y), width, height)
                         .Select(neighbour => tilesData[(int)neighbour.x, (int)neighbour.y])
                         .Where(neighbourTileData => !neighbourTileData.HasPiece))
            {
                neighbourTileData.SetEnabled(true);
            }
            
            return true;
        }
        
        public HexPieceData PlacePiece(int x, int y, HexPieceTemplate pieceTemplate = null)
        {
            if (!BoardHelper.IsWithinBounds(width, height, x, y))
            {
                return null;
            }
            
            HexPieceData hexPieceData = new HexPieceData(x, y, pieceTemplate);
            SetPieceAt(x, y, hexPieceData);
            
            return hexPieceData;

        }
    }
}