using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    public float rotationSpeed = 30.0f;

    // Update is called once per frame
    void Update() 
    {
        transform.eulerAngles += Vector3.forward * rotationSpeed * Time.deltaTime;
    }
}
