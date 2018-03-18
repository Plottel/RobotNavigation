﻿using System;
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
        private const int MAX_GRID_WIDTH = 800;
        private const int MAX_GRID_HEIGHT = 700;

        public static NodeGrid LoadGridFrom(string filePath)
        {
            NodeGrid grid;

            using (StreamReader reader = new StreamReader(File.Open(filePath, FileMode.Open)))
            {
                grid = SetupGridDimensions(reader.ReadLine());
                var startNode = reader.ReadLine();
                var targetNode = reader.ReadLine();

                while (!reader.EndOfStream)
                    LoadWall(reader.ReadLine(), grid);
            }

            return grid;
        }

        // Extracts the cols and rows from the line in format [rows, cols]
        private static NodeGrid SetupGridDimensions(string input)
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

        // Extracts the wall data in format (startCol, startRow, width, height) and sets corresponding nodes to wall.
        private static void LoadWall(string input, NodeGrid grid)
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
