using System.Collections.Generic;
using Snake;
using UnityEngine;

namespace Snake
{

public class SnakeParts : LinkedList<SnakePart>
{
    public SnakeParts Clone()
    {
        var result = new SnakeParts();

        foreach (SnakePart part in this)
        {
            result.AddLast(part.Clone());
        }

        return result;
    }

    public SnakeParts CloneAndMove()
    {
        return this.CloneAndMove(this.Last.Value.Direction);
    }

    public SnakeParts CloneAndMove(Vector3Int direction)
    {
        SnakeParts newParts = this.Clone();

        newParts.MoveInPlace(direction);

        return newParts;
    }

    public void MoveInPlace()
    {
        this.MoveInPlace(this.Last.Value.Direction);
    }

    public void MoveInPlace(Vector3Int direction)
    {
        LinkedListNode<SnakePart> curNode = this.First;

        while (curNode != null)
        {
            SnakePart curPart = curNode.Value;

            curPart.Pos += curPart.Direction;

            curPart.Direction = curNode.Next != null ? curNode.Next.Value.Direction : direction;

            curNode.Value = curPart;
            curNode = curNode.Next;
        }
    }
}
}