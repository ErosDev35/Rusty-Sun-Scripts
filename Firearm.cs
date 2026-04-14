using UnityEngine;
using System.Collections;

public class Firearm : Weapon
{
    public int ammo;
    public int maxAmmo;
    public string ammoName;
    public float accuracy = 100;
    public int bulletToFire = 1;
    public AudioClip firearmShootingSound;
    public double reloadingTime;
}
