using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
{
    class RedSlime : Entity
    {

        /// <summary>
        /// Represents the slime
        /// </summary>
        /// <param name="nX">X Position in grid</param>
        /// <param name="nY">Y Position in grid</param>
        /// <param name="nType">Tile type</param>
        /// <param name="health">Max health</param>
        /// <param name="nDamage">Damage</param>
        /// <param name="nDice">Attack dice</param>
        /// <param name="nSprite">Sprite</param>
        public RedSlime(int nX, int nY, int nType, int health, int nDamage, Dice[] nDice, Texture2D nSprite) : base(nX, nY, nType, health, nDamage, nDice, nSprite)
        {
            maxHealth = health;
            currentHealth = maxHealth;
            damage = nDamage;
            sprite = nSprite;

            dice = nDice;

            name = "redslime";

            enemyID = 2;

            x = nX;
            y = nY;
            type = nType;

            // Give it a very moderate attack die
            setDice(0, new Dice(new int[6] { 1, 1, 1, 2, 2, 3 }));
            setDice(1, new Dice(new int[6] { 1, 1, 1, 2, 2, 3 }));
            setDice(2, new Dice(new int[6] { 1, 1, 1, 2, 2, 3 }));
            // Give it a very moderate movement die
            setMoveDie(new Dice(new int[6] { 1, 1, 1, 2, 2, 2 }));
        }
    }
}