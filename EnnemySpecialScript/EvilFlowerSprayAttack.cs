using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvilFlowerSprayAttack : MonoBehaviour
{
    public float damage = 0;
    public PlayerCharacterController player;
    void Start()
    {
        player = GameObject.Find("Player").transform.GetComponent<PlayerCharacterController>();
    }
    void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        int numEvents = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numEvents; i++)
        {
            Vector3 collisionPosition = collisionEvents[i].intersection;

            RaycastHit hit;

            if (Physics.Raycast(collisionPosition, player.transform.position - collisionPosition, out hit, 10, LayerMask.GetMask("Default")))
            {
                if (hit.transform.gameObject.name == "ItemPusher")
                {
                    hit.transform.parent.GetComponent<PlayerCharacterController>().HurtPlayer(damage, null, 0.5f, 0.75f, 0.75f, 0);
                }
            }
        }
        if (Vector3.Distance(player.transform.position, transform.position) < 50f)
        {
            player.AddExplosion(0.5f);
        }
    }
}
