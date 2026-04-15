using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AYellowpaper.SerializedCollections;
using UnityEngine.EventSystems;
using System;
using UnityEditor;
using NUnit.Framework;
public class GameInterface : MonoBehaviour
{
    public GameObject player;
    public PlayerCharacterController playerComponent;
    public RectTransform stamina;
    public Vector2 minWidthHeightStamina;
    private Vector2 staminaWidthHeightLock;
    private UnityEngine.UI.Image staminaImage;
    public VolumeProfile gameVolume;
    private ChromaticAberration chromaticAberration;
    private DepthOfField depthOfField;
    ChromaticAberration cA;
    DepthOfField dOF;
    private Transform cMCam;
    public Transform bag;
    public GameObject itemPlayerLooking;
    public TextMeshProUGUI itemName;
    public Slot slotHovered;
    public TextMeshProUGUI inventoryItemName;
    public TextMeshProUGUI inventoryItemDescription;
    public TextMeshProUGUI inventoryItemWeight;
    public TextMeshProUGUI inventoryItemType;
    public Transform itemPhoto;
    public Transform itemWeightLogo;
    public Transform crosshair;
    public List<Animator> hotbarSlotAnim;
    public Transform hotbar;
    public Transform itemSelectedCursor;
    public Transform itemVisualizerBackground;
    public Transform handsItem;
    public Item itemHand;
    public Image itemHandDisplay;
    public Animator itemHandDisplayAnim;
    public Sprite emptyHand;
    public Transform firearmCrosshair;
    public float baseFov;
    public Image healthDisplay;
    public Transform playerNeeds;
    public float needGap = 10;
    public bool showHealRadialMenu = false;
    public Transform healthRadialMenu;
    public Image itemHealthMenuImage;
    public TextMeshProUGUI itemHealthMenuText;
    public Image crouchOverlay;
    public Transform buildMenu;
    public LayerMask levelMask;
    public Transform preBuildVisualizer;
    public Build buildToBuild;
    public List<Item> itemToBuild = new List<Item>();
    public float buildRotationIntensity = 1;
    public Transform bodyMenu;
    public List<Transform> bodyMenuBodyParts;
    public BodyPart bodyPartToCheck;
    public LayerMask ArmMask;
    public Dictionary<int, bool> canBuildThatBuild = new Dictionary<int, bool>();
    public List<Dictionary<string, int>> buildNeedsDict = new List<Dictionary<string, int>>();
    public int buildToBuildId;
    public GameObject pauseMenu;
    public bool gamePaused = false;
    public Transform[] bodyCheckDescription;
    public Transform contextMenuCheckBody;
    public OptionData optionData;
    public bool isPlayerBleeding = false;
    void Start()
    {
        bag.gameObject.SetActive(false);
        player = GameObject.Find("Player");
        Cursor.lockState = CursorLockMode.Locked;
        playerComponent = player.GetComponent<PlayerCharacterController>();
        stamina = GameObject.Find("Stamina").GetComponent<RectTransform>();
        staminaImage = GameObject.Find("Stamina").GetComponent<Image>();
        staminaWidthHeightLock = new Vector2(stamina.rect.width, stamina.rect.height);
        cMCam = playerComponent.cMCam;
        itemName.text = "";
        baseFov = cMCam.GetComponent<CinemachineCamera>().Lens.FieldOfView;

        if (gameVolume.TryGet(out cA))
        {
            chromaticAberration = cA;
            chromaticAberration.intensity.overrideState = true;
            chromaticAberration.intensity.value = 0f;
        }
        if (gameVolume.TryGet(out dOF))
        {
            depthOfField = dOF;
            depthOfField.gaussianStart.value = 200f;
        }
        BuildInit();
    }
    public void OptionInit(OptionData optionToLoad)
    {
        print("On recharge les options au niveau de l'interface");
        this.optionData = optionToLoad;
    }
    void BuildInit()
    {
        foreach (Transform buildOption in buildMenu.GetChild(0).GetChild(0).GetChild(0))
        {
            Transform buildNeed = buildOption.GetChild(3);
            Item[] itemNeeded = buildNeed.GetComponents<Item>();

            foreach (Transform itemNeeds in buildNeed)
            {
                itemNeeds.GetComponent<Image>().sprite = itemNeeded[itemNeeds.GetSiblingIndex()].inventoryImage;
                itemNeeds.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemNeeded[itemNeeds.GetSiblingIndex()].itemNumber.ToString();
                buildNeedsDict.Add(new Dictionary<string, int>());

                buildNeedsDict[buildOption.GetSiblingIndex()].Add(itemNeeded[itemNeeds.GetSiblingIndex()].itemName, itemNeeded[itemNeeds.GetSiblingIndex()].itemNumber);
            }
        }
    }
    void Update()
    {
        Inventory();
        CursorLockChoose();
        CrosshairShouldBeActive();
        CheckHotbarItems();
        Health();
        Visualizer();
        SoundSync();
        Pause();
        SlotColoring();
    }
    void SlotColoring()
    {
        Inventory bag = this.bag.GetComponent<Inventory>();
        List<GameObject> inventorySlots = bag.slots;
        foreach (GameObject slot in bag.equippedSlots) inventorySlots.Add(slot);
        foreach (GameObject slot in inventorySlots)
        {
            if (bag.ItemSelectedSlot.slotItem != null)
            {
                slot.GetComponent<Image>().color = (bag.TypeCorrespondanceCheck(bag.ItemSelectedSlot.slotItem, slot.GetComponent<Slot>().item_type) || slot.GetComponent<Slot>().item_type == "any")
                ? new Color(0.65f, 0.65f, 0.65f) : ColorTransition(new Color(0.65f, 0.65f, 0.65f), new Color(0.85f, 0, 0), 50 / Vector2.Distance(slot.transform.position, Input.mousePosition));
            }
            else slot.GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
        }
    }
    Color ColorTransition(Color beginColor, Color endColor, float progression)
    {
        return Color.Lerp(beginColor, endColor, progression);
    }
    void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) gamePaused = !gamePaused;
        pauseMenu.SetActive(gamePaused);
        Time.timeScale = (gamePaused) ? 0 : 1;
    }
    void Visualizer()
    {
        DisplayVisualizer();
        StaminaVisualizer();
        InventoryItemVisualizer();
        PlayerNeedVisualizer();
        CrouchVisualizer();
        BuildMenu();
        BodyCheck();
    }
    void BuildMenu()
    {
        buildMenu.gameObject.SetActive(playerComponent.wantToBuild);
        preBuildVisualizer.gameObject.SetActive(playerComponent.preBuild);

        if (playerComponent.wantToBuild)
            foreach (Transform buildOption in buildMenu.GetChild(0).GetChild(0).GetChild(0))
            {
                bool canBuild = true;

                foreach (var need in buildNeedsDict[buildOption.GetSiblingIndex()])
                {
                    int amountOfThatResource = 0;

                    foreach (GameObject slotGo in playerComponent.inventory.GetComponent<Inventory>().slots)
                    {
                        Slot slot = slotGo.GetComponent<Slot>();
                        if (slot.slotItem && slotGo.name != "SlotHotbar" && slot.slotItem.itemName.Contains(need.Key))
                        {
                            amountOfThatResource += slot.slotItem.itemNumber;
                        }
                    }
                    foreach (GameObject slotGo in playerComponent.inventory.GetComponent<Inventory>().equippedSlots)
                    {
                        Slot slot = slotGo.GetComponent<Slot>();
                        if (slot.slotItem && slot.slotItem.itemName.Contains(need.Key))
                        {
                            amountOfThatResource += slot.slotItem.itemNumber;
                        }
                    }
                    if (amountOfThatResource < need.Value) canBuild = false;
                }
                if (!canBuildThatBuild.ContainsKey(buildOption.GetSiblingIndex()) || (canBuildThatBuild.ContainsKey(buildOption.GetSiblingIndex()) && canBuildThatBuild[buildOption.GetSiblingIndex()] != canBuild))
                    canBuildThatBuild[buildOption.GetSiblingIndex()] = canBuild;
            }

        if (playerComponent.preBuild)
        {
            RaycastHit hit;
            if (Physics.Raycast(cMCam.transform.position, cMCam.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, levelMask))
            {
                bool canBuild = hit.distance < 8f;

                preBuildVisualizer.GetComponent<MeshRenderer>().material = (canBuild) ? preBuildVisualizer.GetComponent<BuildPrevisualizer>().validBuild : preBuildVisualizer.GetComponent<BuildPrevisualizer>().invalidBuild;

                preBuildVisualizer.transform.position = hit.point + buildToBuild.buildOffset;
                preBuildVisualizer.transform.localPosition = hit.point + buildToBuild.buildOffset;

                Vector3 playerDirection = player.transform.position - preBuildVisualizer.transform.position;
                Vector3 rotationTowards = Vector3.RotateTowards(preBuildVisualizer.transform.forward, playerDirection, 10000, 0.0f);
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                float direction = Vector3.Dot(hit.normal, hit.transform.forward);
                float directionRight = Vector3.Dot(hit.normal, hit.transform.right);

                rotationTowards += new Vector3(slopeAngle * directionRight * buildRotationIntensity, 90, slopeAngle * direction * buildRotationIntensity);

                preBuildVisualizer.transform.rotation = Quaternion.LookRotation(rotationTowards);

                preBuildVisualizer.transform.localScale = buildToBuild.buildPrefab.transform.localScale;

                preBuildVisualizer.GetComponent<MeshFilter>().mesh = buildToBuild.buildPrefab.GetComponent<MeshFilter>().sharedMesh;

                if (Input.GetAxis("Mouse LC") != 0 && canBuild)
                {
                    var build = Instantiate(buildToBuild.buildPrefab);
                    build.transform.position = hit.point + buildToBuild.buildOffset;
                    build.transform.rotation = preBuildVisualizer.transform.rotation;
                    playerComponent.preBuild = !playerComponent.preBuild;
                    CraftBuild();
                }
            }
            if (playerComponent.wantToBuild) playerComponent.preBuild = false;
        }
    }
    void CraftBuild()
    {
        foreach (var itemAsked in buildNeedsDict[buildToBuildId])
        {
            int nbItemWanted = itemAsked.Value;
            foreach (GameObject slotGo in playerComponent.inventory.GetComponent<Inventory>().slots)
            {
                Slot slot = slotGo.GetComponent<Slot>();
                if (slot.slotItem && slotGo.name != "SlotHotbar" && slot.slotItem.itemName.Contains(itemAsked.Key) && nbItemWanted > 0)
                {
                    print("on enlève cet item");
                    int inventoryItemNumber = slot.slotItem.itemNumber;
                    if (slot.slotItem.itemNumber - nbItemWanted <= 0)
                    {
                        playerComponent.inventory.GetComponent<Inventory>().RemoveItemComponent(slot.transform);
                        Destroy(slot.slotItem);
                        nbItemWanted -= inventoryItemNumber;
                    }
                    else
                    {
                        slot.slotItem.itemNumber -= nbItemWanted;
                        nbItemWanted = 0;
                    }
                }
            }
            // RACCOURCIR STP C'EST HORRIBLE LA VIDA LOCA
            foreach (GameObject slotGo in playerComponent.inventory.GetComponent<Inventory>().equippedSlots)
            {
                Slot slot = slotGo.GetComponent<Slot>();
                if (slot.slotItem && slot.slotItem.itemName.Contains(itemAsked.Key) && nbItemWanted > 0)
                {
                    print("on enlève cet item");
                    int inventoryItemNumber = slot.slotItem.itemNumber;
                    if (slot.slotItem.itemNumber - nbItemWanted <= 0)
                    {
                        playerComponent.inventory.GetComponent<Inventory>().RemoveItemComponent(slot.transform);
                        Destroy(slot.slotItem);
                        nbItemWanted -= inventoryItemNumber;
                    }
                    else
                    {
                        slot.slotItem.itemNumber -= nbItemWanted;
                        nbItemWanted = 0;
                    }
                }
            }
        }
    }
    void CrosshairShouldBeActive()
    {
        crosshair.gameObject.SetActive(crosshairBool());
        firearmCrosshair.gameObject.SetActive(playerComponent.handItem != null && playerComponent.handItem.firearm != null);
    }
    bool crosshairBool()
    {
        bool crosshair = !playerComponent.LookingAtUi();
        if (playerComponent.handItem != null && playerComponent.handItem.firearm != null)
            crosshair = false;

        return crosshair;
    }
    void CursorLockChoose()
    {
        Cursor.lockState = (playerComponent.LookingAtUi()) ? CursorLockMode.None : CursorLockMode.Locked;
    }
    void Inventory()
    {
        bag.gameObject.SetActive(playerComponent.isLookingAtBag);

        if (playerComponent.isLookingAtBag && !playerComponent.wantToHeal)
        {
            itemSelectedCursor.transform.position = Input.mousePosition;
            itemSelectedCursor.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            itemSelectedCursor.GetChild(0).GetComponent<Image>().color = (playerComponent.inventory.GetComponent<Inventory>().ItemSelectedSlot.slotItem == null) ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
        }
    }
    void StaminaVisualizer()
    {
        Vector2 rapportMinMax = new Vector2(map(stamina.rect.width, 0, stamina.rect.width, 0, minWidthHeightStamina.x) * (playerComponent.stamina / 100),
            map(stamina.rect.height, 0, stamina.rect.height, 0, minWidthHeightStamina.y)) * (playerComponent.stamina / 100);

        stamina.sizeDelta = new Vector2((staminaWidthHeightLock.x / (1f + (rapportMinMax.x / staminaWidthHeightLock.y)) * 1.50f), (staminaWidthHeightLock.y / (1f + (rapportMinMax.y / staminaWidthHeightLock.y))) * 1.5f);
        staminaImage.color = new Vector4(1, 1, 1, (playerComponent.stamina / 100) - 0.01f);

        if (!playerComponent.LookingAtUi())
        {
            chromaticAberration.intensity.value = (this.optionData.chromaticAberration != 0) ? (playerComponent.stamina / 100 + (1 / playerComponent.playerHealth.health) * 5) : 0;
            depthOfField.gaussianStart.value = 200 - (playerComponent.stamina * 2 + (1 / playerComponent.playerHealth.health) * 4000);
            depthOfField.gaussianEnd.value = 500 - (playerComponent.stamina * 5 + (1 / playerComponent.playerHealth.health) * 4000);
            depthOfField.gaussianMaxRadius.value = 0;
        }
        else
        {
            depthOfField.gaussianStart.value = 0;
            depthOfField.gaussianEnd.value = 0;
            depthOfField.gaussianMaxRadius.value = 1.5f;
        }
    }
    void DisplayVisualizer()
    {
        itemName.gameObject.SetActive(!playerComponent.isLookingAtBag);
        itemPlayerLooking = (playerComponent.itemLookingAt != null) ? playerComponent.itemLookingAt : (playerComponent.doorLookingAt != null) ? playerComponent.doorLookingAt : playerComponent.buildLookingAt;
        itemName.text = (itemPlayerLooking != null) ? (itemPlayerLooking.GetComponent<Item>() != null) ? itemPlayerLooking.GetComponent<Item>().itemName + ((itemPlayerLooking.GetComponent<Item>().itemNumber >= 2) ? " (" + itemPlayerLooking.GetComponent<Item>().itemNumber + ")" : "")
        : (itemPlayerLooking.GetComponent<Door>() != null) ? itemPlayerLooking.GetComponent<Door>().doorName : itemPlayerLooking.GetComponent<Build>().buildName : "";

        string descriptor = (itemPlayerLooking != null) ? (itemPlayerLooking.GetComponent<Item>() != null) ? "[E] to grab" : (itemPlayerLooking.GetComponent<Door>() != null) ? "[E] to enter" : "[E] to interact" : "";
        descriptor += (itemPlayerLooking != null && itemPlayerLooking.GetComponent<CustomItemBehaviour>()) ? " [F] " + itemPlayerLooking.GetComponent<CustomItemBehaviour>().itemInteraction : "";
        itemName.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = descriptor;

        //FPS item visualizer

        Item oldItem = this.itemHand;
        itemHand = playerComponent.handItem;

        handsItem.transform.localPosition = (itemHand) ? itemHand.itemPositionFps : Vector3.zero;

        handsItem.GetChild(0).GetComponent<MeshFilter>().mesh = (itemHand != null && itemHand.itemPrefab.GetComponent<MeshFilter>() != null) ? itemHand.itemPrefab.GetComponent<MeshFilter>().sharedMesh : null;

        handsItem.gameObject.SetActive(!playerComponent.wantToCheckBody);

        if (oldItem != itemHand)
        {
            foreach (Transform child in handsItem.GetChild(0))
            {
                child.GetComponent<MeshFilter>().mesh = new Mesh();
            }
        }

        //Modèles plus complexes

        if (itemHand != null && itemHand.itemPrefab.transform.childCount > 0)
        {
            foreach (Transform child in itemHand.itemPrefab.transform)
            {
                int result;
                int.TryParse(child.name, out result);

                handsItem.GetChild(0).GetChild(result - 1).GetComponent<MeshFilter>().mesh = child.GetComponent<MeshFilter>().sharedMesh;
                handsItem.GetChild(0).GetChild(result - 1).localScale = child.localScale;
                handsItem.GetChild(0).GetChild(result - 1).GetComponent<MeshRenderer>().material = child.GetComponent<MeshRenderer>().sharedMaterial;
                handsItem.GetChild(0).GetChild(result - 1).localPosition = child.transform.localPosition;
                handsItem.GetChild(0).GetChild(result - 1).localRotation = child.transform.localRotation;
            }
        }

        handsItem.GetChild(0).localScale = (itemHand != null) ? itemHand.itemPrefab.transform.localScale : Vector3.zero;
        handsItem.GetChild(0).GetComponent<MeshRenderer>().material = (itemHand != null) ? itemHand.itemMaterial : null;

        if (itemHand != null)
            handsItem.GetChild(0).transform.localRotation = Quaternion.Euler(itemHand.itemRotationFps);
    }
    void InventoryItemVisualizer()
    {
        slotHovered = bag.GetComponent<Inventory>().hoveredSlot;

        bool canSeeItem = slotHovered != null && slotHovered.GetComponent<Slot>().slotItem != null;

        itemVisualizerBackground.gameObject.SetActive(canSeeItem);
        inventoryItemName.text = (canSeeItem) ? slotHovered.GetComponent<Slot>().slotItem.itemName : "";
        inventoryItemDescription.text = (canSeeItem) ? slotHovered.GetComponent<Slot>().slotItem.itemDescription : "";
        inventoryItemWeight.text = (canSeeItem) ? slotHovered.GetComponent<Slot>().slotItem.itemEquippedTotalWeight.ToString() + " (" + slotHovered.GetComponent<Slot>().slotItem.itemTotalWeight.ToString() + " non équipé)" : "";
        inventoryItemType.text = (canSeeItem) ? slotHovered.GetComponent<Slot>().slotItem.type[0] : "";
        itemPhoto.gameObject.SetActive(canSeeItem);
        itemPhoto.GetComponent<Image>().sprite = (canSeeItem) ? slotHovered.GetComponent<Slot>().slotItem.inventoryImage : null;
        itemWeightLogo.gameObject.SetActive(canSeeItem);
    }
    public void ShowUpHotbar()
    {
        foreach (Animator anim in hotbarSlotAnim)
        {
            anim.SetBool("ShowSlot", false);
        }

        return;
    }
    void Health()
    {
        healthDisplay.color = new Color(1, 1, 1, (1 - (playerComponent.playerHealth.health / 100)) - 0.15f);

        healthRadialMenu.gameObject.SetActive(playerComponent.wantToHeal);

        if (playerComponent.wantToHeal)
        {
            float minDistanceFromCursor = 75;
            Transform nearestBodyPart = null;

            foreach (Transform bodyPartTransform in healthRadialMenu)
            {
                float distanceFromCursor = Vector3.Distance(bodyPartTransform.position, Input.mousePosition);
                if (distanceFromCursor < minDistanceFromCursor)
                {
                    minDistanceFromCursor = distanceFromCursor;
                    nearestBodyPart = bodyPartTransform;
                }
                if (bodyPartTransform.tag != "No Fade")

                    bodyPartTransform.GetComponent<Image>().color = new Color(1, 1, 1, 100 * (1 / distanceFromCursor));
            }

            itemHealthMenuImage.sprite = playerComponent.handItem.inventoryImage;
            itemHealthMenuText.text = playerComponent.handItem.itemName;

            if (Input.GetAxis("Mouse LC") == 0)
            {
                playerComponent.wantToHeal = false;
                print(nearestBodyPart);
                playerComponent.HealPlayer((nearestBodyPart != null) ? nearestBodyPart.name : null, playerComponent.handItem.medicine);
            }

            if (playerComponent.wantToHeal) playerComponent.isLookingAtBag = false;
        }
    }
    void CheckHotbarItems()
    {
        List<Transform> hotbarItems = new List<Transform>();

        for (int i = 0; i < 3; i++)
        {
            hotbarItems.Add(hotbar.transform.GetChild(i));
        }

        playerComponent.handsItems = hotbarItems;

        itemHandDisplay.sprite = (itemHand != null && itemHand.inventoryImage != null) ? itemHand.inventoryImage : emptyHand;
    }
    // ptite formule arduino tu coco :p
    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    public void HandItemDisplayUpdate()
    {
        itemHandDisplayAnim.Play("UpdateSlot");
    }
    void PlayerNeedVisualizer()
    {
        //Activer/Désactiver les besoins en fonction des besoins du joueur

        foreach (Transform need in playerNeeds)
        {
            switch (need.name)
            {
                case "Hunger":
                    need.gameObject.SetActive(playerComponent.hunger < 65);
                    break;
                case "Bleeding":
                    bool isPlayerBleeding = false;
                    foreach (BodyPart bodyPart in playerComponent.playerHealth.bodyParts)
                    {
                        if (bodyPart.ShouldBleed()) isPlayerBleeding = true;
                    }
                    this.isPlayerBleeding = isPlayerBleeding;
                    need.gameObject.SetActive(isPlayerBleeding);
                    break;
            }
        }

        //Mettre les besoins dns l'ordre sur l'écran

        foreach (Transform need in playerNeeds)
        {
            if (need.GetSiblingIndex() - 1 >= 0 && need.GetSiblingIndex() - 1 <= playerNeeds.childCount)
            {
                Transform needBefore = playerNeeds.GetChild(need.GetSiblingIndex() - 1);
                if (needBefore.gameObject.activeSelf)
                {
                    need.transform.position = new Vector3(playerNeeds.position.x, needBefore.position.y - needGap, playerNeeds.position.z);
                }
                else
                {
                    bool foundBeforeNeed = false;
                    foreach (Transform child in playerNeeds)
                    {
                        if (child.GetSiblingIndex() < need.GetSiblingIndex() && child.gameObject.activeSelf)
                        {
                            need.transform.position = new Vector3(child.position.x, child.position.y - needGap, child.position.z);
                            foundBeforeNeed = !foundBeforeNeed;
                        }
                    }
                    if (!foundBeforeNeed)
                    {
                        need.position = playerNeeds.position;
                    }
                }
            }
            else
            {
                need.position = playerNeeds.position;
            }
        }
    }
    void CrouchVisualizer()
    {
        crouchOverlay.color = new Color(1, 1, 1, (playerComponent.isCrouching()) ? 0.25f : 0);
        crouchOverlay.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, (playerComponent.isCrouching()) ? 0.15f : 0);
    }
    public void PreBuild(Build buildToBuild)
    {
        this.buildToBuild = buildToBuild;
        if (canBuildThatBuild.ContainsKey(buildToBuild.transform.parent.GetSiblingIndex()) && canBuildThatBuild[buildToBuild.transform.parent.GetSiblingIndex()])
        {
            buildToBuildId = buildToBuild.transform.parent.GetSiblingIndex();
            playerComponent.wantToBuild = false;
            playerComponent.preBuild = true;
        }
        else
        {
            print("Tu ne peux pas construire ça :(");
        }
    }
    void BodyCheck()
    {
        bodyMenu.gameObject.SetActive(playerComponent.wantToCheckBody);

        if (playerComponent.wantToCheckBody)
        {
            float maxDistance = 100;
            Transform nearestBodyPart = null;

            foreach (Transform bodyPart in bodyMenuBodyParts)
            {
                float distanceFromCursor = Vector3.Distance(bodyPart.transform.position, Input.mousePosition);
                if (distanceFromCursor < maxDistance)
                {
                    maxDistance = distanceFromCursor;
                    nearestBodyPart = bodyPart;
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                contextMenuCheckBody.gameObject.SetActive(false);
                if (nearestBodyPart) bodyPartToCheck = FindBodyPart(nearestBodyPart.name);
            }

            if (bodyPartToCheck && Input.GetKeyDown(KeyCode.Mouse1))
            {
                contextMenuCheckBody.gameObject.SetActive(!contextMenuCheckBody.gameObject.activeSelf);
                contextMenuCheckBody.position = Input.mousePosition;
            }
        }
        else
        {
            contextMenuCheckBody.gameObject.SetActive(false);
        }
        foreach (Transform bodyPartDescriptor in bodyCheckDescription)
        {
            BodyPart bodyPartToDisplay = FindBodyPart(bodyPartDescriptor.GetChild(0).GetComponent<TextMeshProUGUI>().text);

            bodyPartDescriptor.gameObject.SetActive(playerComponent.wantToCheckBody);

            if (bodyPartToCheck)
            {
                bodyPartDescriptor.GetChild(1).GetComponent<TextMeshProUGUI>().text = (MathF.Round(bodyPartToDisplay.bodyPartHealth * 100) / 100).ToString() + "%";
                bodyPartDescriptor.GetChild(2).GetComponent<TextMeshProUGUI>().text = (bodyPartToDisplay.medicineApplied) ? "Bandaged" : bodyPartToDisplay.bodyPartState.ToString();
                bodyPartDescriptor.GetChild(3).GetComponent<TextMeshProUGUI>().text = (bodyPartToDisplay.infectionRate == 0) ? "Not infected" : (MathF.Round(bodyPartToDisplay.infectionRate * 100) / 100).ToString();
            }
        }
    }
    public BodyPart FindBodyPart(string bodyPartName)
    {
        foreach (BodyPart bodyPartToCheck in playerComponent.playerHealth.bodyParts)
        {
            if (bodyPartToCheck.bodyPartName.Contains(bodyPartName))
            {
                return bodyPartToCheck;
            }
        }

        return null;
    }
    public void SoundSync()
    {
        AudioSource audioSource = playerComponent.transform.GetComponent<AudioSource>();

        if (itemHand && itemHand.itemName == "Boombox")
        {
            if (itemHand.customItemBehaviour.activatedBehaviour && !audioSource.isPlaying)
            {
                audioSource.clip = (itemHand.customItemBehaviour && itemHand.customItemBehaviour.musicSource.clip) ? itemHand.customItemBehaviour.musicSource.clip : null;
                audioSource.time = itemHand.customItemBehaviour.musicTime;
                audioSource.Play();
            }
            else if (!itemHand.customItemBehaviour.activatedBehaviour)
            {
                audioSource.Stop();
            }
            if (audioSource.isPlaying)
                itemHand.customItemBehaviour.musicTime = (audioSource.time >= 0) ? audioSource.time : itemHand.customItemBehaviour.musicTime;
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
}
