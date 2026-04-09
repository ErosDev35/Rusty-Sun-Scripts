using UnityEngine;

public class OverrideSortingFix : MonoBehaviour
{
    void Update()
    {
        GetComponent<Canvas>().overrideSorting = false;
    }
}
