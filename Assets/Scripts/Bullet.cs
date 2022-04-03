using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float cooldown;
    public float damage;
    float timer;
    GameManager manager;
    [SerializeField]
    GameObject explosion;
    public bool dontDieOnWall;
    private void Start()
    {
        manager = GameManager.gameManager;
    }
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > cooldown)
        {
            Destroy(this.gameObject);
            if (this.tag == "PlayerBullet")
            {
                manager.misses += 1;//used for an achievement
            }
        }
        else if (manager.killAll)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dontDieOnWall)
        {
            if (collision.collider.gameObject.tag == "Player")
            {
                Destroy(collision.otherCollider.gameObject);
            }
        }
        else
        {
            Destroy(collision.otherCollider.gameObject);
        }
    }
}
