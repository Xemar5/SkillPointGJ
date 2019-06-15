using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Node
{

    public int index;
    public Vector2 position;
    public NodeSide side;
    public Vector2 velocity;

    public override bool Equals(object obj)
    {
        return obj is Node && this == (Node)obj;
    }
    public override int GetHashCode()
    {
        return index.GetHashCode() ^ side.GetHashCode();
    }
    public static bool operator == (Node a, Node b)
    {
        return a.index == b.index && a.side == b.side;
    }
    public static bool operator !=(Node a, Node b)
    {
        return !(a == b);
    }

}
