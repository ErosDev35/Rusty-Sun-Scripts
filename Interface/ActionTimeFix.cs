using UnityEngine;

public class ActionTimeFix : MonoBehaviour
{
    void Update()
    {
        GetComponent<RectTransform>().offsetMax = new Vector4(0,0,0,0);
    }
}
