using UnityEngine;
using System.Collections;
using Unity.VisualScripting.AssemblyQualifiedNameParser;

public class ItemUsage : MonoBehaviour
{
    public string itemType;
    public Transform cMCam;
    public Transform player;
    public GameObject bulletCollisionParticle;
    private PlayerAnimations playerAnimations;

    void Start()
    {
        cMCam = GameObject.Find("Main Cinemachine Camera").transform;
        player = GameObject.Find("Player").transform;
        playerAnimations = GameObject.Find("UI").GetComponent<PlayerAnimations>();
    }
    public void ItemUse(Slot slot = null, Item item = null, PlayerCharacterController player = null)
    {
        if ((item.oneHanded || !item.oneHanded && !item.twoHanded) && player.IsAiming())
        {
            throwItem(slot, item, player);
            return;
        }
        if (item.customItemBehaviour)
        {
            item.customItemBehaviour.OnActivate();
            return;
        }
        switch (itemType)
        {
            case "Consommable":
                Eat(slot, item, player);
                break;

            case "Armor":
                ReplaceArmorItem(item, player, slot);
                break;

            case "Melee":
                MeleeAttack(player);
                break;

            case "Firearm":
                if (player.IsAiming())
                    Shoot(player);
                break;

            case "Medicine":
                player.wantToHeal = true;
                break;
        }
    }
    void ReplaceArmorItem(Item item, PlayerCharacterController player, Slot slotUsed)
    {
        Inventory playerInv = player.inventory.GetComponent<Inventory>();

        if (playerInv.equippedSlots.Contains(slotUsed.gameObject))
        {
            foreach (GameObject slot in playerInv.slots)
            {
                if (slot.GetComponent<Slot>().slotItem == null)
                {
                    playerInv.SlotClick(slot.transform, slotUsed);

                    return;
                }
            }

            playerInv.EjectItem(slotUsed);
        }
        else foreach (GameObject slot in playerInv.equippedSlots)
        {
            if (playerInv.TypeCorrespondanceCheck(item, slot.GetComponent<Slot>().item_type))
            {
                playerInv.SlotClick(slot.transform, slotUsed);

                return;
            }
        }
    }
    public void DestroyItem(Slot slot)
    {
        if (slot.slotItem == null) return;
        if (slot.slotItem.itemNumber >= 2)
        {
            slot.slotItem.itemNumber -= 1;
            return;
        }
        else
        {
            if (slot.slotItem.armor != null) Destroy(slot.slotItem.armor);
            if (slot.slotItem.consommable != null) Destroy(slot.slotItem.consommable);
            if (slot.slotItem.itemUsage != null) Destroy(slot.slotItem.itemUsage);
            if (slot.slotItem.firearm != null) Destroy(slot.slotItem.firearm);

            Destroy(slot.slotItem);
        }
    }
    public void Eat(Slot slot, Item item, PlayerCharacterController player)
    {
        if (player.hunger < 150 && player.eatingFrames <= 0)
        {
            player.hunger += item.consommable.nutritiousValue;
            playerAnimations.EatAnimation();
            DestroyItem(slot);
            player.eatingFrames += 1;
        }
        else
        {
            print("jié trop mangé");
        }
    }
    public void throwItem(Slot slot, Item item, PlayerCharacterController player)
    {
        if (player.throwingFrames <= 0)
        {
            var throwedItem = player.inventory.GetComponent<Inventory>().EjectItem(slot, false);
            throwedItem.GetComponent<Rigidbody>().AddForce(player.cMCam.transform.forward * (1 / (float)item.baseWeight) * 100);
            player.throwingFrames = 1;
        }
    }
    void MeleeAttack(PlayerCharacterController player)
    {
        MeleeWeapon meleeWeapon = GetComponent<MeleeWeapon>();
        if (meleeWeapon.canShoot)
        {
            meleeWeapon.canShoot = false;
            StartCoroutine(meleeWeapon.betweenShotsWaitTime());
            player.MeleeAttack((float)meleeWeapon.damage, true);
        }

    }
    public void Shoot(PlayerCharacterController player)
    {
        Firearm gun = GetComponent<Firearm>();

        if (gun.ammo > 0 && gun.canShoot)
        {
            for (int i = 0; i < gun.bulletToFire; i++)
            {
                RaycastHit hit;

                float gunAccuracy = player.GetComponent<PlayerCharacterController>().firearmAccuracy;
                gunAccuracy += (1 / gun.accuracy) * 1000;
                gunAccuracy = (gunAccuracy <= 0) ? Random.Range(-1f, 1f) / 100 : Random.Range(-gunAccuracy, gunAccuracy) / 100;

                if (Physics.Raycast(cMCam.transform.position, cMCam.TransformDirection(Vector3.forward + new Vector3(gunAccuracy, 0, 0)), out hit, Mathf.Infinity))
                {
                    Debug.DrawRay(cMCam.transform.position, cMCam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
                    print("J'ai touché " + hit.transform.name);
                    if (hit.transform.GetComponent<Shootable>() != null || (hit.transform.parent != null && hit.transform.parent.GetComponent<Shootable>()))
                    {
                        if (hit.transform.GetComponent<Shootable>())
                            hit.transform.GetComponent<Shootable>().shootInteraction((float)gun.damage, hit.point, player.transform.position);
                        else
                            hit.transform.parent.GetComponent<Shootable>().shootInteraction((float)gun.damage, hit.point, player.transform.position);
                    }
                    
                    GameObject hitInst = Instantiate(bulletCollisionParticle);

                    hitInst.transform.position = hit.point;
                    hitInst.transform.name = "hitPoint";

                    hitInst.transform.rotation = player.GetComponent<PlayerCharacterController>().cMCam.rotation;
                }

                player.GetComponent<PlayerCharacterController>().firearmKnockback += Random.Range(gun.knockback / 2, gun.knockback);

                i++;
            }

            var fireSound = new GameObject();

            fireSound.transform.name = "fireSoundEffect";
            fireSound.transform.position = cMCam.transform.position;
            fireSound.transform.parent = GameObject.Find("SoundEffect").transform;

            AudioSource fireSoundAudio = fireSound.AddComponent<AudioSource>();
            Sound sound = fireSound.AddComponent<Sound>();
            sound.soundRadius = gun.soundRadius;

            StartCoroutine(sound.KissYourself(10));

            fireSoundAudio.clip = gun.firearmShootingSound;

            fireSoundAudio.Play();
            sound.SoundStart();

            gun.ammo -= 1;
            gun.canShoot = false;

            GameObject.Find("UI").GetComponent<PlayerAnimations>().ShootAnim();

            StartCoroutine(gun.betweenShotsWaitTime());

            //Particules

            StartCoroutine(gun.bulletShellEject(player.transform, cMCam, gun.bulletShellEjectTiming));

            var muzzleFlashParticle = Instantiate(gun.muzzleFlashParticle);
            muzzleFlashParticle.gameObject.transform.position = player.transform.position + cMCam.transform.forward * gun.muzzleFlashDistance;
            muzzleFlashParticle.gameObject.transform.rotation = cMCam.transform.rotation;

            player.AddExplosion(gun.knockback / 5);
        }
    }
}
