//Gamescreens were made with resourced fonts of textcraft.net
//Soundeffects and original music made with Ableton 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Audio;
using System.Threading;

/// <summary>
/// A class for representing the game world.
/// This contains the grid, the falling block, and everything else that the player can see/do.
/// </summary>
class GameWorld
{
    /// <summary>
    /// An enum for the different game states that the game can have.
    /// </summary>
    enum GameState
    {
        Playing,
        GameOver,
        Pause,
        Start,
        Instructions
    }

    /// <summary>
    /// The main font of the game.
    /// </summary>
    SpriteFont font;

    /// <summary>
    /// The current game state.
    /// </summary>
    GameState gameState;

    public KeyboardState currentKeyboardState, previousKeyboardState;
    Texture2D TetrisStart, TetrisPause, TetrisGameOver, TetrisInstruction;
    SoundEffect LevelUp, RowSound;

    /// <summary>
    /// The main grid of the game.
    /// </summary>
    TetrisGrid grid;
    /// <summary>
    /// The current frame
    /// </summary>
    int ticks;
    int time;

    /// <summary>
    /// The active falling block;
    /// </summary>
    static Block currentBlock, nextblock;
    /// <summary>
    /// The blocks position in the grid
    /// </summary>
    public Vector2 currentblockPositon;

    //level and scores of player
    public int score, targetscore;
    int level;
    string Tscore, Lscore;
    public bool levelup;
    //N//

    public GameWorld()
    {

        //game starts at beginning screen
        gameState = GameState.Start;

        //load sprites and sounds
        LevelUp = TetrisGame.ContentManager.Load<SoundEffect>("TetrisLevelUp");
        RowSound = TetrisGame.ContentManager.Load<SoundEffect>("TetrisRowSound");
        font = TetrisGame.ContentManager.Load<SpriteFont>("SpelFont");
        TetrisStart = TetrisGame.ContentManager.Load<Texture2D>("TetrisStart");
        TetrisPause = TetrisGame.ContentManager.Load<Texture2D>("TetrisPause");
        TetrisGameOver = TetrisGame.ContentManager.Load<Texture2D>("TetrisGameOver");
        TetrisInstruction = TetrisGame.ContentManager.Load<Texture2D>("TetrisInstructions");

        //create the grid
        grid = new TetrisGrid();

        //30 fps
        time = 30;
        ticks = -time;

        //begin score, level and targetscore
        score = 0;
        level = 1;
        targetscore = 250;
    }

    public void Update(GameTime gameTime)
    {
        //get state of the keyboard
        KeyboardState keyBoardState = Keyboard.GetState();

        //if the playing the game, do this logic
        if (gameState == GameState.Playing)
        {
            //strings of the scores and levels
            Tscore = score.ToString();
            Lscore = level.ToString();

            //check if player pressed something
            InputDetection();

            //if there is no block, create one and manage the level
            if (currentBlock == null)
            {
                makeNewBlock();
                Levels();
            }

            if (ticks >= time)
            {
                if (TryPlaceBlock())
                    CheckFullRow();

                // moves the current block down on the grid
                currentblockPositon.Y++; 

                //if the player leveled up, higher the targetscore and add to the level
                if (levelup == true)
                {
                    time -= 4; //makes block fall faster
                    Console.WriteLine("levelup");
                    level++;
                    targetscore *= 2;
                    LevelUp.Play();
                    levelup = false;

                }

                ticks = -time; //reset frame counter
            }
            else
            {
                ticks++; // adds to counter 
            }
        }


        //if the game is on starting screen, help player switch to instructions or playing state
        if (gameState == GameState.Start)
        {
            if (keyBoardState.IsKeyDown(Keys.Enter))
            {
                gameState = GameState.Playing;
            }
            if (keyBoardState.IsKeyDown(Keys.I))
            {
                gameState = GameState.Instructions;
            }
        }

        //press enter to go to playing screen when paused
        if (gameState == GameState.Pause)
        {
            if (keyBoardState.IsKeyDown(Keys.Enter))
            {
                gameState = GameState.Playing;
            }
        }

        //if the player is gameover..
        if (gameState == GameState.GameOver)
        {
            if (keyBoardState.IsKeyDown(Keys.Enter))
                Reset();


            if (keyBoardState.IsKeyDown(Keys.M))
            {
                Reset();
                gameState = GameState.Start;
            }
        }

        //instruction screen
        if (gameState == GameState.Instructions)
        {
            if (keyBoardState.IsKeyDown(Keys.B))
            {
                gameState = GameState.Start;
            }
        }

    }

