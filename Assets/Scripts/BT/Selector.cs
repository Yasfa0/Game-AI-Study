using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector() { }
    public Selector(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        Status childStats = children[currentChild].Process();
        if (childStats == Status.RUNNING)
        {
            return Status.RUNNING;
        }else if(childStats == Status.SUCCESS)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }

        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.FAILURE;
        }

        return Status.RUNNING;
    }
}
