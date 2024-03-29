using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public Rigidbody2D throwable;
    public float throwForce = 100f;

    // Update is called once per frame
    public void Throw(float direction)
    {
        //Create a vector direction for the throw
        Vector2 throwDir = new Vector2(direction, 0);

        //Throw the object
        throwable.AddForce(throwDir * throwForce, ForceMode2D.Impulse);   
    }
}
