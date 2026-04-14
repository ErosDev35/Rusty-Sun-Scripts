using UnityEngine;
using System.Collections;
public class Weapon : MonoBehaviour
{
    public double firerate;
    public float knockback;
    public bool canShoot = true;
    public double damage = 1;
    public float soundRadius = 50;
    public IEnumerator betweenShotsWaitTime()
    {
        yield return new WaitForSeconds(1 / (float) firerate);
        canShoot = true;
    }
}
