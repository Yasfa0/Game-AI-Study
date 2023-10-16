using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence() { }
    public Sequence(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        Status childStats = children[currentChild].Process();
        if (childStats == Status.RUNNING)
        {
            return Status.RUNNING;
        }
        else if (childStats == Status.FAILURE)
        {
            return childStats;
        }

        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }

}
