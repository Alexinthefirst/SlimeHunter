using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
{
    /// <summary>
    /// Contains a room definition
    /// </summary>
    class Room
    {
        public int xPos, yPos, xSize, ySize;

        /// <summary>
        /// Sets all the variables need for a room definiton
        /// </summary>
        /// <param name="nxPos"></param>
        /// <param name="nyPos"></param>
        /// <param name="nxSize"></param>
        /// <param name="nySize"></param>
        public Room(int nxPos, int nyPos, int nxSize, int nySize)
        {
            xPos = nxPos;
            yPos = nyPos;
            xSize = nxSize;
            ySize = nySize;
        }
    }
}
