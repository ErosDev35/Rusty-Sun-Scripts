using AYellowpaper.SerializedCollections;
using UnityEngine;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    public string itemName;
    public string itemDescription;
    public double baseWeight;
    public double itemTotalWeight;
    public double itemEquippedTotalWeight;
    public List<string> type;
    public Sprite inventoryImage;
    public GameObject itemPrefab;
    public double equippedWeightMultiplierValue = 1;
    public Armor armor = null;
    public Consommable consommable = null;
    public ItemUsage itemUsage = null;
    public Firearm firearm = null;
    public Medicine medicine = null;
    public CustomItemBehaviour customItemBehaviour = null;
    public Material itemMaterial;
    public bool twoHanded = false;
    public bool oneHanded = false;
    public Vector3 itemRotationFps;
    public Vector3 itemPositionFps;
    public int itemNumber = 1;
    void Start()
    {
        itemPrefab = GameObject.Find("Catalogue").GetComponent<ItemCatalogue>().itemPrefabDictionnary[itemName];
    }
    void Update()
    {
        itemTotalWeight = baseWeight * itemNumber;
        itemEquippedTotalWeight = itemTotalWeight * equippedWeightMultiplierValue;
    }
}
