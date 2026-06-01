using UnityEngine;

public class HitboxeEnnemy : MonoBehaviour
{
    Shootable shootable;
    public float damageMultiplier = 1;
    public void Start()
    {
        shootable = transform.parent.GetComponentInParent<Shootable>();
    }
    public void Shooted(float damage = 0, Vector3 hitPos = new Vector3(), Vector3 playerPos = new Vector3())
    {
        print(damage + " x " + damageMultiplier);
        shootable.shootInteraction(damage * damageMultiplier, hitPos, playerPos);
    }
}
