using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

// UPDATE: potentially allow different types of enemies to fight eachother?? Combat code will be written with this in mind

namespace Roguelike
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        static Random rand = new Random(); // Declare a random, to be used throughout the program for random numbers

        public enum GameState
        {
            Menu,
            Playing,
            Paused,
            LoadingLevel,
            About,
            Help,
            Highscore
        }

        public GameState gameState = GameState.Menu;

        // TEXTURES
        Texture2D caveWallTexture;
        Texture2D caveFloorTexture;
        Texture2D playerSpritesheet;
        Texture2D enemyTexture;
        Texture2D stairsTexture, lockTexture, fade;
        Texture2D dieOne, dieTwo, dieThree, dieFour, menuBackground, diceLabel, diceAttackLabel, diceMoveLabel;

        Texture2D greenSpritesheet, redSpritesheet, purpleSpritesheet;
        AnimatedSprite animatedSpriteGreenSlime, animatedSpriteRedSlime, animatedSpritePurpleSlime, animatedPlayerSprite;

        Texture2D mainMenuTexture, aboutMenuTexture, helpMenuTexture, cursorTexture, highscoreMenuTexture, rollPanelTexture;

        SpriteFont font;

        Texture2D caveRockTexture, caveGrassTexture, caveDoorTexture;
        Texture2D heartEmptyTexture, heartHalfTexture, heartFullTexture;

        SoundEffectInstance sfi;

        bool isRollingOpen = false;

        int playerX, playerY;

        List<int> combatRollsOne = new List<int>(); // Stores rolls and is used for printing
        List<int> combatRollsTwo = new List<int>();

        bool gameInProgress = false; // Used by various functions to see if the game is in progress

        List<Entity> moveQueue = new List<Entity>(); // Holds the queue of moves
        int currentTurn = 0; // Current position in move queue

        bool isLocked = true; // If the current floor is locked or not
        int currentFloor = 1; // The current floor number
        int enemiesRemaining; // Number of enemies left on the floor

        int movesLeft = 0; // Amount of moves a player has left on the current turn

        const int TILE_SIZE = 20; // Holds tile size, used for placement

        const int ROOM_WIDTH = 40; // Predetermined room width
        const int ROOM_HEIGHT = 40; // Predetermined room height

        float rollDelay = 2f; // Time until rollPanel disappears

        Tile[,] map; // To hold our map
        Entity[,] overlay; // To hold player and enemy positions

        Player player;

        SoundEffect diceRoll, slimeDeath, menuMove, select;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int cursorPos = 1; // Option cursor is at on main menu

        /// <summary>
        /// Constructor that sets up graphics
        /// </summary>
        public Game1()
        {
            this.Window.Title = "Slime Hunter";
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = (ROOM_WIDTH) * 20;
            graphics.PreferredBackBufferHeight = (ROOM_HEIGHT) * 20;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            map = GenerateMap(ROOM_WIDTH, ROOM_HEIGHT, 1, 2000, 3);
            //map = GenerateMap(ROOM_WIDTH, ROOM_HEIGHT, 0, 20, 7, 0);
            gameState = GameState.Menu;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            caveWallTexture = Content.Load<Texture2D>("caveWall");
            font = Content.Load<SpriteFont>("File");
            caveFloorTexture = Content.Load<Texture2D>("caveFloor");
            playerSpritesheet = Content.Load<Texture2D>("charSpritesheet");
            enemyTexture = Content.Load<Texture2D>("enemyPlaceholder");
            stairsTexture = Content.Load<Texture2D>("caveStairs");
            lockTexture = Content.Load<Texture2D>("lock");
            caveRockTexture = Content.Load<Texture2D>("caveRock");
            caveGrassTexture = Content.Load<Texture2D>("caveGrass");
            caveDoorTexture = Content.Load<Texture2D>("caveDoor");
            heartEmptyTexture = Content.Load<Texture2D>("heartEmpty");
            heartHalfTexture = Content.Load<Texture2D>("heartHalf");
            heartFullTexture = Content.Load<Texture2D>("heartFull");
            dieOne = Content.Load<Texture2D>("dieSideOne");
            dieTwo = Content.Load<Texture2D>("dieSideTwo");
            dieThree = Content.Load<Texture2D>("dieSideThree");
            dieFour = Content.Load<Texture2D>("dieSideFour");
            diceLabel = Content.Load<Texture2D>("diceLabel");
            diceAttackLabel = Content.Load<Texture2D>("diceAttackLabel");
            diceMoveLabel = Content.Load<Texture2D>("diceMoveLabel");
            menuBackground = Content.Load<Texture2D>("pauseMenu");
            fade = Content.Load<Texture2D>("fade");
            greenSpritesheet = Content.Load<Texture2D>("greenSlimeSpriteSheet");
            redSpritesheet = Content.Load<Texture2D>("redSlimeSpriteSheet");
            purpleSpritesheet = Content.Load<Texture2D>("purpleSlimeSpriteSheet");
            mainMenuTexture = Content.Load<Texture2D>("mainMenu");
            helpMenuTexture = Content.Load<Texture2D>("helpMenu");
            aboutMenuTexture = Content.Load<Texture2D>("aboutMenu");
            highscoreMenuTexture = Content.Load<Texture2D>("highscoreMenu");
            cursorTexture = Content.Load<Texture2D>("cursor");
            rollPanelTexture = Content.Load<Texture2D>("rollPanel");

            Song song = Content.Load<Song>("song18"); //  Main game music https://opengameart.org/art-search-advanced?keys=cave&title=&field_art_tags_tid_op=or&field_art_tags_tid=&name=&field_art_type_tid%5B%5D=12&sort_by=count&sort_order=DESC&items_per_page=24&Collection=
            diceRoll = Content.Load<SoundEffect>("diceThrow"); // https://opengameart.org/content/54-casino-sound-effects-cards-dice-chips
            //slimeDeath = Content.Load<SoundEffect>("slimeDeath");// https://opengameart.org/content/slime-ultimate-the-best
            select = Content.Load<SoundEffect>("select");// https://opengameart.org/content/51-ui-sound-effects-buttons-switches-and-clicks
            menuMove = Content.Load<SoundEffect>("menuMove");// https://opengameart.org/content/51-ui-sound-effects-buttons-switches-and-clicks

            MediaPlayer.Play(song); // Play the song at the start
            MediaPlayer.Volume = 0.05f;
            MediaPlayer.IsRepeating = true; // Make the song repeat

            animatedSpriteGreenSlime = new AnimatedSprite(greenSpritesheet, 1, 7);
            animatedSpriteRedSlime = new AnimatedSprite(redSpritesheet, 1, 7);
            animatedSpritePurpleSlime = new AnimatedSprite(purpleSpritesheet, 1, 7);
            animatedPlayerSprite = new AnimatedSprite(playerSpritesheet, 1, 7);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        bool keyPressed = false;
        bool upPressed = false;
        bool downPressed = false;
        bool leftPressed = false;
        bool rightPressed = false;
        bool ePressed = false;
        bool escPressed = false;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) && escPressed == false)
            {
                escPressed = true;

                if (gameState == GameState.Playing)
                {
                    gameState = GameState.Paused;
                }
                else if (gameState == GameState.Paused)
                {
                    gameState = GameState.Playing;
                }
            }

            if (isRollingOpen) { // Close rollPanel after so much time
                if (rollDelay < 0)
                {
                    isRollingOpen = false;
                    rollDelay = 2f;
                }
                else
                {
                        rollDelay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;
                }
            }
                

            // Pause Menu
            if (gameState == GameState.Paused)
            {
                // Back to main menu (quit)
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    gameState = GameState.Menu;
                    cursorPos = 1;
                }
            }

            // Main Menu
            if (gameState == GameState.Menu)
            {
                // Make sure our cursor doesnt go out of bounds
                if (cursorPos < 1)
                {
                    cursorPos = 1;
                }
                else if (cursorPos > 5)
                {
                    cursorPos = 5;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W) && upPressed == false && !keyPressed)
                {
                    sfi = menuMove.CreateInstance();
                    sfi.Play();
                    upPressed = true;
                    keyPressed = true;
                    cursorPos--;

                }

                if (Keyboard.GetState().IsKeyDown(Keys.S) && downPressed == false && !keyPressed)
                {
                    sfi = menuMove.CreateInstance();
                    sfi.Play();
                    downPressed = true;
                    keyPressed = true;
                    cursorPos++;
                }

                if (Keyboard.GetState().IsKeyUp(Keys.W))
                {
                    upPressed = false;
                    keyPressed = false;
                }

                if (Keyboard.GetState().IsKeyUp(Keys.S))
                {
                    downPressed = false;
                    keyPressed = false;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    sfi = select.CreateInstance();
                    sfi.Play();
                    // Start
                    if (cursorPos == 1)
                    {
                        gameState = GameState.Playing;
                    }
                    // About
                    else if (cursorPos == 2)
                    {
                        gameState = GameState.About;
                    }
                    // Help
                    else if (cursorPos == 3)
                    {
                        gameState = GameState.Help;
                    }
                    // Highscore
                    else if (cursorPos == 4)
                    {
                        gameState = GameState.Highscore;
                    }
                    // Quit
                    else if (cursorPos == 5)
                    {
                        Exit();
                    }
                }
            }
            // Help Menu
            if (gameState == GameState.Help)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    gameState = GameState.Menu;
                }
            }
            // About Menu
            if (gameState == GameState.About)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    gameState = GameState.Menu;
                }
            }
            // Highscore Menu
            if (gameState == GameState.Highscore)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    gameState = GameState.Menu;
                }
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Escape))
            {
                escPressed = false;
            }

            // Playing
            if (gameState == GameState.Playing)
            {

                animatedSpriteGreenSlime.Update(gameTime);
                animatedSpritePurpleSlime.Update(gameTime);
                animatedSpriteRedSlime.Update(gameTime);
                animatedPlayerSprite.Update(gameTime);

                if (map[player.x, player.y].type == 3 && isLocked == false && gameState != GameState.LoadingLevel)
                {
                    player.currentHealth = player.maxHealth;
                    gameState = GameState.LoadingLevel;
                    currentFloor++;
                    map = GenerateMap(ROOM_WIDTH, ROOM_HEIGHT, 1, 2000, 2 * currentFloor);
                    isLocked = true;

                }

                if (player.currentHealth < 1)
                {
                    updateHighscore(currentFloor);
                }

                if (enemiesRemaining < 1)
                {
                    isLocked = false;
                }

                switch (currentTurn)
                {
                    case 0: // Player turn
                        if (movesLeft <= 0) // If we haven't made moves yet
                        {
                            movesLeft = player.moveDie.sides[rand.Next(0, 6)]; // Roll the move die
                        }

                        // TODO: Add your update logic here
                        var kstate = Keyboard.GetState();
                        if (currentTurn == 0) // If its the players turn
                        {
                            if (movesLeft > 0) // If the player has moves left
                            {
                                if ((kstate.IsKeyDown(Keys.W)) && upPressed == false && !keyPressed)
                                {
                                    keyPressed = true;

                                    // If we hit a mob
                                    if (overlay[playerX, playerY - 1].type != 0)
                                    {
                                        combat(overlay[playerX, playerY], overlay[playerX, playerY - 1]);
                                        upPressed = true;
                                        movesLeft = 0;
                                    }
                                    else if (map[playerX, playerY - 1].type != 1 && overlay[playerX, playerY - 1].type != 4)
                                    {
                                        overlay[playerX, playerY] = new Entity(playerX, playerY, 0);
                                        overlay[playerX, playerY - 1] = player;
                                        player.y -= 1;
                                        upPressed = true;
                                        movesLeft--;
                                    }
                                }

                                if ((kstate.IsKeyDown(Keys.S)) && downPressed == false && !keyPressed)
                                {
                                    keyPressed = true;

                                    // If we hit a mob
                                    if (overlay[playerX, playerY + 1].type != 0)
                                    {
                                        combat(overlay[playerX, playerY], overlay[playerX, playerY + 1]);
                                        downPressed = true;
                                        movesLeft = 0;
                                    }
                                    if (map[playerX, playerY + 1].type != 1 && overlay[playerX, playerY + 1].type != 4)
                                    {
                                        overlay[playerX, playerY] = new Entity(playerX, playerY, 0);
                                        overlay[playerX, playerY + 1] = player;
                                        player.y += 1;
                                        downPressed = true;
                                        movesLeft--;
                                    }
                                }

                                if ((kstate.IsKeyDown(Keys.A)) && leftPressed == false && !keyPressed)
                                {
                                    keyPressed = true;

                                    // If we hit a mob
                                    if (overlay[playerX - 1, playerY].type != 0)
                                    {
                                        combat(overlay[playerX, playerY], overlay[playerX - 1, playerY]);
                                        leftPressed = true;
                                        movesLeft = 0;
                                    }
                                    if (map[playerX - 1, playerY].type != 1 && overlay[playerX - 1, playerY].type != 4)
                                    {
                                        overlay[playerX, playerY] = new Entity(playerX, playerY, 0);
                                        overlay[playerX - 1, playerY] = player;
                                        leftPressed = true;
                                        movesLeft--;
                                        player.x -= 1;
                                    }
                                }

                                if ((kstate.IsKeyDown(Keys.D)) && rightPressed == false && !keyPressed)
                                {
                                    keyPressed = true;

                                    // If we hit a mob
                                    if (overlay[playerX + 1, playerY].type != 0)
                                    {
                                        combat(overlay[playerX, playerY], overlay[playerX + 1, playerY]);
                                        rightPressed = true;
                                        movesLeft = 0;
                                    }
                                    if (map[playerX + 1, playerY].type != 1 && overlay[playerX + 1, playerY].type != 4)
                                    {
                                        overlay[playerX, playerY] = new Entity(playerX, playerY, 0);
                                        overlay[playerX + 1, playerY] = player;
                                        rightPressed = true;
                                        movesLeft--;
                                        player.x += 1;
                                    }
                                }

                                if (kstate.IsKeyDown(Keys.E) && ePressed == false && !keyPressed)
                                {
                                    keyPressed = true;
                                    ePressed = true;
                                    player.currentHealth--;
                                }

                                if (kstate.IsKeyUp(Keys.E))
                                {
                                    ePressed = false;
                                    keyPressed = false;
                                }

                                if (kstate.IsKeyUp(Keys.W))
                                {
                                    upPressed = false;
                                    keyPressed = false;
                                }

                                if (kstate.IsKeyUp(Keys.S))
                                {
                                    downPressed = false;
                                    keyPressed = false;
                                }

                                if (kstate.IsKeyUp(Keys.A))
                                {
                                    leftPressed = false;
                                    keyPressed = false;
                                }

                                if (kstate.IsKeyUp(Keys.D))
                                {
                                    rightPressed = false;
                                    keyPressed = false;
                                }


                            }
                            if (movesLeft <= 0)
                            {
                                currentTurn++;
                            }
                        }
                        break;

                    case 1: // Every other creature


                        for (int i = 1; i < moveQueue.Count; i++)
                        {
                            Entity entity = moveQueue[i];
                            int moveRoll = entity.moveDie.sides[rand.Next(0, 6)]; // Get move value from move die
                                                                                  // Check to see if player is visible

                            //Debug.WriteLine("DIR: " + dirToMove(entity));

                            switch (dirToMove(entity))
                            {
                                case 0: // Found nothing, move randomly
                                    while (moveRoll > 0)
                                    {
                                        moveRoll = 0;
                                        int moveDir = rand.Next(1, 5); // Get direction

                                        switch (moveDir)
                                        {
                                            case 1: // North
                                                if (map[entity.x, entity.y - 1].type != 1 && overlay[entity.x, entity.y - 1].type != 2 && overlay[entity.x, entity.y - 1].type != 4)
                                                {
                                                    overlay[entity.x, entity.y] = new Entity(playerX, playerY, 0);
                                                    overlay[entity.x, entity.y - 1] = entity;
                                                    entity.y -= 1;
                                                    moveRoll--;
                                                }
                                                else if (overlay[entity.x, entity.y - 1].type == 2)
                                                {
                                                    combat(moveQueue[i], player);
                                                    moveRoll--;
                                                }
                                                break;

                                            case 2: // South
                                                if (map[entity.x, entity.y + 1].type != 1 && overlay[entity.x, entity.y + 1].type != 2 && overlay[entity.x, entity.y + 1].type != 4)
                                                {
                                                    overlay[entity.x, entity.y] = new Entity(playerX, playerY, 0);
                                                    overlay[entity.x, entity.y + 1] = entity;
                                                    entity.y += 1;
                                                    moveRoll--;
                                                }
                                                else if (overlay[entity.x, entity.y + 1].type == 2)
                                                {
                                                    combat(moveQueue[i], player);
                                                    moveRoll--;
                                                }
                                                break;

                                            case 3: // East
                                                if (map[entity.x + 1, entity.y].type != 1 && overlay[entity.x + 1, entity.y].type != 1 && overlay[entity.x + 1, entity.y].type != 4)
                                                {
                                                    overlay[entity.x, entity.y] = new Entity(playerX, playerY, 0);
                                                    overlay[entity.x + 1, entity.y] = entity;
                                                    entity.x += 1;
                                                    moveRoll--;
                                                }
                                                else if (overlay[entity.x + 1, entity.y].type == 2)
                                                {
                                                    combat(moveQueue[i], player);
                                                    moveRoll--;
                                                }
                                                break;

                                            case 4: // West
                                                if (map[entity.x - 1, entity.y].type != 1 && overlay[entity.x - 1, entity.y].type != 2 && overlay[entity.x - 1, entity.y].type != 4)
                                                {
                                                    overlay[entity.x, entity.y] = new Entity(playerX, playerY, 0);
                                                    overlay[entity.x - 1, entity.y] = entity;
                                                    moveRoll--;
                                                    entity.x -= 1;
                                                }
                                                else if (overlay[entity.x - 1, entity.y].type == 2)
                                                {
                                                    combat(moveQueue[i], player);
                                                    moveRoll--;
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case 1: // North
                                    if (map[entity.x, entity.y - 1].type != 1 && overlay[entity.x, entity.y - 1].type != 2 && overlay[entity.x, entity.y - 1].type != 4)
                                    {
                                        overlay[entity.x, entity.y] = new Entity(playerX, playerY, 0);
                                        overlay[entity.x, entity.y - 1] = entity;
                                        entity.y -= 1;
                                        moveRoll--;
                                    }
                                    else if (overlay[entity.x, entity.y - 1].type == 2)
                                    {
                                        combat(moveQueue[i], player);
                                        moveRoll--;
                                    }
                                    break;

                                case 2: // East
                                    if (map[entity.x + 1, entity.y].type != 1 && overlay[entity.x + 1, entity.y].type != 1 && overlay[entity.x + 1, entity.y].type != 4)
                                    {
                                        overlay[entity.x, entity.y] = new Entity(playerX, playerY, 0);
                                        overlay[entity.x + 1, entity.y] = entity;
                                        entity.x += 1;
                                        moveRoll--;
                                    }
                                    else if (overlay[entity.x + 1, entity.y].type == 2)
                                    {
                                        combat(moveQueue[i], player);
                                        moveRoll--;
                                    }
                                    break;

                                case 3: // South
                                    if (map[entity.x, entity.y + 1].type != 1 && overlay[entity.x, entity.y + 1].type != 2 && overlay[entity.x, entity.y + 1].type != 4)
                                    {
                                        overlay[entity.x, entity.y] = new Entity(playerX, playerY, 0);
                                        overlay[entity.x, entity.y + 1] = entity;
                                        entity.y += 1;
                                        moveRoll--;
                                    }
                                    else if (overlay[entity.x, entity.y + 1].type == 2)
                                    {
                                        combat(moveQueue[i], player);
                                        moveRoll--;
                                    }
                                    break;

                                case 4: // West
                                    if (map[entity.x - 1, entity.y].type != 1 && overlay[entity.x - 1, entity.y].type != 2 && overlay[entity.x - 1, entity.y].type != 4)
                                    {
                                        overlay[entity.x, entity.y] = new Entity(playerX, playerY, 0);
                                        overlay[entity.x - 1, entity.y] = entity;
                                        moveRoll--;
                                        entity.x -= 1;
                                    }
                                    else if (overlay[entity.x - 1, entity.y].type == 2)
                                    {
                                        combat(moveQueue[i], player);
                                        moveRoll--;
                                    }
                                    break;
                            }

                        }
                        currentTurn = 0;

                        break;

                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Reads username from settings and updates the highscores with it
        /// </summary>
        /// <param name="currentFloor"></param>
        private void updateHighscore(int currentFloor)
        {
            //NOT IMPLEMENTED

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            if (gameState == GameState.Playing || gameState == GameState.Paused || gameState == GameState.LoadingLevel)
            {
                // iterate through map and overlay and draw them
                for (int i = 0; i < ROOM_WIDTH; i++)
                {
                    for (int j = 0; j < ROOM_HEIGHT; j++)
                    {
                        if (map[i, j].type == 0) // Empty
                        {
                            spriteBatch.Draw(caveFloorTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                        }
                        else if (map[i, j].type == 1) // Wall
                        {
                            spriteBatch.Draw(caveWallTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                        }
                        else if (map[i, j].type == 3) // Exit
                        {
                            spriteBatch.Draw(stairsTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                            if (isLocked)
                            {
                                spriteBatch.Draw(lockTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                            }
                        }
                        else if (map[i, j].type == 9)
                        {
                            spriteBatch.Draw(caveFloorTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                            spriteBatch.Draw(caveDoorTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                        }
                        else if (map[i, j].type == 98) // Rock
                        {
                            spriteBatch.Draw(caveFloorTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                            spriteBatch.Draw(caveRockTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                        }
                        else if (map[i, j].type == 99) // Grass
                        {
                            spriteBatch.Draw(caveFloorTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                            spriteBatch.Draw(caveGrassTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                        }

                        if (overlay[i, j].type == 2) // Player
                        {
                            if (player.sprite != null)
                            {
                                //spriteBatch.Draw(player.sprite, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                                animatedPlayerSprite.Draw(spriteBatch, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)));
                            }
                            else
                            {
                                //spriteBatch.Draw(playerTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                                animatedPlayerSprite.Draw(spriteBatch, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)));
                            }
                            playerX = i;
                            playerY = j;
                        }
                        else if (overlay[i, j].type == 4) // Enemy
                        {
                            //spriteBatch.Draw(enemyTexture, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)), Color.White);
                            if (overlay[i, j].enemyID == 1)
                            {
                                animatedSpriteGreenSlime.Draw(spriteBatch, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)));
                            }
                            else if (overlay[i, j].enemyID == 2)
                            {
                                animatedSpriteRedSlime.Draw(spriteBatch, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)));
                            }
                            else if (overlay[i, j].enemyID == 3)
                            {
                                animatedSpritePurpleSlime.Draw(spriteBatch, new Vector2(0 + (TILE_SIZE * i), 0 + (TILE_SIZE * j)));
                            }
                        }

                    }
                }

                // Draw floor
                spriteBatch.DrawString(font, "Floor: " + currentFloor.ToString(), new Vector2(graphics.PreferredBackBufferWidth - 150, 10), Color.Red);
                if (movesLeft == 4)
                {
                    spriteBatch.Draw(dieFour, new Vector2(5, 50), null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                }
                else if (movesLeft == 3)
                {
                    spriteBatch.Draw(dieThree, new Vector2(5, 50), null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                }
                else if (movesLeft == 2)
                {
                    spriteBatch.Draw(dieTwo, new Vector2(5, 50), null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                }
                else if (movesLeft == 1)
                {
                    spriteBatch.Draw(dieOne, new Vector2(5, 50), null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                }


                // Draw life (Could probably be cleaner...)
                if (player.currentHealth == 6)
                {
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (0 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (45 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (45 * 2), 5), new Color(255, 255, 255, 140));
                }
                else if (player.currentHealth == 5)
                {
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (0 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (45 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartHalfTexture, new Vector2(5 + (45 * 2), 5), new Color(255, 255, 255, 140));
                }
                else if (player.currentHealth == 4)
                {
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (0 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (45 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (45 * 2), 5), new Color(255, 255, 255, 140));
                }
                else if (player.currentHealth == 3)
                {
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (0 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartHalfTexture, new Vector2(5 + (45 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (45 * 2), 5), new Color(255, 255, 255, 140));
                }
                else if (player.currentHealth == 2)
                {
                    spriteBatch.Draw(heartFullTexture, new Vector2(5 + (0 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (45 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (45 * 2), 5), new Color(255, 255, 255, 140));
                }
                else if (player.currentHealth == 1)
                {
                    spriteBatch.Draw(heartHalfTexture, new Vector2(5 + (0 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (45 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (45 * 2), 5), new Color(255, 255, 255, 140));
                }
                else if (player.currentHealth == 0)
                {
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (0 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (45 * 1), 5), new Color(255, 255, 255, 140));
                    spriteBatch.Draw(heartEmptyTexture, new Vector2(5 + (45 * 2), 5), new Color(255, 255, 255, 140));
                }

                // Combat screen
                if (isRollingOpen)
                {
                    if (player.y * TILE_SIZE > graphics.PreferredBackBufferHeight)
                    {
                        spriteBatch.Draw(rollPanelTexture, new Vector2(player.x * TILE_SIZE - 50, player.y * TILE_SIZE - 30), Color.White);
                    }
                    else if (player.y * TILE_SIZE < graphics.PreferredBackBufferHeight)
                    {
                        spriteBatch.Draw(rollPanelTexture, new Vector2(player.x * TILE_SIZE - 50, player.y * TILE_SIZE + 30), Color.White);
                    }

                    for (int i = 0; i < combatRollsOne.Count; i++)
                    {
                        switch (combatRollsOne[i])
                        {
                            case 1:
                                spriteBatch.Draw(dieOne, new Vector2(player.x * TILE_SIZE - 40 + (25 * i), player.y * TILE_SIZE + 40), Color.White);
                                break;
                            case 2:
                                spriteBatch.Draw(dieTwo, new Vector2(player.x * TILE_SIZE - 40 + (25 * i), player.y * TILE_SIZE + 40), Color.White);
                                break;
                            case 3:
                                spriteBatch.Draw(dieThree, new Vector2(player.x * TILE_SIZE - 40 + (25 * i), player.y * TILE_SIZE + 40), Color.White);
                                break;
                            case 4:
                                spriteBatch.Draw(dieFour, new Vector2(player.x * TILE_SIZE - 40 + (25 * i), player.y * TILE_SIZE + 40), Color.White);
                                break;
                        }
                    }
                    for (int i = 0; i < combatRollsTwo.Count; i++)
                    {
                        switch (combatRollsTwo[i])
                        {
                            case 1:
                                spriteBatch.Draw(dieOne, new Vector2(player.x * TILE_SIZE + 60 + (25 * i), player.y * TILE_SIZE + 40), Color.White);
                                break;
                            case 2:
                                spriteBatch.Draw(dieTwo, new Vector2(player.x * TILE_SIZE + 60 + (25 * i), player.y * TILE_SIZE + 40), Color.White);
                                break;
                            case 3:
                                spriteBatch.Draw(dieThree, new Vector2(player.x * TILE_SIZE + 60 + (25 * i), player.y * TILE_SIZE + 40), Color.White);
                                break;
                            case 4:
                                spriteBatch.Draw(dieFour, new Vector2(player.x * TILE_SIZE + 60 + (25 * i), player.y * TILE_SIZE + 40), Color.White);
                                break;
                        }
                    }

                }


                // IDEAS
                /*
                 * 
                 * Could make predetermined rooms
                 * - Check top left corner, then count right and down from that point, every tile, to ensure there is room (not already taken)
                 * - Could make the outsides of those rooms unbreakable by assigning a value like 9 and making the gen not go through those
                 * 
                 * Doors
                 * - Just check to make sure both sides have a solid wall
                 * 
                 * Make start position the exit of the previous level
                 * 
                 */

                if (gameState == GameState.LoadingLevel)
                {
                    spriteBatch.Draw(fade, new Vector2(5, 50), null, Color.White, 0f, Vector2.Zero, 300f, SpriteEffects.None, 0f);
                }

                if (gameState == GameState.Paused)
                {
                    //spriteBatch.DrawString(font, "TEST", new Vector2(0, 0), Color.Red);
                    spriteBatch.Draw(menuBackground, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), null, Color.White, 0f, new Vector2(menuBackground.Width / 2, menuBackground.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                    spriteBatch.Draw(diceLabel, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 - 170), null, Color.White, 0f, new Vector2(diceLabel.Width / 2, diceLabel.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                    spriteBatch.Draw(diceAttackLabel, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 - 130), null, Color.White, 0f, new Vector2(diceAttackLabel.Width / 2, diceAttackLabel.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                    spriteBatch.Draw(diceMoveLabel, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2 + 30), null, Color.White, 0f, new Vector2(diceMoveLabel.Width / 2, diceMoveLabel.Height / 2), Vector2.One, SpriteEffects.None, 0f);

                    // Draw the current dice

                    for (int i = 0; i < player.dice.Length; i++)
                    {

                        for (int j = 0; j < player.dice[i].sides.Length; j++)
                        {
                            Vector2 location = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuBackground.Width / 4) + (23 * j), graphics.PreferredBackBufferHeight / 2 - (menuBackground.Height / 4) + (30 * i));
                            switch (player.dice[i].sides[j])
                            {
                                case 1:
                                    spriteBatch.Draw(dieOne, location, null, Color.White, 0f, new Vector2(dieOne.Width / 2, dieOne.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                                    break;
                                case 2:
                                    spriteBatch.Draw(dieTwo, location, null, Color.White, 0f, new Vector2(dieOne.Width / 2, dieOne.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                                    break;
                                case 3:
                                    spriteBatch.Draw(dieThree, location, null, Color.White, 0f, new Vector2(dieOne.Width / 2, dieOne.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                                    break;
                                case 4:
                                    spriteBatch.Draw(dieFour, location, null, Color.White, 0f, new Vector2(dieOne.Width / 2, dieOne.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                                    break;
                            }
                        }
                    }

                    for (int i = 0; i < player.moveDie.sides.Length; i++)
                    {
                        Vector2 location = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuBackground.Width / 4) + (23 * i), graphics.PreferredBackBufferHeight / 2 + 60);
                        switch (player.moveDie.sides[i])
                        {
                            case 1:
                                spriteBatch.Draw(dieOne, location, null, Color.White, 0f, new Vector2(dieOne.Width / 2, dieOne.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                                break;
                            case 2:
                                spriteBatch.Draw(dieTwo, location, null, Color.White, 0f, new Vector2(dieOne.Width / 2, dieOne.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                                break;
                            case 3:
                                spriteBatch.Draw(dieThree, location, null, Color.White, 0f, new Vector2(dieOne.Width / 2, dieOne.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                                break;
                            case 4:
                                spriteBatch.Draw(dieFour, location, null, Color.White, 0f, new Vector2(dieOne.Width / 2, dieOne.Height / 2), Vector2.One, SpriteEffects.None, 0f);
                                break;
                        }
                    }

                }
            }
            else if (gameState == GameState.Menu)
            {
                spriteBatch.Draw(mainMenuTexture, new Vector2(0, 0), Color.White);

                if (cursorPos == 1)
                {
                    spriteBatch.Draw(cursorTexture, new Vector2(305, 415), Color.White);
                    ;
                }
                else if (cursorPos == 2)
                {
                    spriteBatch.Draw(cursorTexture, new Vector2(305, 415 + (40)), Color.White);
                }
                else if (cursorPos == 3)
                {
                    spriteBatch.Draw(cursorTexture, new Vector2(305, 415 + (80)), Color.White);
                }
                else if (cursorPos == 4)
                {
                    spriteBatch.Draw(cursorTexture, new Vector2(305, 415 + (120)), Color.White);
                }
                else if (cursorPos == 5)
                {
                    spriteBatch.Draw(cursorTexture, new Vector2(305, 415 + (160)), Color.White);
                }

            }
            else if (gameState == GameState.Help)
            {
                spriteBatch.Draw(helpMenuTexture, new Vector2(0, 0), Color.White);
            }
            else if (gameState == GameState.About)
            {
                spriteBatch.Draw(aboutMenuTexture, new Vector2(0, 0), Color.White);
            }
            else if (gameState == GameState.Highscore)
            {
                spriteBatch.Draw(highscoreMenuTexture, new Vector2(0, 0), Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Prints the grid to the debug output (FOR DEBUGGING PURPOSES)
        /// </summary>
        /// <param name="toPrint">Array to print</param>
        private void printGrid(Tile[,] toPrint)
        {
            Debug.Write(Environment.NewLine);
            for (int i = 0; i < toPrint.GetLength(0); i++)
            {
                for (int j = 0; j < toPrint.GetLength(1); j++)
                {
                    Debug.Write(toPrint[i, j].type);
                }
                Debug.Write(Environment.NewLine);
            }
        }

        /// <summary>
        /// Generates the map based on the provided parameters.
        /// Currently uses custom room algorithm to generate.
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        /// <param name="type">Type of room gen to use</param>
        /// <param name="numOfRooms">Desired number of rooms</param>
        /// <param name="numOfEnemies">Number of enemies</param>
        /// <param name="notUSED">Used to make it different from other room gen TEMPORARY</param>
        /// <returns></returns>
        private Tile[,] GenerateMap(int width, int height, int type, int numOfRooms, int numOfEnemies, int notUSED)
        {

            /*
             * 
             * 2 ways of going about this...
             * 1. Generate all the rooms first than make corridors
             * 2. Generate room, then corridor, then room, etc.
             * 
             * UPDATE: Randomly choose if room will have treasure when made, up to a max determined somewhere.
             * 
             * THIS WILL USE #2
             * 
             */

            // Initialize grid as solid
            // Put a border around the entire array
            Tile[,] grid = new Tile[width, height];
            overlay = new Entity[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = new Tile(i, j, 1);
                    overlay[i, j] = new Entity(i, j, 0);
                }
            }

            int currentRooms = 0;
            int xPos, yPos;
            int xSize, ySize;

            List<Room> rooms = new List<Room>(); // Holds all of our rooms - xPos, yPos, xSize, ySize

            int MIN_SIZE = 3;
            int MAX_SIZE = 10;

            xPos = rand.Next(1, width);
            yPos = rand.Next(1, height);
            xSize = rand.Next(MIN_SIZE, MAX_SIZE);
            ySize = rand.Next(MIN_SIZE, MAX_SIZE);

            // Check if the room would stretch beyond the x boundary, if so fix that with math!
            if (xPos + xSize > width)
            {
                xSize = width - xPos - 1;
            }

            // Check if the room would stretch beyond the y boundary, if so fix that with math!
            if (yPos + ySize > height)
            {
                ySize = height - yPos - 1;
            }

            // Make the room!
            for (int i = xPos; i < xPos + xSize; i++)
            {
                for (int j = yPos; j < yPos + ySize; j++)
                {
                    //Debug.WriteLine("i: " + i + " j: " + j);
                    grid[i, j].type = 0;
                }
            }

            // Add the room to the list of rooms
            rooms.Add(new Room(xPos, yPos, xSize, ySize));

            int startX;
            int startY;

            currentRooms++;

            // While there is still rooms to be made 
            while (currentRooms < numOfRooms)
            {

                int randomRoom = rand.Next(0, rooms.Count); // Get a random room from the list

                bool roomCreated = false;

                while (!roomCreated)
                {

                    int x = rand.Next(rooms[randomRoom].xPos, rooms[randomRoom].xPos + rooms[randomRoom].xSize);
                    int y = rand.Next(rooms[randomRoom].yPos, rooms[randomRoom].yPos + rooms[randomRoom].ySize);


                    // Determine a wall that is on border of room
                    if ((grid[x, y].type == 0 && grid[x - 1, y].type == 1)) // Wall to left
                    {
                        Debug.WriteLine("LEFT");
                        startX = x - 1;
                        startY = y;

                        // Get a random length and height for the corridor
                        int randomCorridorLength = rand.Next(3, 11); // UPDATE: Change this to use global variable lengths
                        int randomCorridorHeight = rand.Next(1, 4); // UPDATE: Same here

                        // Make sure we don't go over the boundaries
                        if (startX - randomCorridorLength < 0)
                        {
                            randomCorridorLength = 1 + startX;
                        }

                        for (int i = startX; i > randomCorridorLength - startX; i--)
                        {
                            for (int j = startY; j < randomCorridorHeight; j++)
                            {
                                grid[startX - i, startY + j].type = 0;
                                roomCreated = true;
                            }
                        }

                    }
                    if ((grid[x, y].type == 0 && grid[x + 1, y].type == 1))  // Wall to the right
                    {
                        Debug.WriteLine("RIGHT");
                        startX = x + 1;
                        startY = y;

                        // Get a random length and height for the corridor
                        int randomCorridorLength = rand.Next(3, 11); // UPDATE: Change this to use global variable lengths
                        int randomCorridorHeight = rand.Next(1, 4); // UPDATE: Same here

                        // Make sure we don't go over the boundaries
                        if (startX + randomCorridorLength > width)
                        {
                            randomCorridorLength = width - startX - 1;
                        }

                        for (int i = startX; i < randomCorridorLength + startX; i++)
                        {
                            for (int j = startY; j < randomCorridorHeight + startY; j++)
                            {
                                grid[startX + i, startY + j].type = 0;
                                roomCreated = true;
                            }
                        }

                    }

                    if ((grid[x, y].type == 0 && grid[x, y - 1].type == 1)) // Wall to the north
                    {
                        Debug.WriteLine("NORTH");
                        startX = x;
                        startY = y - 1;

                        // Get a random length and height for the corridor
                        int randomCorridorLength = rand.Next(3, 11); // UPDATE: Change this to use global variable lengths
                        int randomCorridorHeight = rand.Next(1, 4); // UPDATE: Same here

                        // Make sure we don't go over the boundaries
                        if (startY - randomCorridorLength < 0)
                        {
                            randomCorridorLength = 1 + startY;
                        }

                        for (int i = startX; i < randomCorridorHeight; i++)
                        {
                            for (int j = startY; j > randomCorridorLength; j--)
                            {
                                grid[startX + i, startY - j].type = 0;
                                roomCreated = true;
                            }
                        }

                    }

                    if ((grid[x, y].type == 0 && grid[x, y + 1].type == 1)) // Wall to the south
                    {
                        Debug.WriteLine("SOUTH");
                        startX = x;
                        startY = y + 1;

                        // Get a random length and height for the corridor
                        int randomCorridorLength = rand.Next(3, 11); // UPDATE: Change this to use global variable lengths
                        int randomCorridorHeight = rand.Next(1, 4); // UPDATE: Same here

                        // Make sure we don't go over the boundaries
                        if (startY + randomCorridorLength > height)
                        {
                            randomCorridorLength = width - startY - 1;
                        }

                        for (int i = startX; i < randomCorridorLength; i++)
                        {
                            for (int j = startY; j < randomCorridorHeight; j++)
                            {
                                grid[startX + i, startY + j].type = 0;
                                roomCreated = true;
                            }
                        }

                    }

                }
                currentRooms++;
                // Whether it creates a corridor or a room
                int random = rand.Next(1, 4);

                if (random < 3)
                {
                    // Get random position for a corridor


                    /*Debug.WriteLine("X: " + x + " Y: " + y);
                    
                    if ((grid[x, y].type == 1 && grid[x - 1, y].type == 0) ||
                        (grid[x, y].type == 1 && grid[x + 1, y].type == 0) ||
                        (grid[x, y].type == 1 && grid[x, y - 1].type == 0) ||
                        (grid[x, y].type == 1 && grid[x, y + 1].type == 0))
                    {
                        overlay[x, y].type = 4;
                    }*/

                }

            }

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
                if (!gameInProgress && grid[randX, randY].type == 0) // New game
                {
                    // Place the player
                    playerPlaced = true;
                    overlay[randX, randY] = new Player(randX, randY, 2, 6, 2, new Dice[3], playerSpritesheet);
                    player = (Player)overlay[randX, randY];

                }
                else if (gameInProgress && grid[randX, randY].type == 0) // New level
                {
                    // Place the player
                    playerPlaced = true;
                    overlay[randX, randY] = player;
                    player = (Player)overlay[randX, randY];
                }


            }

            gameInProgress = true; // Make sure to not reset the player when the game is in progress

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

            return grid;
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
            overlay = new Entity[width, height];

            moveQueue.Clear();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = new Tile(i, j, 1);
                    overlay[i, j] = new Entity(i, j, 0);
                }
            }

            int currentStep = 0;
            bool moved = false;

            // 1. Define a start point, in this case the middle, and set it to empty (0)
            int x = width / 2;
            int y = height / 2;



            /*
             * ----------MAP GEN------------
             */


            //printGrid(grid);
            //Debug.WriteLine("Current Step: " + currentStep + " X: " + x + " Y: " + y);



            // NORMAL
            if (type == 0)
            {
                grid[x, y].type = 0;
                // While we still have steps to make
                while (currentStep < steps)
                {

                    // 2. Get a random number to represent direction
                    int dir = rand.Next(1, 5); // Between 1 and 4

                    // 3. Move in direction
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
            }
            else if (type == 1)
            {
                grid[x, y].type = 0;
                // While we still have steps to make
                while (currentStep < steps)
                {

                    int dirJustMoved = 0;
                    if (!moved)
                    {

                        // 2. Get a random number to represent direction
                        int dir = rand.Next(1, 5); // Between 1 and 4
                        dirJustMoved = dir;

                        // 3. Move in direction
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
                if (!gameInProgress && grid[randX, randY].type == 0)
                {
                    // Place the player
                    playerPlaced = true;

                    Dice die = new Dice(new int[6] { 1, 1, 2, 2, 3, 3 });

                    overlay[randX, randY] = new Player(randX, randY, 2, 6, 2, new Dice[3] { die, die, die }, playerSpritesheet);
                    player = (Player)overlay[randX, randY];
                    // Add the player to the move queue, always ensures that they are first on every floor
                    moveQueue.Add(player);
                    gameInProgress = true;

                    // Give the player 3 new dice IF A NEW GAME
                    if (!gameInProgress)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            player.setDice(i, new Dice());
                        }
                    }
                }
                else if (gameInProgress && grid[randX, randY].type == 0)
                {
                    // Place the player
                    playerPlaced = true;

                    overlay[randX, randY] = player;
                    player.x = randX;
                    player.y = randY;
                    // Add the player to the move queue, always ensures that they are first on every floor
                    moveQueue.Add(player);
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
                    // Place the enemy

                    //Choose random type
                    if (currentFloor < 3)
                    {
                        overlay[randX, randY] = new Slime(randX, randY, 4, 1, 0, new Dice[3], playerSpritesheet);
                        overlay[randX, randY].isEnemy = true;
                        overlay[randX, randY].viewDistance = 8;
                        overlay[randX, randY].name = "Slime" + enemies;
                        enemies++;
                        enemiesRemaining++;
                        // Add it to the moveQueue in the order they are created
                        moveQueue.Add(overlay[randX, randY]);
                    }
                    else if (currentFloor < 5)
                    {
                        int enemyID = rand.Next(1, 4); // Get random enemy

                        if (enemyID == 1)
                        {
                            overlay[randX, randY] = new Slime(randX, randY, 4, 1, 0, new Dice[3], playerSpritesheet);
                            overlay[randX, randY].isEnemy = true;
                            overlay[randX, randY].viewDistance = 8;
                            overlay[randX, randY].name = "Slime" + enemies;
                            enemies++;
                            enemiesRemaining++;
                            // Add it to the moveQueue in the order they are created
                            moveQueue.Add(overlay[randX, randY]);
                        }
                        else if (enemyID == 2)
                        {
                            overlay[randX, randY] = new RedSlime(randX, randY, 4, 2, 0, new Dice[3], playerSpritesheet);
                            overlay[randX, randY].isEnemy = true;
                            overlay[randX, randY].viewDistance = 8;
                            overlay[randX, randY].name = "RedSlime" + enemies;
                            enemies++;
                            enemiesRemaining++;
                            // Add it to the moveQueue in the order they are created
                            moveQueue.Add(overlay[randX, randY]);
                        }
                    }
                    else if (currentFloor > 4)
                    {
                        int enemyID = rand.Next(1, 4); // Get random enemy

                        if (enemyID == 1)
                        {
                            overlay[randX, randY] = new Slime(randX, randY, 4, 1, 0, new Dice[3], playerSpritesheet);
                            overlay[randX, randY].isEnemy = true;
                            overlay[randX, randY].viewDistance = 8;
                            overlay[randX, randY].name = "Slime" + enemies;
                            enemies++;
                            enemiesRemaining++;
                            // Add it to the moveQueue in the order they are created
                            moveQueue.Add(overlay[randX, randY]);
                        }
                        else if (enemyID == 2)
                        {
                            overlay[randX, randY] = new RedSlime(randX, randY, 4, 2, 0, new Dice[3], playerSpritesheet);
                            overlay[randX, randY].isEnemy = true;
                            overlay[randX, randY].viewDistance = 8;
                            overlay[randX, randY].name = "RedSlime" + enemies;
                            enemies++;
                            enemiesRemaining++;
                            // Add it to the moveQueue in the order they are created
                            moveQueue.Add(overlay[randX, randY]);
                        }
                        else if (enemyID == 3)
                        {
                            overlay[randX, randY] = new purpleSlime(randX, randY, 4, 3, 0, new Dice[3], playerSpritesheet);
                            overlay[randX, randY].isEnemy = true;
                            overlay[randX, randY].viewDistance = 8;
                            overlay[randX, randY].name = "PurpleSlime" + enemies;
                            enemies++;
                            enemiesRemaining++;
                            // Add it to the moveQueue in the order they are created
                            moveQueue.Add(overlay[randX, randY]);
                        }
                    }


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
            printGrid(grid);

            gameState = GameState.Playing;

            return grid;

            //UPDATE: Generate random square rooms, save their top left and bottom right coords
            //        Select a random wall to start corridor, then use coords to select random range
            //        of another room, you can determine if it is above, below, left, or right,
            //        then draw a corridor to somewhere in that range
            //
            //        Another way is generate random room, then select random wall. Generate a random
            //        length corridor from that. At end of corridor, do random chance to produce room or another
            //        corridor in a direction.

        }

        //DICE ROLLS

        /// <summary>
        /// Initiates a dice roll combat between two entities
        /// </summary>
        /// <param name="entity1">Attacking Entity</param>
        /// <param name="entity2">Defending Entity</param>
        public void combat(Entity entity1, Entity entity2)
        {

            sfi = diceRoll.CreateInstance();
            sfi.Play();

            combatRollsOne.Clear();
            combatRollsTwo.Clear();

            int entityOneRoll = 0;
            int entityTwoRoll = 0;

            isRollingOpen = true;

            // Entity one roll
            foreach (Dice die in entity1.dice)
            {
                int randomRoll = rand.Next(0, 6); // Roll between 1 and 6
                entityOneRoll += die.sides[randomRoll];
                combatRollsOne.Add(die.sides[randomRoll]);
            }

            // Entity two roll
            foreach (Dice die in entity2.dice)
            {
                int randomRoll = rand.Next(0, 6); // Roll between 1 and 6
                entityTwoRoll += die.sides[randomRoll];
                combatRollsTwo.Add(die.sides[randomRoll]);
            }

            // Entity One wins
            if (entityOneRoll > entityTwoRoll)
            {
                entity2.currentHealth--;

                if (entity2.currentHealth < 1)
                {
                    //sfi = slimeDeath.CreateInstance();
                    //sfi.Play();

                    enemiesRemaining--;
                    moveQueue.Remove(entity2);
                    overlay[entity2.x, entity2.y] = new Entity(entity2.x, entity2.y, 0);


                }
            }

        }

        /// <summary>
        /// Calculates the direction an entity should travel to get to a player
        /// </summary>
        /// <param name="entity">Entity to get direction for</param>
        /// <returns></returns>
        public int dirToMove(Entity entity)
        {
            int x = entity.x;
            int y = entity.y;

            int distX = 0;
            int distY = 0;

            int xDir = 0;
            int yDir = 0;

            // Calculate distX
            if (player.x > x) // East
            {
                distX = player.x - x;
                xDir = 2;
            }
            else if (player.x < x) // West
            {
                distX = x - player.x;
                xDir = 4;
            }

            // Calculate distY
            if (player.y < y) // North
            {
                distY = y - player.y;
                yDir = 1;
            }
            else if (player.y > y) // South
            {
                distY = player.y - y;
                yDir = 2;
            }

            bool isPlayerFound = false;

            foreach (var point in searchTiles(x, y, entity.viewDistance)) // Check every tile in view distance for the player
            {
                //Debug.WriteLine("X: " + point.x + " Y: " + point.y + " | PX: " + player.x + " PY: "+ player.y);
                if (point.x == player.x && point.y == player.y) // If a player is in the view distance
                {
                    isPlayerFound = true;
                }
            }



            if (isPlayerFound)
            {
                if (distY != 0 && distX < distY)
                {
                    return yDir;
                }
                else if (distX != 0 && distY > distX)
                {
                    return xDir;
                }
                else
                {
                    return xDir;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Collects every tile within R radius from X, Y origin
        /// </summary>
        /// <param name="x">X coordinate of origin</param>
        /// <param name="y">Y cooridinate of origin</param>
        /// <param name="r">Radius of circle</param>
        /// <returns></returns>
        public Tile[] searchTiles(int x, int y, int r)
        {
            HashSet<Tile> points = new HashSet<Tile>(); // HashSet is a collection of UNIQUE objects --- ADAPTED FROM THIS JAVA CODE https://gamedev.net/forums/topic/671422-find-tile-coordinates-within-a-given-radius/5250074/
            Vector2 origin = new Vector2(x, y); // Origin position

            for (int i = x - r; i <= x + r; i++) // i = starting X coord - radius; i <= starting X coord + radius; i++; Essentially goes through the entire radius on X axis
            {
                for (int j = y - r; j <= y + r; j++) // j = starting Y coord - radius; j <= starting Y coord + radius; j++; Essentially goes through the entire radius on Y axis
                {
                    Tile currentTile = new Tile(i, j, 0);

                    if (Vector2.Distance(new Vector2(currentTile.x, currentTile.y), origin) <= r) // If the distance between the current tile and the origin is <= radius, we are in the circle
                    {
                        points.Add(currentTile); // Add the current tile to the point list
                    }
                }
            }

            Tile[] tiles = points.ToArray();

            return tiles;
        }
    }
}
