using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    public static float yOffset = 0;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position + new Vector3(0, yOffset, 0);
    }
}
