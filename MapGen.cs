using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
{
    /// <summary>
    /// Contains the functions to generate a map
    /// </summary>
    class MapGen
    {

        Tile[,] map;
        Tile[,] overlay;

        int ROOM_WIDTH, ROOM_HEIGHT;

        Random rand = new Random();

        /// <summary>
        /// Constructor for the mapgen
        /// </summary>
        /// <param name="nGrid"></param>
        /// <param name="nOverlay"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public MapGen(Tile[,] nGrid, Tile[,] nOverlay, int width, int height)
        {
            map = nGrid;
            overlay = nOverlay;

            ROOM_WIDTH = width;
            ROOM_HEIGHT = height;
        }

        /// <summary>
        /// Generates the map based on the provided parameters.
        /// Currently uses Drunkard Walk algorithm to generate,
        /// which can be found here: http://pcg.wikidot.com/pcg-algorithm:drunkard-walk
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="type">Determines biases (kinda implemented)</param>
        /// <param name="steps">How many "steps" drunkard should take</param>
        /// <param name="numOfEnemies">Number of enemies to generate</param>
        private Tile[,] GenerateMap(int width, int height, int type, int steps, int numOfEnemies)
        {

            // UPDATE: Change all predetermined nums (numOfDoors, numOfRooms, etc.) to passable parameters to have more varying dungeons.

            /*
             * 
             * INT VALUES
             * 0 = EMPTY
             * 1 = SOLID
             * 2 = SPAWN
             * 3 = EXIT
             * 4 = ENEMY
             * 
             * 
             */

            /*
             * 
             * TYPE VALUES
             * 0 = nothing
             * 1 = corridor bias, kinda...
             * 
             */

            /*
             * 
             *  1. Pick a random point on a filled grid and mark it empty.
             *  2. Choose a random cardinal direction (N, E, S, W).
             *  3. Move in that direction, and mark it empty unless it already was.
             *  4. Repeat steps 2-3, until you have emptied as many grids as desired.
             *
             */

            // Initialize grid as solid
            // Put a border around the entire array
            Tile[,] grid = new Tile[width, height];
            overlay = new Tile[width, height];

            for (int i = 0; i < ROOM_WIDTH; i++)
            {
                for (int j = 0; j < ROOM_HEIGHT; j++)
                {
                    grid[i, j] = new Tile(i, j, 1);
                    overlay[i, j] = new Tile(i, j, 0);
                }
            }

            int currentStep = 0;
            bool moved = false;

            // 1. Define a start point, in this case the middle, and set it to empty (0)
            int x = ROOM_WIDTH / 2;
            int y = ROOM_HEIGHT / 2;

            grid[x, y].type = 0;

            /*
             * ----------MAP GEN------------
             */

            // While we still have steps to make
            while (currentStep < steps)
            {
                //printGrid(grid);
                //Debug.WriteLine("Current Step: " + currentStep + " X: " + x + " Y: " + y);

                // 3. Move in direction

                // NORMAL
                if (type == 0)
                {
                    // 2. Get a random number to represent direction
                    int dir = rand.Next(1, 5); // Between 1 and 4
                    switch (dir)
                    {
                        case 1: // NORTH
                            if (y != 1)
                            {
                                if (grid[x, y - 1].type == 1)
                                {
                                    grid[x, y - 1].type = 0;
                                }

                                y = y - 1;
                            }
                            currentStep++;
                            break;
                        case 2: // EAST
                            if (x != width - 2)
                            {
                                if (grid[x + 1, y].type == 1)
                                {
                                    grid[x + 1, y].type = 0;
                                }

                                x = x + 1;
                            }
                            currentStep++;
                            break;
                        case 3: // SOUTH
                            if (y != height - 2)
                            {
                                if (grid[x, y + 1].type == 1)
                                {
                                    grid[x, y + 1].type = 0;
                                }

                                y = y + 1;
                            }
                            currentStep++;
                            break;
                        case 4: // WEST
                            if (x != 1)
                            {
                                if (grid[x - 1, y].type == 1)
                                {
                                    grid[x - 1, y].type = 0;
                                }

                                x = x - 1;
                            }
                            currentStep++;
                            break;
                    }
                }
                else if (type == 1)
                {
                    int dirJustMoved = 0;
                    if (!moved)
                    {

                        // 2. Get a random number to represent direction
                        int dir = rand.Next(1, 5); // Between 1 and 4
                        dirJustMoved = dir;

                        switch (dir)
                        {
                            case 1: // NORTH
                                if (y != 1)
                                {
                                    if (grid[x, y - 1].type == 1)
                                    {
                                        grid[x, y - 1].type = 0;
                                    }

                                    y = y - 1;
                                }

                                currentStep++;
                                break;
                            case 2: // EAST
                                if (x != width - 2)
                                {
                                    if (grid[x + 1, y].type == 1)
                                    {
                                        grid[x + 1, y].type = 0;
                                    }

                                    x = x + 1;
                                }

                                currentStep++;
                                break;
                            case 3: // SOUTH
                                if (y != height - 2)
                                {
                                    if (grid[x, y + 1].type == 1)
                                    {
                                        grid[x, y + 1].type = 0;
                                    }

                                    y = y + 1;
                                }

                                currentStep++;
                                break;
                            case 4: // WEST
                                if (x != 1)
                                {
                                    if (grid[x - 1, y].type == 1)
                                    {
                                        grid[x - 1, y].type = 0;
                                    }

                                    x = x - 1;
                                }

                                currentStep++;
                                break;
                        }
                    }
                    else
                    {
                        switch (dirJustMoved)
                        {
                            case 1: // NORTH
                                if (y != 1)
                                {
                                    if (grid[x, y - 1].type == 1)
                                    {
                                        grid[x, y - 1].type = 0;
                                    }

                                    y = y - 1;
                                }

                                currentStep++;
                                break;
                            case 2: // EAST
                                if (x != width - 2)
                                {
                                    if (grid[x + 1, y].type == 1)
                                    {
                                        grid[x + 1, y].type = 0;
                                    }

                                    x = x + 1;
                                }

                                currentStep++;
                                break;
                            case 3: // SOUTH
                                if (y != height - 2)
                                {
                                    if (grid[x, y + 1].type == 1)
                                    {
                                        grid[x, y + 1].type = 0;
                                    }

                                    y = y + 1;
                                }

                                currentStep++;
                                break;
                            case 4: // WEST
                                if (x != 1)
                                {
                                    if (grid[x - 1, y].type == 1)
                                    {
                                        grid[x - 1, y].type = 0;
                                    }

                                    x = x - 1;
                                }

                                currentStep++;
                                break;
                        }

                        moved = false;
                    }
                }
            }

            /*
             * ----------SPECIAL PLACEMENTS------------
             */

            int randX;
            int randY;

            // ----PLAYER PLACEMENT---- //
            bool playerPlaced = false; // Determines if player has been placed
            bool exitPlaced = false; // Determines if exit has been placed

            // While the player is not placed
            while (!playerPlaced)
            {
                randX = rand.Next(0, width); // Random x position
                randY = rand.Next(0, height); // Random y position

                // If the selected tile is empty
                if (grid[randX, randY].type == 0)
                {
                    // Place the player
                    playerPlaced = true;
                    overlay[randX, randY] = new Player(randX, randY, 2, 6, 2, new Dice[1], null);
                    //player = (Player)overlay[randX, randY];

                }

            }

            // ----EXIT PLACEMENT---- //

            // While the exit is not placed
            while (!exitPlaced)
            {
                randX = rand.Next(0, width); // Random x position
                randY = rand.Next(0, height); // Random y position

                // If the selected tile is empty
                if (grid[randX, randY].type == 0)
                {
                    // Place the exit
                    exitPlaced = true;
                    grid[randX, randY].type = 3;
                }

            }

            // ----TREASURE ROOM PLACEMENT---- //
            int treasureRooms = 1;
            int currentRooms = 0;

            // UPDATE: Could potentially trace out from where doors SHOULD be and label them rooms, then produce doors that way

            // While we dont have the required number of room
            while (currentRooms < treasureRooms)
            {
                currentRooms++;
            }

            // ----ENEMIES PLACEMENT---- //

            int enemies = 0;

            // While we dont have the required number of enemies
            while (enemies < numOfEnemies)
            {
                randX = rand.Next(0, width); // Random x position
                randY = rand.Next(0, height); // Random y position

                // If the selected tile is empty
                if (grid[randX, randY].type == 0)
                {
                    // Place the exit
                    overlay[randX, randY].type = 4;
                    enemies++;
                }
            }

            // ----DOOR PLACEMENT---- //
            int numOfDoors = 2;
            int currentDoors = 0;

            // While we dont have the required number of doors
            while (currentDoors < numOfDoors)
            {
                //UPDATE: If save a list of all empty tiles, could probably speed up this part, but as of right now the speed is negligable
                //UPDATE: Could make every tile its own class and save all it's neighbours that way, allowing easier navigation of them for placement purposes, etc. 

                randX = rand.Next(0, width); // Random x position
                randY = rand.Next(0, height); // Random y position

                // If the selected tile is empty
                if (grid[randX, randY].type == 0)
                {
                    // Make sure we arent on an edge or it will cause an index out of bounds exception
                    if (randX != 0 && randX != width - 1 && randY != 0 && randY != height - 1)
                    {
                        // Check to make sure there is a solid wall on both sides of the door
                        if ((grid[randX + 1, randY].type == 1 && grid[randX - 1, randY].type == 1) || (grid[randX, randY - 1].type == 1 && grid[randX, randY + 1].type == 1))
                        {
                            grid[randX, randY].type = 9; //Door
                            currentDoors++;
                        }
                    }
                }
            }

            // ----EXTRAS PLACEMENT---- //

            int extrasMax = rand.Next(10, 30);
            int currentExtras = 0;

            // While we dont have the required number of extras
            while (currentExtras < extrasMax)
            {
                randX = rand.Next(0, width); // Random x position
                randY = rand.Next(0, height); // Random y position

                // If the selected tile is empty
                if (grid[randX, randY].type == 0)
                {

                    if (rand.Next(0, 2) == 0)
                    {
                        grid[randX, randY].type = 99; // Grass
                    }
                    else
                    {
                        grid[randX, randY].type = 98; // Rock
                    }
                    // Place the extra

                    currentExtras++;
                }
            }

            // ----SET NEIGHBOURS---- //

            for (int i = 0; i < width - 1; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    grid[i, j].setNeighbours(grid[i, j].findNeighbours(grid));
                }
            }

            return grid;

            //UPDATE: Generate random square rooms, save their top left and bottom right coords
            //        Select a random wall to start cooridor, then use coords to select random range
            //        of another room, you can determine if it is above, below, left, or right,
            //        then draw a cooridor to somewhere in that range
            //
            //        Another way is generate random room, then select random wall. Generate a random
            //        length corridor from that. At end of corridor, do random chance to produce room or another
            //        corridor in a direction.

        }

    }
}
