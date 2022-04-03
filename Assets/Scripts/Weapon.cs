using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    public int bulletAmount = 1;
    public bool mainFire = true;
    public bool secondaryFire = false;
    public bool thirdFire = false;
    public bool fourthFire = false;
    public bool fifthFire = false;
    public float force = 15;
    public float cooldown = 1f;
    public float bulletLifetime = 1f;
    public float damage = 1;
    public GameObject bulletPrefab;
    public Sprite sprite;
    public AudioClip sound;
    public float volume = 1f;
    public float manaUsed;
    public Vector3 positionRandomness;
    public float rotationRandomness;
    public bool completelyRandomRotation;
    public void Shoot(Transform firePoint)
    {
            Fire(firePoint, 1, 1,Vector3.zero);
    }
    public void Shoot(Transform[] firepoints, Transform[] secondaryFirepoints, Transform[] thirdFirepoints, Transform[] fourthFirepoints, Transform[] fifthFirepoints, Vector3 audioPos, float speedMultiplier, float volumeMultiplier, float damageMultiplier, Vector3 extraSpeed)
    {
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, audioPos, volume*volumeMultiplier);
        }
        if (mainFire)
        {
            foreach (Transform firePoint in firepoints)
            {
                Fire(firePoint, speedMultiplier, damageMultiplier, extraSpeed);
            }
        }
        if (secondaryFire)
        {
            foreach (Transform firePoint in secondaryFirepoints)
            {
                Fire(firePoint, speedMultiplier, damageMultiplier, extraSpeed);
            }
        }
        if (thirdFire)
        {
            foreach (Transform firePoint in thirdFirepoints)
            {
                Fire(firePoint, speedMultiplier, damageMultiplier, extraSpeed);
            }
        }
        if (fourthFire)
        {
            foreach (Transform firePoint in fourthFirepoints)
            {
                Fire(firePoint, speedMultiplier, damageMultiplier, extraSpeed);
            }
        }
        if (fifthFire)
        {
            foreach (Transform firePoint in fifthFirepoints)
            {
                Fire(firePoint,speedMultiplier, damageMultiplier, extraSpeed);
            }
        }
    }
    private void Fire(Transform firePoint, float speedMultipler, float damageMultiplier, Vector3 extraSpeed)
    {
        float rotationAmount;
        if (completelyRandomRotation)
        {
            rotationAmount = Random.Range(0f, 360f);
        }
        else
        {
            rotationAmount = rotationRandomness + firePoint.rotation.eulerAngles.z;
        }
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position + positionRandomness, Quaternion.Euler(0,0,rotationAmount));
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.cooldown = bulletLifetime;
            bulletScript.damage = damage * damageMultiplier;
        }
        Rigidbody2D bulletrb = bullet.GetComponent<Rigidbody2D>();
        bulletrb.velocity = extraSpeed;
        bulletrb.AddForce(firePoint.up * force * speedMultipler , ForceMode2D.Impulse);
    }
}
