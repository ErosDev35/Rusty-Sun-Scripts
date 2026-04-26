using UnityEngine;

public class Shootable : MonoBehaviour
{
    public string shootableType;

    public void shootInteraction(float damage = 0, Vector3 hitPos = new Vector3(), Vector3 playerPos = new Vector3(), float pushForce = 10)
    {
        switch (shootableType)
        {
            case "Ennemy":
                Ennemy ennemy = (GetComponent<Ennemy>()) ? GetComponent<Ennemy>() : transform.parent.GetComponent<Ennemy>();
                ennemy.shootInteraction(damage);
                var bloodImpact = Instantiate(ennemy.bloodParticleContact);
                bloodImpact.transform.position = hitPos;
                bloodImpact.transform.LookAt(playerPos);
                bloodImpact.transform.position = bloodImpact.transform.position + bloodImpact.transform.forward / 3;
                break;

            case "Item":
                GetComponent<Rigidbody>().AddForce((pushForce / 100) * ((5f - (float)GetComponent<Item>().baseWeight / 10) * (this.transform.position - playerPos)), ForceMode.Impulse);
                break;

            case "Interactable":
                print("Interactable Touché");
                break;
        }
    }
}
