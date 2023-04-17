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
        LinkedListNode<SnakePart> headNode = this.Last;
        SnakePart headPart = headNode.Value;
        headPart.Direction = direction;
        headNode.Value = headPart;

        LinkedListNode<SnakePart> curNode = this.First;
        while (curNode != null)
        {
            SnakePart curPart = curNode.Value;

            curPart.Pos += curPart.Direction;
            if (curNode.Next != null)
            {
                curPart.Direction = curNode.Next.Value.Direction;
            }
            curNode.Value = curPart;
            
            curNode = curNode.Next;
        }
    }

    public SnakeParts CloneAndGrow(Vector3Int direction)
    {
        SnakeParts newParts = this.Clone();
        
        newParts.GrowInPlace(direction);

        return newParts;
    }

    public void GrowInPlace(Vector3Int direction)
    {
        Vector3Int newPos;
        if (this.Last.Value.Direction == Vector3Int.zero)
        {
            newPos = this.Last.Value.Pos;
            this.RemoveLast();
        }
        else 
            newPos = this.Last.Value.Pos + this.Last.Value.Direction;
        
        this.AddLast(new SnakePart { 
            Direction = direction, 
            Pos =  newPos
        });
    }
}
}