using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
{
    /// <summary>
    /// Contains the definitions of a die
    /// </summary>
    public class Dice
    {
        public int[] sides = new int[6];
        
        /// <summary>
        /// Constructor to initialize a higher level die
        /// </summary>
        /// <param name="s1">Side one</param>
        /// <param name="s2">Side two</param>
        /// <param name="s3">Side three</param>
        /// <param name="s4">Side four</param>
        /// <param name="s5">Side five</param>
        /// <param name="s6">Side six</param>
        public Dice(int s1, int s2, int s3, int s4, int s5, int s6)
        {
            sides = new int[] { s1, s2, s3, s4, s5, s6 };
        }

        /// <summary>
        /// Constructor of a die, can plug in sides directly
        /// </summary>
        /// <param name="side">Array of sides</param>
        public Dice(int[] side)
        {
            sides = side;
        }

        /// <summary>
        /// Default constructor, makes the lowest level die
        /// </summary>
        public Dice()
        {
            sides = new int[] { 1, 1, 2, 2, 3, 3};
        }
    }
}
