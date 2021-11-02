using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrafe : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cam;
    [SerializeField] private Rigidbody rb;

    [Header("Strafe constraints")]
    [SerializeField] private float rotationSpeed;
    private float lastAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rotationSpeed = Mathf.Abs(cam.rotation.y - lastAngle) * Time.deltaTime * 1000000;

        if(0f < rotationSpeed && rotationSpeed <= 100)
        {
            rb.velocity = new Vector3(rb.velocity.x * transform.forward.x, rb.velocity.y, rb.velocity.z * transform.forward.z);
        }

        lastAngle = cam.rotation.y;
    }
}