    //detect the input of the player
    private void InputDetection()
    {
        previousKeyboardState = currentKeyboardState;
        currentKeyboardState = Keyboard.GetState();
        KeyboardState keyBoardState = Keyboard.GetState();

        if (currentBlock != null && currentblockPositon.Y > -1) // can only move block when it is in a valid grid pos
        {
            //movements
            if (currentKeyboardState.IsKeyDown(Keys.Right) && previousKeyboardState.IsKeyUp(Keys.Right)) //move right
            {
                var potentialpos = currentblockPositon;
                potentialpos.X += 1;
                if (!CheckAnyCollison(potentialpos)) //if there is not a possible collission, you can move.
                    currentblockPositon = potentialpos;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left) && previousKeyboardState.IsKeyUp(Keys.Left)) //move left
            {
                var potentialpos = currentblockPositon;
                potentialpos.X -= 1;
                if (!CheckAnyCollison(potentialpos))
                    currentblockPositon = potentialpos;
            }

            if (currentKeyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyUp(Keys.A)) //rotate to the right if you can rotate, otherwise, rotate back
            {
                currentBlock.RotateRight();
                if (CheckAnyCollison(currentblockPositon))
                    currentBlock.RotateLeft();
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyUp(Keys.D)) //same but for rotating left
            {
                currentBlock.RotateLeft();
                if (CheckAnyCollison(currentblockPositon))
                    currentBlock.RotateRight();
            }
        }

        if (currentKeyboardState.IsKeyDown(Keys.Down) && previousKeyboardState.IsKeyUp(Keys.Down)) //move down one block when pressed
        {
            ticks = time;
        }
        if (currentKeyboardState.IsKeyDown(Keys.Space)) //move down the block fast
        {
            ticks = time;
        }

