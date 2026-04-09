using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldData
{
    //General infos
    public int worldId;
    public string worldName = "Empty";
    public string worldTime = "00:00:00";
    public int deathNumber = 0;
    //Player infos
    public PlayerCharacterController playerCharacterController;
    public Vector3 playerSavedPos = Vector3.zero;
    public Vector3 playerSavedRotation = Vector3.zero;
    //Items infos
    public List<GameObject> itemsSaved = new List<GameObject>();
    public List<Vector3> itemSavedPos = new List<Vector3>();
    public List<int> itemSavedNumber = new List<int>();
    //Entities infos
    public Transform entities;
    public Dictionary<Transform, Vector3> entitiesSavedPos;
}
