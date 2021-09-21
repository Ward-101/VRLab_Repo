using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SCR_LocomotionController : MonoBehaviour
{
    public XRController teleportRay;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;

    public bool EnableTeleportRay { get; set; } = true;

    private void Update()
    {
        if (teleportRay)
        {
            teleportRay.gameObject.SetActive(EnableTeleportRay && CheckIfActivated(teleportRay));
        }
    }

    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    } 
}
