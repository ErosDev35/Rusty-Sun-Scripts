using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class BuildUsage : MonoBehaviour
{
    Build build;
    PlayerCharacterController player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerCharacterController>();
        build = GetComponent<Build>();
    }
    public void BuildUse()
    {
        GameInterface gameInterface = GameInterface.Instance;
        
        switch (build.buildCategory)
        {
            case "Container":
                player.isLookingAtBag = true;
                gameInterface.mainTab = false;
                gameInterface.container = GetComponent<Container>();
                gameInterface.ContainerTabInit();
            break;
            default:
                print("Je sais pas t'es quoi mais bon bonne chance");
            break;
        }
    }
}
