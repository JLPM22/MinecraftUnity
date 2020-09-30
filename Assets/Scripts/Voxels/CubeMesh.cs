using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CubeMesh
{
    #region Vertices
    readonly static Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
    readonly static Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
    readonly static Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
    readonly static Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);

    readonly static Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
    readonly static Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
    readonly static Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
    readonly static Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

    public readonly static Vector3[] Vertices = new Vector3[]
    {
        // Bottom
        p0, p1, p2, p3,
        // Left
        p7, p4, p0, p3,
        // Front
        p4, p5, p1, p0,
        // Back
        p6, p7, p3, p2,
        // Right
        p5, p6, p2, p1,
        // Top
        p7, p6, p5, p4
    };
    #endregion

    #region Normales
    readonly static Vector3 up = Vector3.up;
    readonly static Vector3 down = Vector3.down;
    readonly static Vector3 front = Vector3.forward;
    readonly static Vector3 back = Vector3.back;
    readonly static Vector3 left = Vector3.left;
    readonly static Vector3 right = Vector3.right;

    public readonly static Vector3[] Normals = new Vector3[]
    {
        // Bottom
        down, down, down, down,
        // Left
        left, left, left, left,
        // Front
        front, front, front, front,
        // Back
        back, back, back, back,
        // Right
        right, right, right, right,
        // Top
        up, up, up, up
    };
    #endregion

    #region UVs
    readonly static float Epsilon = 0.001f;
    readonly static Vector2 _00 = new Vector2(Epsilon, Epsilon);
    readonly static Vector2 _10 = new Vector2(BlockInfo.TexStep - Epsilon, Epsilon);
    readonly static Vector2 _01 = new Vector2(Epsilon, BlockInfo.TexStep - Epsilon);
    readonly static Vector2 _11 = new Vector2(BlockInfo.TexStep - Epsilon, BlockInfo.TexStep - Epsilon);

    public readonly static Vector2[] UVs = new Vector2[]
    {
        // Bottom
        _11, _01, _00, _10,
        // Left
        _11, _01, _00, _10,
        // Front
        _11, _01, _00, _10,
        // Back
        _11, _01, _00, _10,
        // Right
        _11, _01, _00, _10,
        // Top
        _11, _01, _00, _10,
    };
    #endregion

    #region Triangles
    public readonly static int[] Triangles = new int[]
    {
        // Bottom
        3, 1, 0,
        3, 2, 1,		
        // Left
        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
        3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
        // Front
        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
        3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
        // Back
        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
        3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
        // Right
        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
        3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
        // Top
        3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
        3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

    };
    #endregion
}