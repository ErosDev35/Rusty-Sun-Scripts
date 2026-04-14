using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Collections;

public class PlayerCharacterController : MonoBehaviour
{
    //Physics Section
    private float gravityValue = -9.81f;
    private CharacterController controller;
    public Vector3 playerVelocity;
    //Camera Section
    public float mouseSentivity = 10f;
    public Transform cMCam;
    private Vector2 rotation = Vector2.zero;
    private float yRotationLimit = 88f;
    private CinemachineBasicMultiChannelPerlin cMChannelPerlin;
    public float explosionCam = 0;
    //.....................................................

    //UI Section

    public GameInterface gameInterface;

    //.....................................................

    //Movements Section
    public float speed = 50f;
    public float speedDebuff = 100;
    public float sprintMultiplier = 1;
    public float jumpHeight = 3f;
    public float stamina = 100f;
    public float staminaConsumption = 1f;
    public float maxStamina = 100f;
    private bool groundedPlayer;
    private LayerMask layerMaskGround;
    public Vector3 inputMove = Vector3.zero;
    public int tiltDegree = 45;
    public int panDegree = 45;
    public float addTilt = 0;
    public float addPan = 0;
    public bool outOfBreath = false;
    float sprintMultiplierApplied = 0f;
    public float firearmKnockback = 0f;
    public float crouchMultiplier = 0f;
    public bool sliding = false;
    private Vector3 oldMovement;
    private float slideFrames = 2;
    private float oldVerticalSpeed = 0;
    private bool wasGrounded = true;
    public float timeBeforeJumpCancel = 0;
    //.....................................................

    //Inventory / Item / Hands Section 
    LayerMask itemsMask;
    public GameObject itemLookingAt;
    public bool isLookingAtBag = false;
    public GameObject inventory;
    public double weight;
    public List<Transform> handsItems;
    public Item handItem;
    public int handItemChoice = 0;
    //.....................................................

    //Armor Section
    public double armor;
    public double thermalInsolation;
    //.....................................................

    //Survival Section
    public PlayerHealth playerHealth;
    public double hunger = 100;
    public double starvingMultiplier = 1;
    public float firearmAccuracy = 0;
    public float invincibilityFrames = 0;
    public bool wantToHeal = false;
    public bool wantToCheckBody = false;
    public float eatingFrames = 0;
    public float throwingFrames = 0;
    public bool onMeleeCooldown = false;
    //House Section
    LayerMask doorsMask;
    public GameObject doorLookingAt;
    //Build Section
    LayerMask buildMask;
    public GameObject buildLookingAt;
    //Build Section
    public bool wantToBuild = false;
    public bool preBuild = false;
    //.....................................................
    // Audio Section
    public float stepTime = 0;
    public float timeBetweenStep = 100;
    public float timeBetweenRunningStep = 10;
    public float randomStepTime = -20;
    public AudioClip stepSound;
    public Transform soundEffectFolder;
    //.....................................................
    void Start()
    {
        itemsMask = LayerMask.GetMask("Item");
        doorsMask = LayerMask.GetMask("Door");
        buildMask = LayerMask.GetMask("Build");
        controller = gameObject.GetComponent<CharacterController>();
        cMCam = GameObject.Find("Main Cinemachine Camera").transform;
        layerMaskGround = LayerMask.GetMask("Ground");
        cMChannelPerlin = cMCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        inventory = GameObject.Find("Bag");
    }

    // Update is called once per frame
    void Update()
    {
        if (!LookingAtUi()) PrimaryActions();
        SecondaryActions();
    }

