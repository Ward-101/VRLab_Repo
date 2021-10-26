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
    public InputHelpers.Button fingerButton;
    public SphereCollider leftHandColider;
    public SphereCollider rightHandColider;
    public float activationThreshold = 0.1f;

    public bool EnableTeleportRay { get; set; } = true;

    public Animator handLeftAnimator;
    public Animator handRightAnimator;

    private bool isFingerLeft, isFingerRight;
    private bool cantPressLeft, cantpressRight;
    private void Update()
    {
        /*if (teleportRay)
        {
            teleportRay.gameObject.SetActive(EnableTeleportRay && CheckIfActivated(teleportRay));
        }*/
        if (rightHand)
        {
            InputHelpers.IsPressed(rightHand.inputDevice, fingerButton, out bool isPressed);
            if (isPressed && !cantpressRight)
            {
                if (handRightAnimator == null)
                    handRightAnimator = rightHand.GetComponent<XRController>().modelParent.GetComponentInChildren<SCR_HandPresence>().spawnedHandModel.GetComponent<Animator>();
                isFingerRight = !isFingerRight;
                rightHandColider.enabled = !isFingerRight;
                handRightAnimator.SetBool("FingerOn", isFingerRight);
                StartCoroutine(TresholdRight());
            }
        }
        if (lefttHand)
        {
            InputHelpers.IsPressed(lefttHand.inputDevice, fingerButton, out bool isPressed);
            if (isPressed && !cantPressLeft)
            {
                if (handLeftAnimator == null)
                    handLeftAnimator = lefttHand.GetComponent<XRController>().modelParent.GetComponentInChildren<SCR_HandPresence>().spawnedHandModel.GetComponent<Animator>();
                isFingerLeft = !isFingerLeft;
                leftHandColider.enabled = !isFingerLeft;
                handLeftAnimator.SetBool("FingerOn", !handLeftAnimator.GetBool("FingerOn"));
                StartCoroutine(TresholdLeft());
            }
        }

    }
    IEnumerator TresholdRight()
    {
        cantpressRight = true;
        yield return new WaitForSeconds(0.2f);
        cantpressRight = false;

    }
    IEnumerator TresholdLeft()
    {
        cantPressLeft = true;
        yield return new WaitForSeconds(0.2f);
        cantPressLeft = false;

    }

    /*public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    } */
}
