using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public GameInterface gameInterface;
    public Dictionary<string, Vector4> bags = new Dictionary<string, Vector4>();
    public string bagWear = "Hands";
    public string bagWearOld = "";
    public GameObject slot;
    public List<GameObject> slots = new List<GameObject>();
    public TextMeshProUGUI itemsWeight;
    public Transform handItemPosition;
    public Slot ItemSelectedSlot;
    public Slot slotExchange;
    public Slot hoveredSlot;
    public Transform player;
    public double totalArmor;
    public TextMeshProUGUI armorValue;
    public double totalThermal;
    public TextMeshProUGUI thermalValue;
    public List<GameObject> equippedSlots;
    public Transform tabs;
    public int tabSelected = 0;
    public GameObject bag;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        bags.Add("Hands", new Vector4(0, 0, 0, 1));
        bags.Add("Survivor bag", new Vector4(6, 6, 6, 6));
        bags.Add("Hitchhike bag", new Vector4(5, 5, 6, 0));
        bags.Add("Supermarket plastic bag", new Vector4(3, 3, 3, 3));
        BagUpdate();
        AllUpdate();
    }
    void Update()
    {
        if (bagWear != bagWearOld)
        {
            BagUpdate();
        }
        AllUpdate();
        UsingItems();
    }
    void InventoryTabsGestion()
    {
        int tabsChoice = 0;

        foreach (Transform tab in tabs)
        {
            tab.gameObject.SetActive(tabsChoice == tabSelected);

            tabsChoice++;
        }
    }

    void BagUpdate()
    {
        bagWearOld = bagWear;
        foreach (Transform child in bag.transform)
        {
            Destroy(child);
        }
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 3:
                    CreateBagSlot(bags[bagWear].x, new Vector3(-190, -190, 0));
                    break;
                case 2:
                    CreateBagSlot(bags[bagWear].y, new Vector3(-190, -120, 0));
                    break;
                case 1:
                    CreateBagSlot(bags[bagWear].z, new Vector3(-190, -50, 0));
                    break;
                case 0:
                    CreateBagSlot(bags[bagWear].w, new Vector3(-190, 20, 0));
                    break;
            }
        }
    }
    void CreateBagSlot(float slotNb, Vector3 posOffset)
    {
        int slotGap = 0;
        for (int b = 0; b < slotNb; b++)
        {
            var slotVar = Instantiate(slot, bag.transform);
            slotVar.transform.position = slotVar.transform.position += (posOffset + new Vector3(slotGap * -1, 0, 0));
            slotGap -= 76;
            slots.Add(slotVar);
        }
    }
    public void SlotClick(Transform slotTo, Slot customSlot = null)
    {
        Item slotItem = slotTo.GetComponent<Slot>().slotItem;
        Slot selectedSlot = (customSlot != null) ? customSlot : ItemSelectedSlot;

        if (selectedSlot.slotItem != null)
        {
            if (TypeCorrespondanceCheck(selectedSlot.slotItem, slotTo.GetComponent<Slot>().item_type) || slotTo.GetComponent<Slot>().item_type == "any")
            {
                if (slotItem != null)
                {
                    if (selectedSlot.slotItem.itemName.Equals(slotItem.itemName) && selectedSlot.slotItem.type.Contains("Stackable") && slotItem.type.Contains("Stackable"))
                    {
                        RemoveItemComponent(selectedSlot.transform);
                        slotTo.GetComponent<Slot>().slotItem.itemNumber += selectedSlot.slotItem.itemNumber;
                    }
                    else
                    {
                        slotExchange.slotItem = CopyItemProperties(selectedSlot.slotItem, slotExchange.gameObject);
                        RemoveItemComponent(selectedSlot.transform);

                        selectedSlot.GetComponent<Slot>().slotItem = CopyItemProperties(slotItem, selectedSlot.gameObject);
                        RemoveItemComponent(slotTo.transform);

                        slotTo.GetComponent<Slot>().slotItem = CopyItemProperties(slotExchange.slotItem, slotTo.gameObject);
                        RemoveItemComponent(slotExchange.transform);
                    }
                }
                else
                {
                    slotItem = CopyItemProperties(selectedSlot.slotItem, slotTo.gameObject);
                    slotTo.GetComponent<Slot>().slotItem = slotItem;
                    RemoveItemComponent(selectedSlot.transform);
                    selectedSlot.slotItem = null;
                }
            }
        }
        else
        {
            if (slotItem != null)
            {
                selectedSlot.slotItem = CopyItemProperties(slotItem, selectedSlot.gameObject);
                RemoveItemComponent(slotTo);
                Destroy(slotItem);
                slotItem = null;
            }
        }
        AllUpdate();
    }
    public void RemoveItemComponent(Transform slot)
    {
        if (slot.GetComponent<Item>() != null)
        {
            Destroy(slot.GetComponent<Item>());
        }
        if (slot.GetComponent<Armor>() != null)
        {
            Destroy(slot.GetComponent<Armor>());
        }
        if (slot.GetComponent<Consommable>() != null)
        {
            Destroy(slot.GetComponent<Consommable>());
        }
        if (slot.GetComponent<ItemUsage>() != null)
        {
            Destroy(slot.GetComponent<ItemUsage>());
        }
        if (slot.GetComponent<Firearm>() != null)
        {
            Destroy(slot.GetComponent<Firearm>());
        }
        if (slot.GetComponent<Medicine>() != null)
        {
            Destroy(slot.GetComponent<Medicine>());
        }
        if (slot.GetComponent<CustomItemBehaviour>() != null)
        {
            Destroy(slot.GetComponent<CustomItemBehaviour>());
        }
        if (slot.GetComponent<AudioSource>())
        {
            Destroy(slot.GetComponent<AudioSource>());
        }
    }
    public GameObject AddItem(GameObject groundItem)
    {
        if (groundItem != null)
        {
            if (groundItem.GetComponent<Item>().type.Contains("Stackable"))
            {
                List<GameObject> slots = this.slots;

                foreach (Transform handSlot in player.GetComponent<PlayerCharacterController>().handsItems)
                {
                    slots.Add(handSlot.GetChild(0).gameObject);
                }

                foreach (GameObject slotGO in slots)
                {
                    Slot slot = slotGO.GetComponent<Slot>();
                    Item item = groundItem.GetComponent<Item>();

                    if (slot.slotItem != null && slot.slotItem.type.Contains("Stackable") && slot.slotItem.itemName.Equals(groundItem.GetComponent<Item>().itemName))
                    {
                        slot.slotItem.itemNumber += groundItem.GetComponent<Item>().itemNumber;

                        Destroy(groundItem);

                        AllUpdate();
                        return null;
                    }
                }
            }
            if (TypeCorrespondanceCheck(groundItem.GetComponent<Item>(), "Equippable"))
            {
                foreach (Transform slotTrans in player.GetComponent<PlayerCharacterController>().handsItems)
                {
                    Slot slot = slotTrans.GetChild(0).GetComponent<Slot>();
                    if (slot.GetComponent<Slot>().slotItem == null)
                    {
                        Item item = groundItem.GetComponent<Item>();

                        if (slot.slotItem == null && (TypeCorrespondanceCheck(item, slot.item_type) || slot.item_type.Contains("any")))
                        {
                            slot.slotItem = CopyItemProperties(item, slotTrans.GetChild(0).gameObject);

                            Destroy(groundItem);

                            AllUpdate();

                            player.GetComponent<PlayerCharacterController>().handItemChoice = player.GetComponent<PlayerCharacterController>().handsItems.IndexOf(slotTrans);

                            gameInterface.HandItemDisplayUpdate();

                            return null;
                        }
                    }
                }
            }
            foreach (GameObject slotGO in slots)
            {
                Slot slot = slotGO.GetComponent<Slot>();
                Item item = groundItem.GetComponent<Item>();

                if (slot.slotItem == null && (TypeCorrespondanceCheck(item, slot.item_type) || slot.item_type.Contains("any")))
                {
                    slot.slotItem = CopyItemProperties(item, slotGO);

                    Destroy(groundItem);

                    AllUpdate();
                    return null;
                }
            }
        }
        return null;
    }
    public bool TypeCorrespondanceCheck(Item item, string itemType)
    {
        bool correspond = false;

        foreach (string type in item.type)
        {
            if (type == itemType) correspond = true;
        }

        return correspond;
    }
    public Item CopyItemProperties(Item itemGrab, GameObject slot = null, bool baseNumber = false)
    {
        if (slot.GetComponent<Item>() != null) Destroy(slot.GetComponent<Item>());
        Item item = slot.AddComponent<Item>();
        Armor armor = (itemGrab.armor != null) ? slot.AddComponent<Armor>() : null;
        Consommable consommable = (itemGrab.consommable != null) ? slot.AddComponent<Consommable>() : null;
        ItemUsage itemUsage = (itemGrab.itemUsage != null) ? slot.AddComponent<ItemUsage>() : null;
        Firearm firearm = (itemGrab.firearm != null) ? slot.AddComponent<Firearm>() : null;
        Medicine medicine = (itemGrab.medicine != null) ? slot.AddComponent<Medicine>() : null;
        CustomItemBehaviour customItemBehaviour = (itemGrab.customItemBehaviour != null) ? slot.AddComponent<CustomItemBehaviour>() : null;

        AudioSource audioSource = (itemGrab.GetComponent<AudioSource>()) ? (slot.transform.parent)? slot.AddComponent<AudioSource>() : slot.GetComponent<AudioSource>() : null;

        item.itemName = itemGrab.itemName;
        item.itemDescription = itemGrab.itemDescription;
        item.baseWeight = itemGrab.baseWeight;
        item.itemTotalWeight = itemGrab.itemTotalWeight;
        item.type = itemGrab.type;
        item.inventoryImage = itemGrab.inventoryImage;
        item.itemPrefab = itemGrab.itemPrefab;
        item.equippedWeightMultiplierValue = itemGrab.equippedWeightMultiplierValue;
        item.itemMaterial = itemGrab.itemMaterial;
        item.twoHanded = itemGrab.twoHanded;
        item.itemRotationFps = itemGrab.itemRotationFps;
        item.itemPositionFps = itemGrab.itemPositionFps;
        item.itemNumber = (baseNumber) ? 1 : itemGrab.itemNumber;
        item.oneHanded = itemGrab.oneHanded;

        if (armor)
        {
            armor.armorValue = itemGrab.armor.armorValue;
            armor.thermalIsolation = itemGrab.armor.thermalIsolation;
            armor.movementConstraint = itemGrab.armor.movementConstraint;

            item.armor = armor;
        }

        if (consommable)
        {
            consommable.nutritiousValue = itemGrab.consommable.nutritiousValue;
            consommable.spoilingRate = itemGrab.consommable.spoilingRate;
            consommable.isSpoiled = itemGrab.consommable.isSpoiled;

            item.consommable = consommable;
        }

        if (itemUsage)
        {
            itemUsage.itemType = itemGrab.itemUsage.itemType;
            itemUsage.bulletCollisionParticle = itemGrab.itemUsage.bulletCollisionParticle;

            item.itemUsage = itemUsage;
        }

        if (firearm)
        {
            firearm.ammo = itemGrab.firearm.ammo;
            firearm.maxAmmo = itemGrab.firearm.maxAmmo;
            firearm.ammoName = itemGrab.firearm.ammoName;
            firearm.firerate = itemGrab.firearm.firerate;
            firearm.knockback = itemGrab.firearm.knockback;
            firearm.reloadingTime = itemGrab.firearm.reloadingTime;
            firearm.canShoot = itemGrab.firearm.canShoot;
            firearm.damage = itemGrab.firearm.damage;
            firearm.accuracy = itemGrab.firearm.accuracy;
            firearm.bulletToFire = itemGrab.firearm.bulletToFire;
            firearm.firearmShootingSound = itemGrab.firearm.firearmShootingSound;
            firearm.canShoot = true;

            item.firearm = firearm;
        }
        if (medicine)
        {
            medicine.medicineType = itemGrab.medicine.medicineType;

            item.medicine = medicine;
        }
        if (customItemBehaviour)
        {
            customItemBehaviour.itemName = itemGrab.customItemBehaviour.itemName;
            customItemBehaviour.activatedBehaviour = itemGrab.customItemBehaviour.activatedBehaviour;
            customItemBehaviour.musicTime = itemGrab.customItemBehaviour.musicTime;
            customItemBehaviour.musics = itemGrab.customItemBehaviour.musics;

            item.customItemBehaviour = customItemBehaviour;
        }
        if (audioSource)
        {
            audioSource.clip = itemGrab.GetComponent<AudioSource>().clip;
            audioSource.spatialBlend = itemGrab.GetComponent<AudioSource>().spatialBlend;
            audioSource.dopplerLevel = itemGrab.GetComponent<AudioSource>().dopplerLevel;
            audioSource.spread = itemGrab.GetComponent<AudioSource>().spread;
            audioSource.loop = itemGrab.GetComponent<AudioSource>().loop;
            audioSource.time = itemGrab.GetComponent<AudioSource>().time;
        }

        return item;
    }
    void AllUpdate()
    {
        InventoryTabsGestion();

        double tempArmor = 0;
        double tempThermal = 0;

        foreach (GameObject slotGO in equippedSlots)
        {
            Item item = slotGO.GetComponent<Slot>().slotItem;
            if (item != null)
            {
                if (slotGO.GetComponent<Slot>().slotItem.armor != null)
                {
                    tempArmor += slotGO.GetComponent<Armor>().armorValue;
                    tempThermal += slotGO.GetComponent<Armor>().thermalIsolation;
                }
            }
        }

        totalThermal = tempThermal;
        thermalValue.text = (tempThermal >= 0) ? "+" + tempThermal.ToString() + " °C" : "-" + tempThermal.ToString() + " °C";

        totalArmor = tempArmor;
        tempArmor *= 100;
        armorValue.text = tempArmor.ToString() + " %";

        itemsWeight.text = InventoryWeightUpdate();
    }
    string InventoryWeightUpdate()
    {
        double weight = 0;

        foreach (GameObject slotGo in slots)
        {
            if (slotGo.GetComponent<Slot>().slotItem != null && slotGo.name != "SlotHotbar")
            {
                weight += slotGo.GetComponent<Slot>().slotItem.itemTotalWeight;
            }
        }

        foreach (GameObject slotGo in equippedSlots)
        {
            if (slotGo.GetComponent<Slot>().slotItem != null)
            {
                weight += slotGo.GetComponent<Slot>().slotItem.itemTotalWeight * slotGo.GetComponent<Slot>().slotItem.equippedWeightMultiplierValue;
            }
        }

        return weight.ToString();
    }
    public void ExitBag(Transform ejectTransform)
    {
        if (ItemSelectedSlot.slotItem != null)
        {
            hoveredSlot = null;
            Instantiate(ItemSelectedSlot.slotItem.itemPrefab, ejectTransform.position, ejectTransform.rotation);
            Destroy(ItemSelectedSlot);
            ItemSelectedSlot = null;
        }
    }
    public void EjectSelectedSlot()
    {
        EjectItem(ItemSelectedSlot ,true);
    }
    public GameObject EjectItem(Slot itemToEjectSlot = null, bool ejectStack = true)
    {
        gameInterface.SoundSync();
        if (itemToEjectSlot && itemToEjectSlot.slotItem != null)
        {
            PlayerCharacterController playerController = player.GetComponent<PlayerCharacterController>();
            Vector3 ejectItemPosition = (!playerController.isLookingAtBag) ? handItemPosition.GetChild(0).position : player.transform.position + playerController.cMCam.transform.forward * 1.25f;
            Quaternion rotationInstantiate = handItemPosition.GetChild(0).rotation;
            print(rotationInstantiate);
            var ejectedItem = Instantiate(itemToEjectSlot.slotItem.itemPrefab, ejectItemPosition, rotationInstantiate);
            
            CopyItemProperties(itemToEjectSlot.slotItem, ejectedItem, (!ejectStack && itemToEjectSlot.slotItem.itemNumber >= 2));

            if (!ejectStack && itemToEjectSlot.slotItem.itemNumber >= 2)
            {
                itemToEjectSlot.slotItem.itemNumber -= 1;
            }
            else
            {
                RemoveItemComponent(itemToEjectSlot.transform);
                Destroy(itemToEjectSlot.slotItem);
                itemToEjectSlot.slotItem = null;
            }

            AllUpdate();

            return ejectedItem;
        }

        return null;
    }
    public void UsingItems()
    {
        if (hoveredSlot != null && Input.GetKeyDown("e") && hoveredSlot.slotItem != null)
        {
            if (hoveredSlot.slotItem.itemUsage != null)
            {
                hoveredSlot.slotItem.itemUsage.ItemUse(hoveredSlot, hoveredSlot.slotItem, player.GetComponent<PlayerCharacterController>());
            }
        }
    }
    public void SetTabChoice(int nb)
    {
        this.tabSelected = nb;
    }
}
