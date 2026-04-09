using UnityEngine;
using TMPro;

public class BodyPart : MonoBehaviour
{
    public string bodyPartName;
    public float healthImpact;
    public float bodyPartHealth = 100;
    public BodyPartState bodyPartState = BodyPartState.NORMAL;
    public float infectionRate = 0;
    public bool isBleeding = false;
    public bool isInfected = false;
    public float infectionMultiplier = 0.001f;
    public float desinfectantApplied = 0;
    public Medicine medicineApplied = null;
    void Update()
    {
        Infection();
        Bleed();
        DesinfectantDecrease();
    }    
    public bool ShouldBleed()
    {
        return (isBleeding && (GetComponent<Medicine>() == null || GetComponent<Medicine>().dirtynessRate >= 1f));
    }
    void Bleed()
    {
        if (ShouldBleed() || (bodyPartState == BodyPartState.SCARED && medicineApplied == null))
        {
            bodyPartHealth -= Time.deltaTime;
        }
    }
    void DesinfectantDecrease()
    {
        desinfectantApplied -= (desinfectantApplied > 0)? Time.deltaTime / 10 : 0;
        infectionRate -= (infectionRate > 0 && desinfectantApplied > 0)? Time.deltaTime / 10 : 0;
    }
    void Infection()
    {
        float infectionMultiplierAddUp = 0.001f;

        if(bodyPartState == BodyPartState.SCARED)
        {
            infectionMultiplierAddUp += 0.005f;
        }

        infectionRate += (ShouldBleed() || bodyPartState == BodyPartState.SCARED)? Time.deltaTime * infectionMultiplier : 0;
    }
}
