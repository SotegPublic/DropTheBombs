using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatablePartsMonoComponent : MonoBehaviour
{
    [SerializeField] private Transform[] partObject;

    private Vector3 rotationDelta = new Vector3(0, 0, 1);
    private float rotationSpeedScaler = 2000f;

    private void Update()
    {
        foreach (Transform part in partObject)
        {            
            part.transform.Rotate(rotationDelta * Time.deltaTime * rotationSpeedScaler);
        }
    }
}
