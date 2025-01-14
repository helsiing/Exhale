using System.Collections.Generic;
using UnityEngine;

namespace Exhale.Scripts.Gameplay
{
    public static class BoardHelper
    {
        public static bool IsWithinBounds(int width, int height, int row, int col)
        {
            if (row > width || col > height)
            {
                return false;
            }

            return true;
        }
        
        public static Vector2 GetBoardCenter(int width, int height) 
        {
            return new Vector2(width / 2, height / 2);
        }
        
        public static Vector3 FromCoordinatesToWorldPosition(Vector2 position, int totalRows, int totalCols)
        {
            int row = (int)position.x;
            int col = (int)position.y;

            // Hexagon offsets (assuming flat-topped hexes)
            float xOffset = (col % 2 == 1) ? 0.5f : 0f;
            float zOffset = 0.87f; // Distance between rows (based on hex height)

            // Calculate raw world position (bottom-left origin)
            Vector3 rawPosition = new Vector3(row + xOffset, 0, col * zOffset);

            // Calculate board center offset
            float boardWidth = (totalRows - 1) + 0.5f;  // Approx width of the board
            float boardHeight = (totalCols - 1) * zOffset; // Approx height of the board
            Vector3 boardCenterOffset = new Vector3(boardWidth / 2f, 0, boardHeight / 2f);

            // Offset the position to center the board at (0, 0, 0)
            return rawPosition - boardCenterOffset;
        }
        
        public static List<Vector2> GetNeighbours(Vector2 position, int totalRows, int totalCols)
        {
            List<Vector2> neighbours = new List<Vector2>();
            int row = (int)position.x;
            int col = (int)position.y;

            // Check all 6 possible neighbours
            Vector2[] possibleNeighbours = new Vector2[]
            {
                new Vector2(row - 1, col), // Top
                new Vector2(row + 1, col), // Bottom
                new Vector2(row, col - 1), // Left
                new Vector2(row, col + 1), // Right
                new Vector2(row - 1, col + 1), // Top Right
                new Vector2(row + 1, col - 1) // Bottom Left
            };

            // Check if each neighbour is within bounds
            foreach (Vector2 neighbour in possibleNeighbours)
            {
                if (IsWithinBounds(totalRows, totalCols, (int)neighbour.x, (int)neighbour.y))
                {
                    neighbours.Add(neighbour);
                }
            }

            return neighbours;
        }
    }
}