    void PrimaryActions()
    {
        Move();
        Jump();
        ItemCheck();
        HandSlots();
        DoorCheck();
        BuildCheck();
        ShouldKeepSliding();
    }
    void SecondaryActions()
    {
        Camera();
        Stamina();
        BagCheck();
        StatusCheck();
        Starving();
        EjectItem();
        Crouch();
        InvincibilityFrames();
        Build();
        CheckOnBody();
        PlayerAudio();
        MeleeAttack();
        BodyPartDebuffGestion();
    }
    void Camera()
    {
        Quaternion tilt = Quaternion.Euler(0, 0, (!LookingAtUi()) ? Input.GetAxis("Horizontal") * (tiltDegree * -1) + addTilt : addTilt);
        Quaternion pan = Quaternion.Euler((!LookingAtUi()) ? Input.GetAxis("Vertical") * (panDegree) + addPan : addPan, 0, 0);

        if (!LookingAtUi())
        {
            rotation.x += Input.GetAxis("Mouse X") * mouseSentivity;
            rotation.y += Input.GetAxis("Mouse Y") * mouseSentivity;

            rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        }

        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = (LookingAtUi() && (wantToCheckBody || isLookingAtBag)) ? Quaternion.AngleAxis(-20, Vector3.left) : Quaternion.AngleAxis(rotation.y, Vector3.left);

        cMCam.localRotation = xQuat * yQuat;

        cMCam.transform.rotation = (cMCam.transform.rotation * tilt) * (pan * Quaternion.Euler(firearmKnockback * -1, 0, 0));

        transform.rotation = Quaternion.Euler(0, cMCam.transform.rotation.y, cMCam.transform.rotation.z);

        cMChannelPerlin.AmplitudeGain = map(explosionCam + 10 / playerHealth.health + Mathf.Abs(inputMove.x) + Mathf.Abs(inputMove.z) + ((playerVelocity.y < -1f) ? (playerVelocity.y * -1) : 0), 0, 15, 0.25f, 1.5f);
        cMChannelPerlin.FrequencyGain = cMChannelPerlin.AmplitudeGain / 5f + explosionCam * 0.25f + ((isRunning()) ? 0.025f : 0) + stamina / 500;

        cMChannelPerlin.AmplitudeGain = (LookingAtUi()) ? 0.45f : cMChannelPerlin.AmplitudeGain;
        cMChannelPerlin.FrequencyGain = (LookingAtUi()) ? 0.1f : cMChannelPerlin.FrequencyGain;

        firearmKnockback = (firearmKnockback > 0) ? firearmKnockback - Time.deltaTime * 25 : 0;

        explosionCam -= (explosionCam > 0) ? Time.deltaTime * 4 : 0;
        explosionCam = (explosionCam > 5) ? 5 : explosionCam;
    }
    void ShouldKeepSliding()
    {
        if (IsSliding()) sliding = true;
        if (Mathf.Abs(playerVelocity.x) + Mathf.Abs(playerVelocity.z) <= 0.5f || !isCrouching() || speedDebuff < 50f) sliding = false;
    }
    bool IsSliding()
    {
        return isRunning() && isCrouching();
    }
    void Move()
    {
        groundedPlayer = controller.isGrounded;

        //On récupère les input du joueur

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 move = new Vector3(input.x, 0, input.y);

        move = cMCam.TransformDirection(move);
        move = Vector3.ClampMagnitude(move, 1f);

        slideFrames = (!sliding) ? 2 : slideFrames - ((slideFrames > 0) ? Time.deltaTime : 0);
        move = (!sliding) ? move : oldMovement;

        oldMovement = move;

        float speedApplied = (sliding) ? speed * 1.5f * slideFrames : speed;
        speedApplied *= speedDebuff / 100;

        playerVelocity = new Vector3(move.x * speedApplied, playerVelocity.y, move.z * speedApplied);

        if (move != Vector3.zero) transform.forward = move;

        // On applique la gravité

        playerVelocity.y = (!groundedPlayer) ? playerVelocity.y + (2 * gravityValue * Time.deltaTime) : 0;

        inputMove = (move * (speedApplied / 2) * ((!sliding) ? sprintMultiplierApplied : 1));

        inputMove *= (!sliding && (isCrouching() || IsAiming())) ? crouchMultiplier : 1f;

        inputMove = new Vector3(inputMove.x, 0, inputMove.z);
        inputMove += playerVelocity.y * Vector3.up;

        controller.Move(inputMove * Time.deltaTime);

        throwingFrames -= (throwingFrames <= 0) ? 0 : Time.deltaTime;
    }
    void Stamina()
    {
        if (isRunning())
        {
            stamina += (staminaConsumption * Time.deltaTime) * 10;
            sprintMultiplierApplied = sprintMultiplier * Input.GetAxis("Sprint");
        }
        else
        {
            if (stamina >= 0) stamina -= (staminaConsumption * Time.deltaTime) * 15f;
            sprintMultiplierApplied = 1f;
        }

        //Fait exprès change pas connio

        if (stamina > maxStamina) outOfBreath = true;

        if (stamina <= 0) outOfBreath = false;

        float accuracyDebuff = stamina / 10;
        accuracyDebuff += firearmKnockback;
        accuracyDebuff += (1 / playerHealth.health) * 50;
        accuracyDebuff += (isRunning() && (inputMove.x + inputMove.y) > 1) ? 5 : 0;
        accuracyDebuff += Mathf.Round(100 * (Mathf.Abs(inputMove.x) + Mathf.Abs(inputMove.z))) / 100;
        accuracyDebuff -= (IsAiming()) ? 3 : 0;
        accuracyDebuff -= (isCrouching()) ? 1 : 0;

        accuracyDebuff = (accuracyDebuff <= -7.5f) ? -7.5f : accuracyDebuff;

        firearmAccuracy = accuracyDebuff;
    }
    void Jump()
    {
        bool onGround = groundedPlayer || Physics.Raycast(transform.position + new Vector3(0, -1, 0), transform.TransformDirection(Vector3.down), 0.25f, LayerMask.GetMask("Default"));
        if (isJumping() && (onGround || timeBeforeJumpCancel < 0.4f))
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            timeBeforeJumpCancel = 1;
        }

