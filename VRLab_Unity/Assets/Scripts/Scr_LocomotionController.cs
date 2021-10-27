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
    public InputHelpers.Button gripBtton;
    public SphereCollider leftHandColider;
    public SphereCollider rightHandColider;
    public float activationThreshold = 0.1f;
    [Header("Movement")]
    public float anglePower=5;
    /// <summary>
    /// If negative will invers the vertical axe
    /// </summary>
    public float ySpeed;

    public bool EnableTeleportRay { get; set; } = true;

    public Animator handLeftAnimator;
    public Animator handRightAnimator;

    private bool isFingerLeft, isFingerRight;
    private bool cantPressLeft, cantpressRight;
    private bool ispressLeftTrigger, isPressRightTrigger, ispressLeftGrip, isPressRightGrip;
    private bool canMove;
    private Vector3 initMidle;
    private Vector3 movementMidle;
    public float deltaLeft;
    private float deltaRight;
    private Transform tableTransform;
    private void Start()
    {
        tableTransform = GameObject.FindGameObjectWithTag("Table").transform;
    }
    private void Update()
    {
        /*if (teleportRay)
        {
            teleportRay.gameObject.SetActive(EnableTeleportRay && CheckIfActivated(teleportRay));
        }*/
       
            InputHelpers.IsPressed(rightHand.inputDevice, fingerButton, out isPressRightTrigger);
            InputHelpers.IsPressed(lefttHand.inputDevice, fingerButton, out  ispressLeftTrigger);
            InputHelpers.IsPressed(lefttHand.inputDevice, gripBtton, out  ispressLeftGrip);
            InputHelpers.IsPressed(rightHand.inputDevice, gripBtton, out  isPressRightGrip);

        if (canMove)
        {
            movementMidle =  Vector3.Lerp(lefttHand.transform.localPosition, rightHand.transform.localPosition, 0.5f);
            deltaLeft = Vector3.SignedAngle(movementMidle,initMidle,Vector3.up);
            transform.RotateAround(tableTransform.position, Vector3.up, deltaLeft*anglePower);
            transform.position += Vector3.up * (movementMidle.y - initMidle.y)*ySpeed;
            initMidle = movementMidle;
        }

        if (ispressLeftGrip && ispressLeftTrigger && isPressRightGrip && isPressRightTrigger )
        {
            if (!canMove)
            {
                canMove = true;
                initMidle = Vector3.Lerp(lefttHand.transform.localPosition,rightHand.transform.localPosition, 0.5f);
            }


        }
        else if(canMove)
        {
            canMove = false;
            deltaLeft = 0;
            deltaRight = 0;
        }
        else
        {
            if (!canMove)
                if (isPressRightTrigger && !cantpressRight)
                {
                    if (handRightAnimator == null)
                        handRightAnimator = rightHand.GetComponent<XRController>().modelParent.GetComponentInChildren<SCR_HandPresence>().spawnedHandModel.GetComponent<Animator>();
                    isFingerRight = !isFingerRight;
                    rightHandColider.enabled = !isFingerRight;
                    handRightAnimator.SetBool("FingerOn", isFingerRight);
                    StartCoroutine(TresholdRight());
                }

            if (ispressLeftTrigger && !cantPressLeft)
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
        yield return new WaitUntil(() => !isPressRightTrigger);
        cantpressRight = false;

    }
    IEnumerator TresholdLeft()
    {
        cantPressLeft = true;
        yield return new WaitUntil(()=>!ispressLeftTrigger);
        cantPressLeft = false;

    }

    /*public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    } */
}
