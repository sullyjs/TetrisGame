using Microsoft.Xna.Framework;
/// <summary>
/// Representation of Data in a single cell on the grid
/// </summary>
public  struct BlockData
{
    public readonly bool  hasblock;
    public readonly Color blockColor;
    public readonly ShapeType shape;
    public BlockData(bool _hasblock,ShapeType new_shape,Color newcolor)
    {
        hasblock = _hasblock;
        blockColor = newcolor;
        shape = new_shape;
    }
}