        if (keyBoardState.IsKeyDown(Keys.P)) //pause the game
        {
            gameState = GameState.Pause;
        }


    }

    bool TryPlaceBlock() //try placing a block
    {

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (currentBlock != null && currentBlock.currentShape[i, j] == true)
                {
                    var x = i + (int)currentblockPositon.X;
                    var y = j + (int)currentblockPositon.Y;
                    if (y == grid.gHeight - 1 || grid.gridData[x, y + 1].hasblock) // bottom of grid or below
                    {
                        if (currentblockPositon.Y == 0 || currentblockPositon.Y == -1) //if its top of the grid, its agmeover.
                        {
                            gameState = GameState.GameOver;
                        }
                        else //if a block can be placed, place it and higher the score.
                        {
                            grid.AddPlacedBlock(currentBlock, currentblockPositon);
                            currentBlock = null;
                            score += 75;
                            return true;
                        }

                    }

                }
            }
        }
        return false;
    }

    bool CheckAnyCollison(Vector2 blockposition) //check for collission left or right.
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (currentBlock != null && currentBlock.currentShape[i, j] == true)
                {
                    var x = i + (int)blockposition.X;
                    var y = j + (int)blockposition.Y;

                    if (x >= grid.gWidth || x <= -1 || y >= grid.gHeight) // right,left
                    {
                        return true;
                    }// check bounds for height or width
                    else if (grid.gridData[x, y].hasblock)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void CheckFullRow()
    {

        int bonuspoint = 0;
        for (int i = 0; i < grid.gHeight; i++)
        {
            //  Console.WriteLine(fullrow);
            int fullrow = 0;
            for (int j = 0; j < grid.gWidth; j++)
            {
                //Console.WriteLine($" j is {j}");
                if (grid.gridData[j, i].hasblock) // check full row
                {
                    fullrow++;
                    if (fullrow == grid.gWidth) // full row
                    {
                        grid.RemoveBlockRow(i);
                        grid.ShiftBlocksDown(i-1); // shifting all blocks that are ABOVE the completed row
                        score += 150;
                        bonuspoint++;
                        RowSound.Play();
                    }
                }
            }
        }
        if(bonuspoint > 1)
        {
            score += 50; //give bonuspoints when player makes multiple fullrows
            //Console.WriteLine("bonuspoints given");
        }
    }

    //create a new block that will fall
    void makeNewBlock()
    {
        if (nextblock == null && currentBlock == null) // start of the game
        {
            currentBlock = new Block();
            nextblock = new Block();
        }
        else if (nextblock != null && currentBlock == null) // after block placement
        {
            currentBlock = nextblock;
            nextblock = new Block();
        }
        currentblockPositon = Vector2.Zero; //resets overall position for next block;
        currentblockPositon.Y = -1; // reset blocks position so that first tick is 0
    }


    void Levels() //if player's score hits targetscore, its a level up.
    {
        if(score >= targetscore)
        {
            levelup = true;
        }
    }

    //draw the logic on the screen
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice)
    {
        spriteBatch.Begin();
       
        //draw this when its the beginning screen
        if (gameState == GameState.Start)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(TetrisStart, new Vector2(241, 200), Color.White);
        }

        //if the gamestate is playing, draw all the necessary logic
        if (gameState == GameState.Playing)
        {
            GraphicsDevice.Clear(Color.Black);
            grid.Draw(gameTime, spriteBatch);
            if (currentBlock != null) //if there is a block on the screen
            {
                if (currentblockPositon.Y > -1) //and its on the grid
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                             if (currentBlock.currentShape[i, j] == true) //if the shape matrix has a block, so a true, draw it.
                            {
                                 int offset = j + (int)currentblockPositon.Y;
                                 spriteBatch.Draw(Block.sprite, grid.Gridpositions[i + (int)currentblockPositon.X, offset], currentBlock.CurrentColor);

                             }
                        }
                    }
                }
            }

             if (nextblock != null) //if there is a next block made, draw it on the screen
            {
                 Vector2 pos = new Vector2();
                 pos = new Vector2(400, 200);
                 for (int i = 0; i < 4; i++)
                 {
                     for (int j = 0; j < 4; j++)
                     {
                         if (nextblock.currentShape[i, j] == true)
                         {
                            spriteBatch.Draw(Block.sprite, new Vector2(pos.X + i * 30, pos.Y + j * 30), nextblock.CurrentColor);
                         }
                     }
                 }
             }

            //drawing all the scores and levels on the screen
            spriteBatch.DrawString(font, "Level: " + Lscore, new Vector2(400, 40), Color.Yellow);
            spriteBatch.DrawString(font, "Score: " + Tscore, new Vector2(400, 66), Color.Yellow);
            spriteBatch.DrawString(font, "Next:", new Vector2(400, 92), Color.Yellow);
        }

        //if the gamestate is paused, draw the pause screen
        else if (gameState == GameState.Pause)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(TetrisPause, new Vector2(450, 300) / 2, Color.White);
        }

        //instruction game screen
        else if (gameState == GameState.Instructions)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(TetrisInstruction, new Vector2(145, 25), Color.White);
        }

        //if gameover, draw the game over screen, final score and reset the grid (So all empty)
        else if (gameState == GameState.GameOver)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(TetrisGameOver, new Vector2(300, 300) / 2, Color.White);
            spriteBatch.DrawString(font, "Final Score: " + Tscore, new Vector2(350, 300), Color.White);
            grid.ResetGrid();
        }

        spriteBatch.End();
    }

    //reset the whole game and go back to playing
    public void Reset()
    {
        score = 0;
        level = 1;
        gameState = GameState.Playing;
        targetscore = 250;
        time = 30;
    }


}
