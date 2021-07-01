using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
{
    /// <summary>
    /// Contains the definitions for a single tile. Should contain things like neighbours and their values, as well as it's own position
    /// </summary>
    public class Tile
    {

        public int x { get; set; }
        public int y { get; set; }
        public int type { get; set; }

        public Tile[,] neighbours = new Tile[3, 3];

        public Tile(int nX, int nY, int nType)
        {
            x = nX;
            y = nY;
            type = nType;
        }


        /// <summary>
        /// Sets the neighbours
        /// </summary>
        /// <param name="value"></param>
        public void setNeighbours(Tile[,] value)
        {
            neighbours = value;
        }

        /// <summary>
        /// Set the neighbours of the tile
        /// </summary>
        /// <param name="grid">The grid passed for it to check its neighbours against</param>
        /// <returns>Completed list of neighbours</returns>
        public Tile[,] findNeighbours(Tile[,] grid)
        {

            Tile[,] nNeighbours = new Tile[3, 3];
            Tile[,] map = grid;
            int width = grid.GetLength(0); // Get the width
            int height = grid.GetLength(1); // Get the height

            if ((x != 0 && x != width - 1) && (y != 0 && y != height - 1)) // Dont check the border
            {
                        //Debug.WriteLine("I: " + i + " J: " + j);
                nNeighbours[0, 0] = map[x - 1, y - 1];
                nNeighbours[0, 1] = map[x, y - 1];
                nNeighbours[0, 2] = map[x + 1, y - 1];
                nNeighbours[1, 0] = map[x - 1, y];
                nNeighbours[1, 1] = map[x, y];
                nNeighbours[1, 2] = map[x + 1, y];
                nNeighbours[2, 0] = map[x - 1, y + 1];
                nNeighbours[2, 1] = map[x, y + 1];
                nNeighbours[2, 2] = map[x + 1, y + 1];
            }

            return nNeighbours;
        }

        /// <summary>
        /// Returns the neighbours
        /// </summary>
        /// <returns>Neighbours of the tile</returns>
        public Tile[,] getNeighbours()
        {
            return neighbours;
        }

    }
}
