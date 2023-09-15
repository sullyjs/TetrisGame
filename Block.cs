using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;


public class Block
{

    //block data
    static Color[] colorlist;
    public static Texture2D sprite { get; private set; }
    public bool[,] currentShape { get; private set; }
    public Color CurrentColor { get; private set; }
    public int currentrotation { get; private set; }
    public ShapeType shapetype { get; private set; }

    static Block()
    {
        colorlist = new[] { Color.Blue, Color.Yellow, Color.Red, Color.Turquoise, Color.Indigo, Color.Green, Color.Orange, Color.Cyan, Color.Pink };
        sprite = TetrisGame.ContentManager.Load<Texture2D>("tetris");
    }
    public Block()
    {
        shapetype = (ShapeType)ShapeFactory.random.Next(1, 7);
        currentShape = ShapeFactory.GetNewRotation(shapetype, 0);
        int randcolor = ShapeFactory.random.Next(0, 8);
        CurrentColor = colorlist[randcolor]; 
    }

    //rotate a block left and right
    public void RotateRight()
    {
        if (currentrotation == 3)
            currentrotation = 0;
        else currentrotation++;
        currentShape = ShapeFactory.GetNewRotation(shapetype, currentrotation);
    }

    public void RotateLeft()
    {
        if (currentrotation == 0)
            currentrotation = 3;
        else currentrotation--;
        currentShape = ShapeFactory.GetNewRotation(shapetype, currentrotation);
    }
}

