using System;
using System.Collections.Generic;
/// <summary>
/// 
/// </summary>
static class ShapeFactory
{
    /// <summary>
    /// the Shapefactory consists of shape types and all its possible rotations. 
    /// it will calculate the next rotation and provides this to the game if a player is rotating a tetris block.
    /// </summary>
    static Dictionary<ShapeType, List<bool[,]>> allShapes;
    public static Random random;

    static ShapeFactory()
    {
        //random number generator, will be used to generate a random block.
        random = new Random();

        //a dictionary collection, consistent of shapetypes as key, and their appropriate shape matrix as value.
        allShapes = new Dictionary<ShapeType, List<bool[,]>>();

        //all the shape types are added to the list.
        allShapes.Add(ShapeType.LeftL, PopulateRotation(new bool[,] {
                                          { true, true, true, false },
                                          { true, false, false, false },
                                          {false, false, false, false },
                                          {false, false, false, false }
                                        }));
        allShapes.Add(ShapeType.Longblock, PopulateRotation(new bool[,] {
                                           { true, true, true, true },
                                          { false, false, false, false },
                                          { false, false, false, false },
                                          { false, false, false, false }
                                        }));
        allShapes.Add(ShapeType.Tee, PopulateRotation(new bool[,] {
                                          { true, true, true, false },
                                          { false, true, false, false },
                                          {false, false, false, false },
                                          {false, false, false, false }
                                        }));
        allShapes.Add(ShapeType.Es, PopulateRotation(new bool[,] {
                                          { false, true, true, false },
                                          { true, true, false, false },
                                          {false, false, false, false },
                                          {false, false, false, false }
                                        }));
        allShapes.Add(ShapeType.Zed, PopulateRotation(new bool[,] {
                                          { true, true, false, false },
                                          { false, true, true, false },
                                          {false, false, false, false },
                                          {false, false, false, false }
                                        }));
        allShapes.Add(ShapeType.RightL, PopulateRotation(new bool[,] {
                                          { true, true, true, false },
                                          { false, false, true, false },
                                          {false, false, false, false },
                                          {false, false, false, false }
                                        }));
        allShapes.Add(ShapeType.Square, PopulateRotation(new bool[,] {
                                          { true, true, false, false },
                                          { true, true, false, false },
                                          {false, false, false, false },
                                          {false, false, false, false }
                                        }));
    }

    //calculate the rotations for each shape
    public static bool[,] GetNewRotation(ShapeType shape, int rotationIndex)
    {
        var rotationList = allShapes[shape]; // dict
                                             // Console.WriteLine(rotationIndex);
        return rotationList[rotationIndex];
    }

    static List<bool[,]> PopulateRotation(bool[,] inititalrotation) //take the intital rotation, put it in the list, then rotate it and make that the initial rotation.
    {
        List<bool[,]> rotations = new List<bool[,]>(4);
        rotations.Add(inititalrotation); // 
        for (int i = 0; i < 3; i++)// 3 rotation
        {
            inititalrotation = Rotate(inititalrotation);
            rotations.Add(inititalrotation);
        }
        return rotations;
    }

    //rotating a matrix
    static bool[,] Rotate(bool[,] matrix)
    {
        bool[,] matrix_rotated = new bool[4, 4];

        //tranpose => rows become columns and cols become rows
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                matrix_rotated[j, i] = matrix[i, j];
            }
        }

        //swap the rows
        for (int i = 0; i < 4; i++)
        {
            // swap ends
            var holder = matrix_rotated[i, 0];
            var end = matrix_rotated[i, 3];
            matrix_rotated[i, 0] = end;
            matrix_rotated[i, 3] = holder;

            //swap middle
            var middle = matrix_rotated[i, 1];
            var othermiddle = matrix_rotated[i, 2];
            matrix_rotated[i, 2] = middle;
            matrix_rotated[i, 1] = othermiddle;
        }
        return matrix_rotated;
    }
}

