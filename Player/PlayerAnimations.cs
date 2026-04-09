using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public PlayerCharacterController playerController;
    public GameInterface gameInterface;
    public Animator armsAnim;
    public Animator camAnim;
    void Update()
    {
        GestionAnimatorBools();
    }

    void GestionAnimatorBools()
    {
        armsAnim.SetBool("HoldingItem", playerController.handItem != null && !playerController.handItem.twoHanded && !playerController.wantToCheckBody && !playerController.preBuild);
        armsAnim.SetBool("HoldingItemTwoHanded", playerController.handItem != null && playerController.handItem.twoHanded && !playerController.wantToCheckBody && !playerController.preBuild);
        armsAnim.SetBool("Running", playerController.isRunning() && (playerController.inputMove.x + playerController.inputMove.z != 0));
        armsAnim.SetBool("Crouching", playerController.isCrouching());
        armsAnim.SetBool("Jumping", playerController.IsFalling());
        armsAnim.SetBool("Aiming", playerController.IsAiming());
        armsAnim.SetBool("CheckBody", playerController.wantToCheckBody);
        armsAnim.SetBool("HoldingItemOneHanded", playerController.handItem != null && playerController.handItem.oneHanded && !playerController.wantToCheckBody && !playerController.preBuild);
        armsAnim.SetBool("Sliding", playerController.sliding && !playerController.handItem);

        if(gameInterface.bodyPartToCheck)
        switch (gameInterface.bodyPartToCheck.bodyPartName)
        {
            case "Left Arm":
                armsAnim.SetInteger("BodyPart",1);
                playerController.addPan = 0;
            break;
            case "Right Arm":
                armsAnim.SetInteger("BodyPart",2);
                playerController.addPan = 0;
            break;
            case "Right Leg":
                armsAnim.SetInteger("BodyPart",3);
                playerController.addPan = 40;
            break;
            case "Left Leg":
                armsAnim.SetInteger("BodyPart",3);
                playerController.addPan = 40;
            break;
            default:
                armsAnim.SetInteger("BodyPart",0);
                playerController.addPan = 0;
            break;
        }
        else{ 
            armsAnim.SetInteger("BodyPart",0);
            playerController.addPan = 0;
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
}
