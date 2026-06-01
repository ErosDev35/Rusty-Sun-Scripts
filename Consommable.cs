using UnityEngine;

public class Consommable : MonoBehaviour
{
    public double nutritiousValue;
    public double spoilingRate;
    public bool isSpoiled = false;
    public float timeToEat = 0;

    void Update() {
       isSpoiled = spoilingRate > 50;
       spoilingRate += Time.deltaTime / 20;
    }
}
