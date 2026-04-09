using UnityEngine;

public class Shootable : MonoBehaviour
{
    public string shootableType;

    public void shootInteraction(float damage = 0, Vector3 hitPos = new Vector3())
    {
        switch (shootableType)
        {
            case "Ennemy":
                if (GetComponent<Ennemy>()) GetComponent<Ennemy>().shootInteraction(damage);
                else transform.parent.GetComponent<Ennemy>().shootInteraction(damage);
                break;

            case "Item":
                GetComponent<Rigidbody>().AddForce((5f - (float)GetComponent<Item>().baseWeight / 10) * (this.transform.position - hitPos), ForceMode.Impulse);
                break;

            case "Interactable":
                print("Interactable Touché");
                break;
        }
    }
}
