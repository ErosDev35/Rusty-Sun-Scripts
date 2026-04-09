using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InterfaceFirearm : MonoBehaviour
{
    public List<Transform> firearmBars;
    public List<Transform> firearmBarsOrigin;
    public PlayerCharacterController playerComponent;
    public float playerAccuracy;
    public Transform crosshairOrigin;

    void Start()
    {
        GameObject player = GameObject.Find("Player");
        playerComponent = player.GetComponent<PlayerCharacterController>();
    }
    void Update()
    {
        playerAccuracy = playerComponent.firearmAccuracy;
        foreach(Transform bar in firearmBars)
        {
            Transform origin = firearmBarsOrigin[firearmBars.IndexOf(bar)];

            bar.position = origin.position + ((origin.position - crosshairOrigin.position) * playerAccuracy / 5);
        }
    }
}
