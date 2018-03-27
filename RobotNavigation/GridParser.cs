using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public static class GridParser
    {
        public const int MAX_GRID_WIDTH = 800;
        public const int MAX_GRID_HEIGHT = 700;

        public static NodeGrid LoadGridFrom(string filePath)
        {
            NodeGrid grid;

            using (StreamReader reader = new StreamReader(File.Open(filePath, FileMode.Open)))
            {
                grid = ReadGridDimensions(reader.ReadLine());
                grid.startNode = ReadNode(reader.ReadLine(), grid);
                grid.targetNode = ReadNode(reader.ReadLine(), grid);

                while (!reader.EndOfStream)
                    ReadWall(reader.ReadLine(), grid);
            }

            return grid;
        }

        // Extracts the cols and rows from the line in format [rows,cols]
        private static NodeGrid ReadGridDimensions(string input)
        {
            // Strip square braces
            input = input.Replace("[", "");
            input = input.Replace("]", "");

            // Separate into two numbers, rows and cols
            int commaIndex = input.IndexOf(",");

            int rows = Convert.ToInt32(input.Substring(0, commaIndex));
            int cols = Convert.ToInt32(input.Substring(commaIndex + 1));

            // Determine appropriate TileSize
            int tileWidth = MAX_GRID_WIDTH / cols;
            int tileHeight = MAX_GRID_HEIGHT / rows;

            // Make TileSize square
            int tileSize = Math.Min(tileWidth, tileHeight);

            // Create Grid
            return new NodeGrid(new Vector2(5, 5), cols, rows, tileSize);
        }

        // Extracts the node index in format (col,row) and returns the node at that index
        private static Node ReadNode(string input, NodeGrid grid)
        {
            // Strip brackets
            input = input.Replace("(", "");
            input = input.Replace(")", "");

            // Separate into two numbers, rows and cols
            int commaIndex = input.IndexOf(",");

            int cols = Convert.ToInt32(input.Substring(0, commaIndex));
            int rows = Convert.ToInt32(input.Substring(commaIndex + 1));

            return grid[cols, rows];
        }

        // Extracts the wall data in format (startCol, startRow, width, height) and sets corresponding nodes to wall.
        private static void ReadWall(string input, NodeGrid grid)
        {
            // Strip brackets
            input = input.Replace("(", "");
            input = input.Replace(")", "");

            // Separate string into the four numbers
            string[] stringValues = input.Split(new char[] { ',' });
            int[] intValues = stringValues.Select(int.Parse).ToArray();

            int wallStartCol = intValues[0];
            int wallStartRow = intValues[1];
            int wallWidth = intValues[2];
            int wallHeight = intValues[3];

            for (int col = wallStartCol; col < wallStartCol + wallWidth; ++col)
            {
                for (int row = wallStartRow; row < wallStartRow + wallHeight; ++row)
                    grid[col, row].isWall = true;
            }
        }
    }
}
