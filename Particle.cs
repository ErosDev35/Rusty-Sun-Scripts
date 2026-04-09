using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour
{
    public float timeToDie = 2;
    void Start()
    {
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(timeToDie);
        Destroy(this.gameObject);
    }
}