        timeBeforeJumpCancel = (!onGround) ? timeBeforeJumpCancel + Time.deltaTime : 0;
    }
    void BagCheck()
    {
        if (Input.GetKeyDown("tab") && !preBuild && Mathf.Abs(playerVelocity.y) < 0.4f && (!LookingAtUi() || LookingAtUi() && isLookingAtBag))
        {
            isLookingAtBag = !isLookingAtBag;
            inventory.GetComponent<Inventory>().hoveredSlot = null;
            if (!isLookingAtBag)
            {
                inventory.GetComponent<Inventory>().ExitBag(this.transform);
                foreach (Animator anim in gameInterface.hotbarSlotAnim)
                {
                    anim.SetBool("ShowSlot", false);
                }
            }
            inputMove = Vector3.zero;
            playerVelocity = Vector3.zero;
        }

        if (isLookingAtBag)
        {
            foreach (Animator anim in gameInterface.hotbarSlotAnim)
            {
                anim.SetBool("ShowSlot", true);
            }
        }
    }
    void DoorCheck()
    {
        if (isLookingAtADoor() && Input.GetKeyDown("e"))
        {
            print("JE VEUX ENTRER");
        }
    }
    void ItemCheck()
    {
        if (isLookingAtAnItem())
        {
            if (Input.GetKeyDown("e"))
            {
                Inventory inventorySystem = inventory.GetComponent<Inventory>();
                inventorySystem.AddItem(itemLookingAt);  
            }
            if (Input.GetKeyDown("f") && itemLookingAt.GetComponent<CustomItemBehaviour>())
            {
                itemLookingAt.GetComponent<ItemUsage>().ItemUse(null, itemLookingAt.GetComponent<Item>(), this);
            }  
        }
    }
    void BuildCheck()
    {
        if (isLookingAtABuild() && Input.GetKeyDown("e"))
        {
            print("JE VEUX ENTRER");
        }
    }
    void StatusCheck()
    {
        armor = inventory.GetComponent<Inventory>().totalArmor;
        thermalInsolation = inventory.GetComponent<Inventory>().totalThermal;
    }
    void HandSlots()
    {
        float mouseScroll = 0;
        int itemChoice = 0;

        mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScroll != 0)
        {
            itemChoice = (mouseScroll < 0) ? 1 : -1;
            gameInterface.ShowUpHotbar();
            gameInterface.ShowUpHotbar();

            gameInterface.HandItemDisplayUpdate();

        }
        else itemChoice = 0;

        handItemChoice += itemChoice;

        if (Input.GetKey(KeyCode.Alpha1)) handItemChoice = 0;
        if (Input.GetKey(KeyCode.Alpha2)) handItemChoice = 1;
        if (Input.GetKey(KeyCode.Alpha3)) handItemChoice = 2;

        if (handItemChoice > handsItems.Count - 1) handItemChoice = 0;
        else if (handItemChoice < 0) handItemChoice = handsItems.Count - 1;

        handItem = handsItems[handItemChoice].GetChild(0).GetComponent<Slot>().slotItem;

        if (!isLookingAtBag && Input.GetAxis("Mouse LC") != 0 && handItem != null && handItem.itemUsage != null)
        {
            handItem.itemUsage.ItemUse(handsItems[handItemChoice].GetChild(0).GetComponent<Slot>(), handItem, GetComponent<PlayerCharacterController>());
        }
    }
    public bool IsAiming()
    {
        return Input.GetAxis("Mouse RC") != 0 && !LookingAtUi();
    }
    void Starving()
    {
        int running = (isRunning()) ? 1 : 0;
        int jumping = (isJumping()) ? 1 : 0;
        int outOfBreath = (this.outOfBreath) ? 1 : 0;

        double starvingDebuff = 0;

        starvingDebuff += (starvingMultiplier / 2) + (0.75 * running) + (0.90 * jumping) + (0.25 * outOfBreath);

        hunger -= (hunger > 0) ? Time.deltaTime * starvingDebuff : 0;
        eatingFrames -= (eatingFrames > 0) ? Time.deltaTime : 0;
    }
    void EjectItem()
    {
        if (Input.GetKeyDown("q"))
        {
            if (isLookingAtBag)
            {
                Slot slotToEject;

                if (inventory.GetComponent<Inventory>().hoveredSlot != null && inventory.GetComponent<Inventory>().hoveredSlot.slotItem != null)
                {
                    slotToEject = inventory.GetComponent<Inventory>().hoveredSlot;
                    inventory.GetComponent<Inventory>().EjectItem(slotToEject, false);
                }
                else
                {
                    slotToEject = inventory.GetComponent<Inventory>().ItemSelectedSlot;
                    inventory.GetComponent<Inventory>().EjectItem(slotToEject);
                }
            }
            else
            {
                inventory.GetComponent<Inventory>().EjectItem(handsItems[handItemChoice].GetChild(0).GetComponent<Slot>(), false);
            }
        }
    }
    void Crouch()
    {
        controller.height = (isCrouching()) ? 2 - (0.6f * Input.GetAxis("Crouch")) : 2;
    }
    bool isLookingAtAnItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(cMCam.transform.position, cMCam.TransformDirection(Vector3.forward), out hit, 4, itemsMask))
        {
            Debug.DrawRay(cMCam.transform.position, cMCam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            itemLookingAt = hit.transform.gameObject;
            return true;
        }
        itemLookingAt = null;
        return false;
    }
    bool isLookingAtADoor()
    {
        RaycastHit hit;
        if (Physics.Raycast(cMCam.transform.position, cMCam.TransformDirection(Vector3.forward), out hit, 4, doorsMask))
        {
            Debug.DrawRay(cMCam.transform.position, cMCam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            doorLookingAt = hit.transform.gameObject;
            return true;
        }
        doorLookingAt = null;
        return false;
    }
    bool isLookingAtABuild()
    {
        RaycastHit hit;
        if (Physics.Raycast(cMCam.transform.position, cMCam.TransformDirection(Vector3.forward), out hit, 4, buildMask))
        {
            Debug.DrawRay(cMCam.transform.position, cMCam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
            buildLookingAt = hit.transform.gameObject;
            return true;
        }
        buildLookingAt = null;
        return false;
    }
    public bool isCrouching()
    {
        return Input.GetAxis("Crouch") != 0;
    }
    public bool isRunning()
    {
        bool running = ((Input.GetAxis("Sprint") != 0 && stamina < maxStamina) && !outOfBreath && !IsFalling() && speedDebuff > 40f);
        running = (running && !(isCrouching() && sliding) && !IsAiming()) ? (inputMove.x != 0 || inputMove.z != 0) : false;
        running = (LookingAtUi()) ? false : running;
        return running;
    }
    bool isJumping()
    {
        return Input.GetAxis("Jump") != 0;
    }
    // ptite formule arduino tu coco :p v2
    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    public bool IsFalling()
    {
        if (!wasGrounded && wasGrounded != controller.isGrounded && MathF.Abs(MathF.Round(oldVerticalSpeed)) != 0 && oldVerticalSpeed < -20f)
        {
            List<string> bodyParts = new List<string>();
            bodyParts.Add("Left Leg");
            bodyParts.Add("Right Leg");

            HurtPlayer(map(Mathf.Abs(oldVerticalSpeed * 10), 20, 100, 0, 10), bodyParts, 0, 0, 0, 0.05f + oldVerticalSpeed * 10, true);
        }
        wasGrounded = controller.isGrounded;
        oldVerticalSpeed = playerVelocity.y;
        return playerVelocity.y < -8.5f;
    }
    public void HurtPlayer(float damage = 0, List<string> bodyPart = null, float chanceToBleed = 0, float chanceToScar = 0, float chanceToDestroyBandages = 0, float chanceToBroke = 0, bool damageDispersal = false)
    {
        if (invincibilityFrames <= 0)
        {
            print("Player took " + damage + " damage");

            int randomBodyPart = UnityEngine.Random.Range(0, 6);
            List<BodyPart> bodyPartSelect = new List<BodyPart>();

            Dictionary<BodyPart, float> bodyPartAmountOfDamage = new Dictionary<BodyPart, float>();

            if (bodyPart == null)
            {
                bodyPartSelect.Insert(0, playerHealth.bodyParts[randomBodyPart]);
                bodyPartAmountOfDamage.Add(playerHealth.bodyParts[randomBodyPart], damage);
            }
            else
            {
                float damageInflicted = damage;

                foreach (string partStr in bodyPart)
                {
                    foreach (BodyPart part in playerHealth.bodyParts)
                    {
                        if (part.bodyPartName.Equals(partStr))
                        {
                            float damageFragment = (bodyPart.IndexOf(partStr) + 1 >= bodyPart.Count) ? damageInflicted : UnityEngine.Random.Range(0, damageInflicted);
                            bodyPartSelect.Add(part);
                            bodyPartAmountOfDamage.Add(part, (damageDispersal) ? damageFragment : damage);
                            print("on inflige " + damageFragment + " à " + partStr + " ce qui représente " + damageFragment / damage * 100 + " des dégats totaux (" + damage + ")");
                            damageInflicted -= damageFragment;
                        }
                    }
                }
            }

            foreach (BodyPart part in bodyPartSelect)
            {
                bool bleedAttack = UnityEngine.Random.Range(0, 100) < chanceToBleed * 100;
                bool scarAttack = UnityEngine.Random.Range(0, 100) < chanceToScar * 100;
                bool bandageAttack = UnityEngine.Random.Range(0, 100) < chanceToDestroyBandages * 100;
                bool brokeAttack = UnityEngine.Random.Range(0, 100) < chanceToBroke * 100;

                part.bodyPartHealth -= bodyPartAmountOfDamage[part];

                if (scarAttack)
                {
                    part.bodyPartState = BodyPartState.SCARED;
                    part.isBleeding = (!part.GetComponent<BodyPart>().isBleeding) ? bleedAttack : true;
                    print(part.bodyPartName + " Scar");
                }
                if (brokeAttack)
                {
                    part.bodyPartState = BodyPartState.BROKEN;
                    print(part.bodyPartName + " broken");
                }
                if (bandageAttack && part.GetComponent<Medicine>())
                {
                    Destroy(part.GetComponent<Medicine>());
                    print(part.bodyPartName + "bandage destroyed");
                }

                firearmKnockback += 5;
                invincibilityFrames = 1f;
            }
        }
    }
    public void InvincibilityFrames()
    {
        if (invincibilityFrames > 0)
            invincibilityFrames -= Time.deltaTime;
    }
    public void HealPlayer(string bodyPartToHeal = null, Medicine medicine = null)
    {
        if (bodyPartToHeal == null || medicine == null) return;

        print("Partie à soigner : " + bodyPartToHeal);

        switch (medicine.medicineType)
        {
            case "Bandaid":
                print("on utilise un bandage");
                foreach (BodyPart bodyPart in playerHealth.bodyParts)
                {
                    if (bodyPart.bodyPartName.Contains(bodyPartToHeal))
                    {
                        print("On soigne : " + bodyPart.bodyPartName);

                        if (bodyPart.medicineApplied == null)
                        {
                            Medicine medicineOnBodyPart = transform.AddComponent<Medicine>();
                            medicineOnBodyPart.medicineType = medicine.medicineType;

                            medicine.gameObject.GetComponent<ItemUsage>().DestroyItem(handItem.gameObject.GetComponent<Slot>());
                            bodyPart.medicineApplied = medicineOnBodyPart;
                        }
                        else
                        {
                            print("On a déjà un bandage dessus");
                        }
                    }
                }
                break;
            case "Desinfectant":
                foreach (BodyPart bodyPart in playerHealth.bodyParts)
                {
                    if (bodyPart.bodyPartName.Contains(bodyPartToHeal))
                    {
                        print("On soigne : " + bodyPart.bodyPartName);
                        bodyPart.desinfectantApplied = 100;
                    }
                }
                break;
        }
    }
    void Build()
    {
        wantToBuild = ((Input.GetKeyDown("v") && ((!LookingAtUi()) || (LookingAtUi() && wantToBuild))) || Input.GetKeyDown("escape") && (LookingAtUi() && wantToBuild)) ? !wantToBuild : wantToBuild;
        preBuild = (Input.GetKeyDown("escape") && preBuild) ? !preBuild : preBuild;
    }
    public bool LookingAtUi()
    {
        bool uiLook = isLookingAtBag || wantToBuild || wantToHeal || wantToCheckBody || gameInterface.gamePaused;

        return uiLook;
    }
    public void AddExplosion(float intensity)
    {
        explosionCam += intensity;
    }
    void CheckOnBody()
    {
        bool checkBody = ((Input.GetKeyDown("h") && !isLookingAtBag && !wantToHeal) || Input.GetKeyDown("escape") && (LookingAtUi() && wantToCheckBody)) ? !wantToCheckBody : wantToCheckBody;
        if (checkBody != wantToCheckBody) gameInterface.bodyPartToCheck = playerHealth.bodyParts[2];
        wantToCheckBody = checkBody;
    }
    void PlayerAudio()
    {
        float timeNeeded = timeBetweenStep + randomStepTime;
        timeNeeded -= (isRunning()) ? timeBetweenRunningStep : 0;
        timeNeeded += (isCrouching()) ? timeBetweenRunningStep : 0;
        timeNeeded /= 100;

        if ((inputMove.x != 0 || inputMove.z != 0) && controller.isGrounded && !LookingAtUi() && !sliding)
        {
            if (stepTime >= timeNeeded)
            {
                var stepSound = new GameObject();

                stepSound.transform.name = "StepSoundEffect";
                stepSound.transform.position = transform.position;
                stepSound.transform.parent = soundEffectFolder;

                AudioSource stepSoundAudio = stepSound.AddComponent<AudioSource>();
                Sound sound = stepSound.AddComponent<Sound>();

                sound.soundRadius = (isRunning()) ? 30 : (isCrouching()) ? 10 : 20;

                StartCoroutine(sound.KissYourself(1));

                stepSoundAudio.pitch = UnityEngine.Random.Range(100, 250) / 100;
                stepSoundAudio.volume = 0.15f;
                stepSoundAudio.clip = this.stepSound;

                stepSoundAudio.Play();

                sound.SoundStart();

                stepTime = 0;
                timeNeeded = UnityEngine.Random.Range(-20, 20);
            }
        }
        stepTime += (stepTime >= timeNeeded) ? 0 : Time.deltaTime;
        stepTime = (inputMove.x + inputMove.z == 0) ? 0 : stepTime;
    }
    public void MeleeAttack(float damage = 1, bool meleeWeapon = false)
    {
        if (!LookingAtUi() && !isRunning() && Input.GetKeyDown(KeyCode.Mouse0) && ((!handItem && !onMeleeCooldown) || meleeWeapon))
        {
            SendMeleeDamage(damage);

            PlayerAnimations playerAnimations = GameObject.Find("UI").GetComponent<PlayerAnimations>();
            if(!meleeWeapon)
            playerAnimations.MeleeAnimationHands();
            else 
            playerAnimations.MeleeAnimationWeapon();

        }
    }
    public void SendMeleeDamage(float meleeDamage)
    {
        onMeleeCooldown = true;
        RaycastHit hit;
        if (Physics.BoxCast(cMCam.transform.position, new Vector3(0.25f, 0.5f, 0.25f), cMCam.transform.forward, out hit) && hit.transform.GetComponent<Shootable>() && Vector3.Distance(cMCam.transform.position, hit.transform.position) < 3)
        {
            hit.transform.GetComponent<Shootable>().shootInteraction(meleeDamage, cMCam.transform.position, 15);
        }
        StartCoroutine(MeleeCooldown(0.5f));
    }
    IEnumerator MeleeCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        onMeleeCooldown = false;
    }
    void BodyPartDebuffGestion()
    {
        float speedBaseDebuff = 100f;
        //float meleeDamageBaseDebuff = 100f;
        //float meleeSpeedBaseDebuff = 100f;

        //Get all lowerBodyParts debuff (non c'est pas une ia qui écrit ça c'est vrm moi juste je suis trop english)
        List<BodyPart> lowerBodyparts = new List<BodyPart>() { findPlayerBodyPartByStr("Right Leg"), findPlayerBodyPartByStr("Left Leg") };

        foreach (BodyPart part in lowerBodyparts)
        {
            speedBaseDebuff -= (part.bodyPartState == BodyPartState.BROKEN) ? 40 : Mathf.Abs(part.bodyPartHealth - 100) / 3;
        }

        //Apply all debufs
        speedDebuff = speedBaseDebuff;
    }
    BodyPart findPlayerBodyPartByStr(string bodyPartStr)
    {
        foreach (BodyPart part in playerHealth.bodyParts)
        {
            if (part.bodyPartName.Equals(bodyPartStr))
            {
                return part;
            }
        }
        return null;
    }
}