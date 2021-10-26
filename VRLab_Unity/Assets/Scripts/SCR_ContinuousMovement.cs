using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SCR_ContinuousMovement : MonoBehaviour
{
    public XRNode inputSource;
    public float characterSpeed = 1f;
    public float gravityScale = 1f;
    public LayerMask groundLayer;
    public float additionalHeight = 0f;

    private float fallingSpeed;
    private Vector2 inputAxis;
    private CharacterController characterController;
    private XRRig rig;

    [SerializeField] private bool isGrounded;

    private void Start()
    {
        characterController = this.GetComponent<CharacterController>();
        rig = this.GetComponent<XRRig>();
    }

    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        CapsuleFollowHead();

       /* Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);

        characterController.Move(direction * Time.fixedDeltaTime * characterSpeed);*/

        //gravity
        isGrounded = CheckIfGrounded();
        if (isGrounded)
        {
            fallingSpeed = 0;
        }
        else
        {
            fallingSpeed += (Physics.gravity.y * gravityScale) * Time.fixedDeltaTime;
        }

        //Never have to move func in the same update !!!
        //characterController.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);

    }

    private void CapsuleFollowHead()
    {
        characterController.height = rig.cameraInRigSpaceHeight + additionalHeight;

        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        characterController.center = new Vector3(capsuleCenter.z, characterController.height * 0.5f + characterController.skinWidth, capsuleCenter.z);
    }

    //Check for ground using spherecast
    private bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(characterController.center);
        float rayLengh = characterController.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, characterController.radius, Vector3.down, out RaycastHit hitInfo, rayLengh, groundLayer);
        return hasHit;
    }
}
