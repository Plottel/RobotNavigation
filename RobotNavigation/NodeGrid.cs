using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace RobotNavigation
{
    public class NodeGrid : Grid<Node>
    {
        private Vector2 _pos;
        private int _tileSize;

        public Vector2 Pos { get => _pos; }
        public int TileSize { get => _tileSize; }

        public Node startNode;
        public Node targetNode;

        // <summary>
        /// The width of the grid.
        /// -1 so the absolute edge doesn't register next cell and out of bounds.
        /// </summary>
        public int Width
        {
            get { return Cols * _tileSize - 1; }
        }

        /// <summary>
        /// The height of the grid.
        /// -1 so the absolute edge doesn't register next cell and out of bounds.
        /// </summary>
        public int Height
        {
            get { return Rows * _tileSize - 1; }
        }

        public Rectangle Bounds
        {
            get => new Rectangle((int)_pos.X, (int)_pos.Y, Width, Height);
        }

        public NodeGrid(Vector2 pos, int cols, int rows, int tileSize)
        {
            _pos = pos;
            _tileSize = tileSize;
            AddColumns(cols);
            AddRows(rows);

            SetupNeighbours();
        }

        public Node NodeAt(Vector2 pos)
        {
            int col = (int)Math.Floor((pos.X - Pos.X) / _tileSize);
            int row = (int)Math.Floor((pos.Y - Pos.Y) / _tileSize);

            return this[col, row];
        }

        public Point IndexAt(Vector2 pos)
        {
            var col = (int)Math.Floor((pos.X - Pos.X) / _tileSize);
            var row = (int)Math.Floor((pos.Y - Pos.Y) / _tileSize);

            return new Point(col, row);
        }

        public Point IndexOf(Node node) => IndexAt(node.pos);

        public List<Node> GetSquareNeighbours(Node node)
        {
            var idx = IndexAt(node.pos);

            var result = new List<Node>();

            for (int col = idx.Col() - 1; col <= idx.Col() + 1; ++col)
            {
                for (int row = idx.Row() - 1; row <= idx.Row() + 1; ++row)
                    AddNodeIfNotNull(this[col, row], result);
            }

            result.Remove(node);
            return result;
        }

        public List<Node> GetOrthogonalNeighbours(Node node)
        {
            var idx = IndexAt(node.pos);

            var result = new List<Node>();

            AddNodeIfNotNull(this[idx.Col(), idx.Row() - 1], result);       // North
            AddNodeIfNotNull(this[idx.Col() - 1, idx.Row()], result);       // West
            AddNodeIfNotNull(this[idx.Col(), idx.Row() + 1], result);       // South
            AddNodeIfNotNull(this[idx.Col() + 1, idx.Row()], result);       // East           

            return result;
        }

        /// <summary>
        /// Adds columns to the grid.
        /// Columns are the outer list, so adding a column creates a new list and populates it.
        /// </summary>
        /// <param name="amountToAdd">The number of columns to add.</param>
        public void AddColumns(int amountToAdd)
        {
            // For each column to be added.
            for (int col = 0; col < amountToAdd; col++)
            {
                var newCol = new List<Node>();

                // For each row in the newly created column.
                for (int row = 0; row < Rows; row++)
                {
                    // Add the new tile.
                    var newTile = new Node();
                    newTile.pos = new Vector2(Pos.X + Cols * _tileSize, Pos.Y + Rows * _tileSize);
                    newTile.size = new Vector2(_tileSize, _tileSize);

                    newCol.Add(newTile);
                }

                // Add the column to cells, increment number of columns in the grid.
                Cells.Add(newCol);
                ++Cols;
            }
        }

        /// <summary>
        /// Adds rows to the grid.
        /// Rows are the inner list, so adding a row appends a new cell to the end of each list.
        /// </summary>
        /// <param name="amountToAdd">The number of rows to add.</param>
        public void AddRows(int amountToAdd)
        {
            // For each row to be added.
            for (int row = 0; row < amountToAdd; row++)
            {
                // For each column to have a new row added to it.
                for (int col = 0; col < Cols; col++)
                {
                    // Add the new cell to the column.
                    var newTile = new Node();
                    newTile.pos = new Vector2(Pos.X + col * _tileSize, Pos.Y + Rows * _tileSize);
                    newTile.size = new Vector2(_tileSize, _tileSize);

                    Cells[col].Add(newTile);
                }

                // Increment the number of rows in the grid.
                ++Rows;
            }
        }

        public void SetupNeighbours()
        {
            foreach (var node in this)
                node.neighbours = GetOrthogonalNeighbours(node);
        }

        private void AddNodeIfNotNull(Node node, ICollection<Node> list)
        {
            if (node != null)
                list.Add(node);
        }

    }
}
