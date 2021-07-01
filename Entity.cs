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
    /// Respresents anything that isn't static, like players or enemies (maybe passive mobs???)
    /// </summary>
    public class Entity : Tile
    {

        public int maxHealth;
        public int currentHealth;
        public int damage; // May be used if rework hp systema

        public Dice[] dice;
        public Dice moveDie;

        public int enemyID;

        public int viewDistance = 0; // Used for shading on player, and pathfinding on enemy 
        public bool isEnemy = false;

        public Texture2D sprite;

        public string name = "default";

        /// <summary>
        /// Represents the entity
        /// </summary>
        /// <param name="nX">X Position in grid</param>
        /// <param name="nY">Y Position in grid</param>
        /// <param name="nType">Tile type</param>
        /// <param name="health">Max health</param>
        /// <param name="nDamage">Damage</param>
        /// <param name="nDice">Attack dice</param>
        /// <param name="nSprite">Sprite</param>
        public Entity(int nY, int nX, int nType, int health, int nDamage, Dice[] nDice, Texture2D nSprite) : base(nX, nY, nType)
        {
            maxHealth = health;
            currentHealth = maxHealth;
            damage = nDamage;
            sprite = nSprite;

            dice = nDice;

            x = nX;
            y = nY;
            type = nType;
        }

        /// <summary>
        /// More basic entity constructor
        /// </summary>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        /// <param name="nType"></param>
        public Entity(int nX, int nY, int nType) : base(nX, nY, nType)
        {
            y = nX;
            y = nY;
            type = nType;
        }

        /// <summary>
        /// Sets the entities attack dice
        /// </summary>
        /// <param name="index"></param>
        /// <param name="die"></param>
        public void setDice(int index, Dice die)
        {
            Debug.WriteLine(index + " | " + die);
            dice[index] = die;
        }

        /// <summary>
        /// Sets the entities move die
        /// </summary>
        /// <param name="die"></param>
        public void setMoveDie(Dice die)
        {
            moveDie = die;
        }

    }
}
