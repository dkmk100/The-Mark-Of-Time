using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float speed;
    public Weapon[] constantAttacks;
    public Weapon[] weaponChoices;
    public bool randomizeAttack;
    public float hp = 10;
    public float maxhp = 10;
    public Transform[] firePoint;
    public Transform[] secondaryPoint;
    public Transform[] thirdPoint;
    public Transform[] fourthPoint;
    public Transform[] fifthPoint;
    public int damage = 1;
    public float range = 10;
    public float minDistance = 2;
    public float trueMinDistance = 0;
    public float rotateSpeed = 1;
    public float aimRandomness;
    public GameObject nonRotate;
    public int scoreValue;
    public float idleTime;
    public bool startCooldown;
    public Text healthDisplay;
    public string enemyName;
    public int resetRange;
    public AudioClip hurtSound;
    public float hurtSoundVolume = 0.5f;
    public AudioClip deathSound;
    public float deathSoundVolume = 0.5f;
    public AudioClip spawnSound;
    public float spawnSoundVolume = 0.5f;
    //private float oldRotation;
    private Player player;
    private Rigidbody2D rb;
    private Rigidbody2D playerrb;
    private float[] nextBullets;
    private float nextBullet;
    public bool randomizeIdleTime = true;
    public float randomIdleTime = 0.5f;
    public float randomCooldownTime = 0.25f;
    float randomMovement;
    float randomAngle;
    float currentAngle;
    public GameObject lootDrop;
    public int phase = 0;
    public int nextAttack;
    public bool dieOnCollision;
    public SpriteRenderer enemyRenderer;
    public Weapon deathWeapon;
    bool damageImmune = false;
    bool sfxDisabled;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        nextBullets = new float[constantAttacks.Length];
        player = GameManager.gameManager.player;
        rb = this.GetComponent<Rigidbody2D>();
        playerrb = player.GetComponent<Rigidbody2D>();
        randomMovement = Random.Range(-100, 100) / 10;
        randomAngle = Random.Range(-aimRandomness, aimRandomness);
        Vector2 lookDir = playerrb.position - rb.position;
        currentAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        setCooldowns();
        if (spawnSound != null)
        {
            AudioSource.PlayClipAtPoint(spawnSound, Vector3.zero, spawnSoundVolume);
        }
        if (enemyName == "")
        {
            Debug.LogWarning("no name!");
            enemyName = gameObject.name;
        }
        gameManager = GameManager.gameManager;
    }
    public void Initialize(GameObject prefab)
    {
        if (enemyName == "")
        {
            enemyName = prefab.name;
        }
    }
    private void Update()
    {
        nonRotate.transform.rotation = Quaternion.Euler(Vector3.zero);
        if (weaponChoices.Length > 0)
        {
            if (weaponChoices[0].sprite != null)
            {
                this.GetComponent<SpriteRenderer>().sprite = weaponChoices[0].sprite;
            }
        }
        if (Vector3.Distance(this.transform.position, player.transform.position) > resetRange && resetRange > 0)
        {
            setCooldowns();
        }

    }
    private void setCooldowns()
    {
        if (constantAttacks.Length > 0)
        {
            int i = 0;
            while (i < constantAttacks.Length)
            {
                nextBullets[i] = Time.time;
                nextBullets[i] += constantAttacks[i].cooldown;
                if (randomizeIdleTime)
                {
                   nextBullets[i] += Random.Range(0, randomIdleTime);
                }
                if (startCooldown)
                {
                    nextBullets[i] += idleTime;
                }
                i += 1;
            }
        }
        nextBullet = Time.time;

        if (weaponChoices.Length > 0)
        {
            nextBullet += weaponChoices[0].cooldown;
        }
        if (startCooldown)
        {
            nextBullet += idleTime;
        }
        if (randomizeIdleTime)
        {
            nextBullet += Random.Range(0, randomIdleTime);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
        {
            player = gameManager.player;
        }
        if (gameManager.killAll == true)
        {
            Destroy(this.gameObject);
        }
        if (gameManager.ui == false)
        {
            sfxDisabled = gameManager.sfxDisabled;
            Vector3 finalMovement = this.transform.position;
            if (Vector3.Distance(this.transform.position, player.transform.position) <= range)
            {
                Vector2 lookDir = playerrb.position - rb.position;
                float angle = (Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f) + randomAngle;
                float diff;
                diff = AngleDifference(currentAngle, angle);
                if (diff <= rotateSpeed && diff >= 0)
                {
                    rb.rotation = angle;
                    currentAngle = angle;
                }
                else if (diff >= -rotateSpeed && diff <= 0)
                {
                    rb.rotation = angle;
                    currentAngle = angle;
                }
                else if (diff < 0)
                {
                    rb.rotation = currentAngle + rotateSpeed;
                    currentAngle = rb.rotation;
                }
                else if (diff > 0)
                {
                    rb.rotation = currentAngle - rotateSpeed;
                    currentAngle = rb.rotation;
                }
                Vector2 movement = new Vector2(transform.up.x, transform.up.y);
                if (Vector3.Distance(transform.position, player.transform.position) >= minDistance)
                {
                    finalMovement = rb.position + movement * speed * Time.fixedDeltaTime;
                }
                else if (Vector3.Distance(transform.position, player.transform.position) <= trueMinDistance)
                {
                    finalMovement = rb.position - movement * speed * Time.fixedDeltaTime;
                }
                if (gameManager.gameRunning)
                {
                    Shoot();
                }
            }
            else
            {
                randomMovement += Random.Range(-50, 100) / 10;
                if (randomMovement > 180)
                {
                    randomMovement = -179;
                }
                if (randomMovement < 0)
                {
                    randomMovement = 1;
                }
                if (randomMovement < -180)
                {
                    randomMovement = 179;
                }
                float angle = randomMovement;
                rb.rotation = angle;
                finalMovement = transform.position + transform.up * speed * Time.fixedDeltaTime;
            }
            if (finalMovement.x > -1 * gameManager.worldSizeX && finalMovement.x < gameManager.worldSizeX && finalMovement.y > -1 * gameManager.worldSizeY && finalMovement.y < gameManager.worldSizeY)
            {
                rb.MovePosition(finalMovement);
            }
        }
        nonRotate.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    private float AngleDifference(float a, float b)
    {
        float d = Mathf.Abs(a - b) % 360;
        float r = d > 180 ? 360 - d : d;

        //calculate sign 
        int sign = (a - b >= 0 && a - b <= 180) || (a - b <= -180 && a - b >= -360) ? 1 : -1;
        r *= sign;
        return r;
    }
    private void Shoot()
    {
        float volume = 1;
        if (sfxDisabled)
        {
            volume = 0;
        }
        if (constantAttacks.Length > 0)
        {
            int i = 0;
            while (i < constantAttacks.Length)
            {
                if (Time.time >= nextBullets[i])
                {
                    Weapon weapon = constantAttacks[i];
                    nextBullets[i] = Time.time + weapon.cooldown + Random.Range(0, randomCooldownTime);
                    weapon.Shoot(firePoint, secondaryPoint, thirdPoint, fourthPoint, fifthPoint, player.transform.position, 1, volume, 1,rb.velocity);
                    randomAngle = Random.Range(-aimRandomness, aimRandomness);
                }
                i += 1;
            }
        }
        if (Time.time > nextBullet)
        {
            int attackChoice = nextAttack;
            if (weaponChoices.Length > 0)
            {
                Weapon weapon = weaponChoices[attackChoice];
                nextBullet = Time.time + weapon.cooldown + Random.Range(0, randomCooldownTime);
                weapon.Shoot(firePoint, secondaryPoint, thirdPoint, fourthPoint, fifthPoint, player.transform.position, 1, volume, 1,rb.velocity);
                if (randomizeAttack)
                {
                    nextAttack = Random.Range(0, weaponChoices.Length);
                }
                else
                {
                    nextAttack += 1;
                    if (nextAttack > weaponChoices.Length - 1)
                    {
                        nextAttack = 0;
                    }
                }
            }
        }
    }

    private IEnumerator ResetRotation()
    {
        yield return new WaitForSeconds(0.1f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.tag == "PlayerBullet")
        {
            if (other.GetComponent<Bullet>() != null)
            {
                Damage(other.GetComponent<Bullet>().damage, true);
            }
        }
        else if (other.tag == "Player")
        {
            if (dieOnCollision)
            {
                Damage(maxhp, false);
            }
        }
    }
    void Damage(float amount, bool increaseScore)
    {
        if (player == null)
        {
            player = gameManager.player;
        }
        float volume = 1;
        if (sfxDisabled)
        {
            volume = 0;
        }
        if (player.lifeRemaining > 0 && !damageImmune)
        {
            hp -= amount;
            if (hurtSound != null)
            {
                AudioSource.PlayClipAtPoint(hurtSound, player.transform.position, hurtSoundVolume);
            }
            if (hp < 0)
            {
                hp = 0;
            }
            if (healthDisplay != null)
            {
                healthDisplay.text = enemyName + " HP: " + Mathf.Round(hp) + "/" + Mathf.Round(maxhp);
            }
            if (hp <= 0)
            {
                if (deathSound != null)
                {
                    AudioSource.PlayClipAtPoint(deathSound, player.transform.position, deathSoundVolume);
                }
                if (lootDrop != null)
                {
                    GameObject.Instantiate(lootDrop, this.transform.position, Quaternion.identity);
                }
                if (deathWeapon != null)
                {
                    deathWeapon.Shoot(firePoint, secondaryPoint, thirdPoint, fourthPoint, fifthPoint, player.transform.position, 1, volume, 1,rb.velocity);
                }
                gameManager.OnEnemyDestroyed(this);
                Destroy(this.gameObject);
                if (increaseScore)
                {
                    gameManager.addScore(scoreValue);
                }
            }
        }
    }
}