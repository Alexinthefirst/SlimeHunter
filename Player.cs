using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
{
    /// <summary>
    /// Contains the definition for a player. Contains stats such as health and damage, as well as inventory and its sprite
    /// </summary>
    class Player : Entity
    {

        public string[] inventory;

        /// <summary>
        /// Represents the player
        /// </summary>
        /// <param name="nX">X Position in grid</param>
        /// <param name="nY">Y Position in grid</param>
        /// <param name="nType">Tile type</param>
        /// <param name="health">Max health</param>
        /// <param name="nDamage">Damage</param>
        /// <param name="nDice">Attack dice</param>
        /// <param name="nSprite">Sprite</param>
        public Player(int nX, int nY, int nType, int health, int nDamage, Dice[] nDice, Texture2D nSprite) : base(nX, nY, nType, health, nDamage, nDice, nSprite)
        {
            maxHealth = health;
            currentHealth = maxHealth;
            damage = nDamage;
            sprite = nSprite;

            dice = nDice;

            x = nX;
            y = nY;
            type = nType;

            // Give good move die
            setMoveDie(new Dice(new int[6] { 1, 2, 3, 3, 3, 4 }));
        }

        /// <summary>
        /// Represents the player
        /// </summary>
        /// <param name="nX">X Position in grid</param>
        /// <param name="nY">Y Position in grid</param>
        /// <param name="nType">Tile type</param>
        /// <param name="health">Max health</param>
        /// <param name="nDamage">Damage</param>
        /// <param name="nDice">Attack dice</param>
        /// <param name="nSprite">Sprite</param>
        /// <param name="nInventory">Inventory</param>
        public Player(int nX, int nY, int nType, int health, int nDamage, Dice[] nDice, Texture2D nSprite, string[] nInventory) : base(nX, nY, nType, health, nDamage, nDice, nSprite)
        {
            maxHealth = health;
            currentHealth = maxHealth;
            damage = nDamage;
            sprite = nSprite;
            inventory = nInventory;

            dice = nDice;

            x = nX;
            y = nY;
            type = nType;

            // Give good move die
            setMoveDie(new Dice(new int[6] { 1, 2, 3, 3, 3, 4 }));
        }

        /// <summary>
        /// Adds one to the lowest value on a die
        /// </summary>
        public void upgradeLowestDie()
        {
            bool hasUpgraded = false;
            int curVal = 1;
            int curDie = 0;
            int curSide = 0;

            while (!hasUpgraded) {

                // Check if side is current value
                if (dice[curDie].sides[curSide] == curVal)
                {
                    dice[curDie].sides[curSide] += 1;
                    hasUpgraded = true;
                }

                // If we reached the last side
                if (curSide == 5)
                {
                    curDie++;
                    curSide = 0;
                }

                curSide++;
                
            }

            for (int i = 0; i < dice.Length; i++)
            {
                for (int j = 0; j < dice[i].sides.Length; j++)
                {
                    Debug.Write(dice[i].sides[j] + " | ");
                }
            }

            Debug.WriteLine(" | ");
            

        }

    }
}
