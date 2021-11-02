using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera cam;

    [Header("Key Binds")]
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;

    [Header("Gun Data")]
    [SerializeField] private int range = 100;
    [SerializeField] private float fireRate = 15f;
    public float baseDamage = 30f;

    private float nextTimeToFire = 0f;

    private RaycastHit hit;

    void Update()
    {
        if (Input.GetKeyDown(shootKey) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/ fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Player player = hit.transform.GetComponent<Player>();

            if (player != null)
            {
                hit.transform.gameObject.GetComponent<Player>().TakeDamage(baseDamage);
            }
        }
    }
}
