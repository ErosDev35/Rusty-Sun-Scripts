using UnityEngine;
using System.Collections;

public class Firearm : MonoBehaviour
{
    public int ammo;
    public int maxAmmo;
    public string ammoName;
    public double firerate;
    public float knockback;
    public double reloadingTime;
    public bool canShoot = true;
    public double damage = 1;
    public float accuracy = 100;
    public int bulletToFire = 1;
    public float soundRadius = 50;
    public AudioClip firearmShootingSound;
    public IEnumerator betweenShotsWaitTime()
    {
        yield return new WaitForSeconds(1 / (float) firerate);
        canShoot = true;
    }
}
