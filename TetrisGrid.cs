using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Threading;


/// <summary>
/// A class for representing the Tetris playing grid.
/// </summary>
public class TetrisGrid
{
    /// The sprite of a single empty cell in the grid.
    Texture2D emptySprite;

    /// The position at which this TetrisGrid should be drawn.
    Vector2 position;
    public Vector2 posblock;

    /// The number of grid elements in the x-direction.
    public int gWidth { get { return 10; } }

    /// The number of grid elements in the y-direction.
    public int gHeight { get { return 20; } }

    //data of a block and positions of the grid
    public BlockData[,] gridData, fullRow;
    public Vector2[,] Gridpositions { get; private set; }

    /// <param name="b"></param>
    public TetrisGrid()
    {
        emptySprite = TetrisGame.ContentManager.Load<Texture2D>("tetrisbl");
        Gridpositions = new Vector2[gWidth, gHeight];
        gridData = new BlockData[gWidth, gHeight];
    }

    ///     /// <param name="gameTime">An object with information about the time that has passed in the game.</param>
    /// <param name="spriteBatch">The SpriteBatch used for drawing sprites and text.</param>
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        //drawing the grid and the blocks placed in it
        for (int x = 0; x < gWidth; x++) //0
        {
            for (int y = 0; y < gHeight; y++) //0
            {
                //drawing an emtpy grid
                position = new Vector2(x * emptySprite.Width, y * emptySprite.Height);
                Gridpositions[x, y] = position;
                spriteBatch.Draw(emptySprite, position, Color.White);

               if (gridData[x, y].hasblock)
                    spriteBatch.Draw(Block.sprite, position, gridData[x, y].blockColor);
            }
        }
    }

    //add a block in the grid after it is placed.
    public void AddPlacedBlock(Block block, Vector2 blockpos)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (block.currentShape[i, j])
                {
                    BlockData new_data = new BlockData(true, block.shapetype, block.CurrentColor);
                    gridData[i + (int)blockpos.X, j + (int)blockpos.Y] = new_data;
                }
            }
        }
    }

    //remove a whole block row.
    public void RemoveBlockRow(int rownumber)
    {
        for (int i = rownumber; i > 0; i--)
        {
            for (int j = 0; j < gWidth; j++)
            {
                gridData[j, i] = gridData[j, i - 1];
            }
        }

        // Clear the top row
        for (int j = 0; j < gWidth; j++)
        {
            gridData[j, 0] = new BlockData(false, ShapeType.EmptyShape, Color.Black);
        }
    }

    public void ShiftBlocksDown(int fromRowNumber)
    {
        for (int i = fromRowNumber - 1; i >= 0; i--)
        {
            for (int j = 0; j < gWidth; j++)
            {
                BlockData temp = gridData[j, i];
                if (temp.hasblock)
                {
                    int lookbelow = i + 1;
                    while (lookbelow < gHeight && !gridData[j, lookbelow].hasblock)
                    {
                        gridData[j, lookbelow] = temp;
                        gridData[j, lookbelow - 1] = new BlockData(false, ShapeType.EmptyShape, Color.White);
                        lookbelow++;
                    }
                }
            }
        }
    }
    //reset the whole grid, remove everything, after the game has ended.
    public void ResetGrid()
    {
        for (int i = 0; i < gWidth; i++)
        {
            for (int j = 0; j < gHeight; j++)
            {
                BlockData data = new BlockData(false, ShapeType.EmptyShape, Color.Black);
                gridData[i, j] = data;
            }
        }
    }

}
