using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SCR_LocomotionController : MonoBehaviour
{
    public XRController teleportRay;
    public XRController rightHand;
    public XRController lefttHand;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;

    public bool EnableTeleportRay { get; set; } = true;

    public Animator handLeftAnimator;
    public Animator handRightAnimator;
    private bool cantPressLeft, cantpressRight;
    private void Update()
    {
        if (teleportRay)
        {
            teleportRay.gameObject.SetActive(EnableTeleportRay && CheckIfActivated(teleportRay));
        }
        if (rightHand)
        {
            InputHelpers.IsPressed(rightHand.inputDevice, InputHelpers.Button.Primary2DAxisClick, out bool isPressed);
            if (isPressed && !cantpressRight)
            {
                if (handRightAnimator == null)
                    handRightAnimator = rightHand.GetComponent<XRController>().modelParent.GetComponentInChildren<SCR_HandPresence>().spawnedHandModel.GetComponent<Animator>();
                handRightAnimator.SetBool("FingerOn", !handRightAnimator.GetBool("FingerOn"));
                cantpressRight = true;

                StartCoroutine(Treshold(cantpressRight));
            }
        }
        if (lefttHand)
        {
            InputHelpers.IsPressed(lefttHand.inputDevice, InputHelpers.Button.Primary2DAxisClick, out bool isPressed);
            if (isPressed && !cantPressLeft)
            {
                if (handLeftAnimator == null)
                    handLeftAnimator = lefttHand.GetComponent<XRController>().modelParent.GetComponentInChildren<SCR_HandPresence>().spawnedHandModel.GetComponent<Animator>();
                handLeftAnimator.SetBool("FingerOn", !handRightAnimator.GetBool("FingerOn"));
                cantPressLeft = true;
                StartCoroutine(Treshold(cantPressLeft));
            }
        }

    }
    IEnumerator Treshold(bool canPress)
    {
        yield return new WaitForSeconds(0.5f);
        canPress = false;

    }

    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    } 
}
