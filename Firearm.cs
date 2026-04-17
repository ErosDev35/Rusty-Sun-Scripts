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
    public Transform bulletShellEjectParticle;
    public float bulletShellEjectTiming;
    public float muzzleFlashDistance = 1;
    public Transform muzzleFlashParticle;
    public double reloadingTime;

    public IEnumerator bulletShellEject(Transform player, Transform cMCam, float time)
    {
        yield return new WaitForSeconds(time);
        var bulletShellParticle = Instantiate(bulletShellEjectParticle);
        bulletShellParticle.gameObject.transform.position = player.position;
        bulletShellParticle.gameObject.transform.rotation = cMCam.rotation;
    }
}
