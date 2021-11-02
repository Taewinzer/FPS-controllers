using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [Header("Player Data")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private Vector3 startPos;

    private void Start()
    {
        currentHealth = maxHealth;

        startPos = transform.position;
    }

    private void Update()
    {
        if (currentHealth <= 0f)
        {
            currentHealth = maxHealth;
            //Destroy(gameObject);
        }

        if (transform.position.y <= -15f)
        {
            transform.position = startPos;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
}
