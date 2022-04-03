using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float lifeRemaining = 80;//in seconds
    public float maxLife = 80;//80 seconds is just over a minute
    public float speed = 1;
    public Sprite[] playerSprites;
    public Sprite deadSprite;
    public Weapon[] weapons;
    [SerializeField]
    Transform[] firepoints;
    [SerializeField]
    Transform[] firepoints2;
    [SerializeField]
    Transform[] firepoints3;
    [SerializeField]
    Transform[] firepoints4;
    [SerializeField]
    Transform[] firepoints5;
    [SerializeField]
    GameObject followMouse;
    int weapon = 0;
    float nextBullet;
    Vector2 movement;
    public Rigidbody2D rb;
    bool dead = false;
    SpriteRenderer rend;
    GameManager gameManager;
    bool sfxDisabled;
    int lastId = 0;
    [SerializeField]
    float elixirBonus = 12f;
    [SerializeField]
    SpriteRenderer weaponRend;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameManager.gameManager;
        try
        {
            gameManager.player = this;
        }
        catch
        {
            Debug.LogWarning("unable to set gameManager player during awake");//should only happen if execution order went wrong somehow
        }
        rend = this.GetComponent<SpriteRenderer>();
        lastId = 0;
        rb = this.GetComponent<Rigidbody2D>();
    }

    void SetWeapon(int weapon2)
    {
        weapon = weapon2;
        if (weapon >= weapons.Length)
        {
            weapon = 0;
        }
        if(weapon < 0)
        {
            weapon = weapons.Length - 1;
        }
        if (weapons[weapon].sprite != null && weaponRend != null)
        {
            weaponRend.sprite = weapons[weapon].sprite;
        }
    }

    void Start()
    {
        gameManager = GameManager.gameManager;
        gameManager.player = this;
        rend.sprite = playerSprites[lastId];
        SetWeapon(0);
    }
    public void Initialize()
    {
        nextBullet = Time.time + 0.1f;
        lifeRemaining = maxLife;
        this.transform.position = Vector3.zero;
        dead = false;
        UpdateSprite();
    }
    private void Shoot()
    {
        float volume = 1;
        if (gameManager.sfxDisabled)
        {
            volume = 0;
        }
        if (Time.time > nextBullet)
        {
            if (weapons.Length > 0)
            {
                Weapon weapon = weapons[this.weapon];
                nextBullet = Time.time + weapon.cooldown;
                weapon.Shoot(firepoints,firepoints2,firepoints3,firepoints4,firepoints5, gameManager.gameObject.transform.position, 1, volume, 1,Vector2.zero);
                gameManager.shots += 1;
            }
        }
    }
    private void Update()
    {
        if (gameManager.gameRunning)
        {
            GetMovement();
            if (Input.GetButton("Fire"))
            {
                Shoot();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)||Input.GetKeyDown(KeyCode.Keypad1))
            {
                SetWeapon(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                SetWeapon(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                SetWeapon(2);
            }
            else if(Input.mouseScrollDelta.y > 0)
            {
                SetWeapon(weapon - 1);
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                SetWeapon(weapon + 1);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                SetWeapon(weapon + 1);
            }
            UpdateSprite();
        }
        //Debug.Log(Input.mousePosition);
        Vector2 mousePos = Input.mousePosition;
        Vector2 lookDir = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10)) - followMouse.transform.position;
        float angle = (Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f);
        followMouse.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    void GetMovement()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")) * speed;
    }
    void DoMovement()
    {
        rb.velocity = movement;
        gameManager.movedAmount += Time.fixedDeltaTime;
    }
    void UpdateSprite()
    {
        float percentage = lifeRemaining / maxLife;
        int id = (playerSprites.Length - 1) - Mathf.RoundToInt((percentage * playerSprites.Length) - 0.5f);
        if (id > playerSprites.Length)
        {
            id = playerSprites.Length;
        }
        if (id < 0)
        {
            id = 0;
        }
        if (lastId != id)
        {
            lastId = id;
            rend.sprite = playerSprites[id];//to not update rendering as much
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameManager.gameRunning)
        {
            lifeRemaining -= Time.fixedDeltaTime;
            if (lifeRemaining <= 0)
            {
                onDeath();
            }
            DoMovement();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Damage(float amount)
    {
        if (!dead)
        {
            lifeRemaining -= amount;
            if (lifeRemaining <= 0)
            {
                onDeath();
            }
        }
    }

    void onDeath()
    {
        dead = true;
        rend.sprite = deadSprite;
        gameManager.EndGame();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;
        if (obj.layer == 8)//enemy bullet
        {
            Bullet bullet = obj.GetComponent<Bullet>();
            if (bullet == null)
            {
                Damage(1);
            }
            else
            {
                Damage(bullet.damage);
            }
        }
        else if (obj.layer == 12)//pickup
        {
            if (obj.tag == "Elixir")
            {
                lifeRemaining += elixirBonus;
                if (lifeRemaining > maxLife)
                {
                    lifeRemaining = maxLife;
                }
                Destroy(obj);
            }
        }
    }
}
