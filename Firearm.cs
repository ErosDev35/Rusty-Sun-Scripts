using UnityEngine;
using System.Collections;

namespace AYellowpaper.SerializedCollections
{
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
        [SerializedDictionary("Attachement Name", "Attachement")]
        public SerializedDictionary<string, Attachement> attachements;
        public IEnumerator bulletShellEject(Transform player, Transform cMCam, float time)
        {
            yield return new WaitForSeconds(time);
            var bulletShellParticle = Instantiate(bulletShellEjectParticle);
            bulletShellParticle.gameObject.transform.position = player.position;
            bulletShellParticle.gameObject.transform.rotation = cMCam.rotation;
        }
    }
}