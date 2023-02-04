using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Coords
{
    public static Coords Zero => new Coords(0, 0);

    public int x;
    public int y;

    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}