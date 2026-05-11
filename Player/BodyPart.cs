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
    public float antibleedingApplied = 0;
    public Medicine medicineApplied = null;
    void Update()
    {
        Infection();
        Bleed();
        DesinfectantDecrease();
        AntiBleedingDecrease();
    }    
    public bool ShouldBleed()
    {
        bool shouldBleed = isBleeding && (medicineApplied == null || medicineApplied.dirtynessRate >= 1f);
        shouldBleed = (antibleedingApplied > 0.5)? false : shouldBleed;
        return shouldBleed;
    }
    void Bleed()
    {
        if (ShouldBleed() || (bodyPartState == BodyPartState.SCARED && medicineApplied == null))
        {
            bodyPartHealth -= Time.deltaTime * (1 - antibleedingApplied);
        }
        if(ShouldBleed() && medicineApplied)
        {
            medicineApplied.dirtynessRate += Time.deltaTime / 10;
        }
    }
    void AntiBleedingDecrease()
    {
        antibleedingApplied -= (antibleedingApplied > 0)? Time.deltaTime / 200 : 0;
    }
    void DesinfectantDecrease()
    {
        desinfectantApplied -= (desinfectantApplied > 0)? Time.deltaTime / 10 : 0;
        infectionRate -= (infectionRate > 0 && desinfectantApplied > 0)? (Time.deltaTime / 10) * ((medicineApplied)? medicineApplied.dirtynessRate : 1) : 0;
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
