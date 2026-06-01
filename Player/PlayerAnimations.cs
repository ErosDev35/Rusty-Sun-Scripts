using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public PlayerCharacterController playerController;
    public Transform passiveBleedingParticle;
    public GameInterface gameInterface;
    public Animator armsAnim;
    public Animator camAnim;
    void Update()
    {
        GestionAnimatorBools();
        PlayerBleedingPassive();
    }

    void GestionAnimatorBools()
    {
        bool isActing = gameInterface.actionName != "";

        armsAnim.SetBool("HoldingItem", playerController.handItem != null && !playerController.handItem.twoHanded && !playerController.wantToCheckBody && !playerController.preBuild && !isActing);
        armsAnim.SetBool("HoldingItemTwoHanded", playerController.handItem != null && playerController.handItem.twoHanded && !playerController.wantToCheckBody && !playerController.preBuild && !isActing);
        armsAnim.SetBool("HoldingItemOneHanded", playerController.handItem != null && playerController.handItem.oneHanded && !playerController.wantToCheckBody && !playerController.preBuild && !isActing);
        armsAnim.SetBool("HoldingBoombox", playerController.handItem != null && playerController.handItem.itemName == "Boombox" && !playerController.wantToCheckBody && !playerController.preBuild && !isActing);
        armsAnim.SetBool("Running", playerController.isRunning() && (playerController.inputMove.x + playerController.inputMove.z != 0));
        armsAnim.SetBool("Crouching", playerController.isCrouching());
        armsAnim.SetBool("Jumping", playerController.IsFalling());
        armsAnim.SetBool("Aiming", playerController.IsAiming());
        armsAnim.SetBool("CheckBody", playerController.wantToCheckBody && !isActing);
        armsAnim.SetBool("Sliding", playerController.sliding && !playerController.handItem && !isActing);
        armsAnim.SetBool("ActionTakeTime", isActing);

        if (playerController.wantToCheckBody && gameInterface.bodyPartToCheck)
        {
            switch (gameInterface.bodyPartToCheck.bodyPartName)
            {
                case "Left Arm":
                    armsAnim.SetInteger("BodyPart", 1);
                    break;
                case "Right Arm":
                    armsAnim.SetInteger("BodyPart", 2);
                    break;
                case "Right Leg":
                    armsAnim.SetInteger("BodyPart", 3);
                    break;
                case "Left Leg":
                    armsAnim.SetInteger("BodyPart", 3);
                    break;
                default:
                    armsAnim.SetInteger("BodyPart", 0);
                    break;
            }
        }
        else
        {
            armsAnim.SetInteger("BodyPart", 0);
        }

        camAnim.SetBool("Aiming", playerController.IsAiming());
    }

    public void ShootAnim()
    {
        armsAnim.Play("ShootArms");
    }
    public void EatAnimation()
    {
        armsAnim.Play("EatingArms");
    }
    public void MeleeAnimationHands()
    {
        armsAnim.Play("ArmAttackMelee");
    }
    public void MeleeAnimationWeapon()
    {
        //Tu changera plus tard quand tu aura une animation
        armsAnim.Play("ArmAttackMelee");
    }
    void PlayerBleedingPassive()
    {
        ParticleSystem ps = passiveBleedingParticle.GetComponent<ParticleSystem>();
        var em = ps.emission;
        em.enabled = gameInterface.isPlayerBleeding;
    }
}
