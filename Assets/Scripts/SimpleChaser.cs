using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleChaser : MonoBehaviour
{
    GameObject player;
    public float rotateSpeed;
    public bool makeRigidbodyKinematic;
    Rigidbody2D rb;
    Rigidbody2D target;
    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameManager.gameManager.player.gameObject;
        }
        rb = this.GetComponent<Rigidbody2D>();
        target = player.GetComponent<Rigidbody2D>();
        if (makeRigidbodyKinematic)
        {
            rb.isKinematic = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = target.position - rb.position;

        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;

        rb.velocity = transform.up * rb.velocity.magnitude;
    }
}
