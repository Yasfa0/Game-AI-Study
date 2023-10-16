using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public bool isPickedUp = false;

    public void PickUpObject()
    {
        if (!isPickedUp)
        {
            isPickedUp = true;
            gameObject.SetActive(false);
        }
    }
}
