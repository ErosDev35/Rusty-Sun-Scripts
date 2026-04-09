using UnityEngine;

public class WorldBehaviour : MonoBehaviour
{
    public float dayCoefficient = 1;
    public Transform sun;
    public Material skyboxMaterial;
    public Color dayColor;
    public Color nightColor;
    void Start()
    {
        skyboxMaterial = RenderSettings.skybox;
    }
    void Update()
    {
        DayCycle();
    }
    void DayCycle()
    {
        sun.Rotate(-1 * (dayCoefficient*Time.deltaTime),0,0);
        Light light = sun.GetChild(0).GetComponent<Light>();

        float dayCycle = map(light.transform.position.y,-150,150,0,1);
        light.intensity = 1.5f * dayCycle;

        skyboxMaterial.SetFloat("_SunHaloContribution",0.903f * dayCycle);
        skyboxMaterial.SetFloat("_HorizonLineContribution",dayCycle);
        skyboxMaterial.SetColor("_SkyGradientBottom",skyColorTransition(dayCycle));
        skyboxMaterial.SetColor("_SkyGradientTop",skyColorTransition(dayCycle));
    }
    float map(float x, float in_min, float in_max, float out_min, float out_max) {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    Color skyColorTransition(float dayCycle)
    {
        Color skyColor = Color.Lerp(nightColor, dayColor, dayCycle);

        return skyColor;
    }
}
