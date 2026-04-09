using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public List<BodyPart> bodyParts;
    public float health = 100;
    void Update()
    {
        Health();
    }
    void Health()
    {
        if(health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        float maxHealth = 100;

        foreach(BodyPart bodyPart in bodyParts)
        {
            maxHealth -= (100 - bodyPart.bodyPartHealth) * bodyPart.healthImpact;
            //health -= (bodyPart.ShouldBleed())? Mathf.Round(100 * (bodyPart.healthImpact * Time.deltaTime)) / 100 : 0;
        }

        health = (health > maxHealth)? maxHealth : health; 
    }
}
