using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObject : MonoBehaviour
{
    Vector3 originalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = gameObject.transform.position; //Looks for gameobject originals position
    }

    public void resetPosition(){
        transform.position = originalPosition;
    }
   
}
