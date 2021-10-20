using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SCR_HandPresence : MonoBehaviour
{
    public bool showController = false;
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;

    private GameObject spawnedHandModel;
    private InputDevice targetDevice;
    private GameObject spawnedController;
    private Animator handAnimator;

    private Collider handCollider;

    private void Start()
    {
        TryInitialize();
    }


    private void Update()
    {
        if (!targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            if (showController)
            {
                if (spawnedHandModel.activeSelf)
                {
                    spawnedHandModel.SetActive(false);
                    spawnedController.SetActive(true);
                }
            }
            else
            {
                if (spawnedController.activeSelf)
                {
                    spawnedController.SetActive(false);
                    spawnedHandModel.SetActive(true);
                }

                UpdateHandAnimation();
                UpdateHandCollision();
            }
        }
    }

    private void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach (var item in devices)
        {
            print(item.name + item.characteristics);
        }

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
            {
                spawnedController = Instantiate(prefab, this.transform);
            }
            else
            {
                Debug.LogError("Did not find corresponding controller model");
                spawnedController = Instantiate(controllerPrefabs[0], this.transform);
            }

            spawnedHandModel = Instantiate(handModelPrefab, this.transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
            handCollider = GetComponentInParent<Collider>();
        }
    }

    private void UpdateHandAnimation()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue) && gripValue > 0.1f)
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    /// <summary>
    /// Très shetan tier à enlever au plus vite 
    /// </summary>
    private void UpdateHandCollision()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.5f && targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue) && gripValue == 0)
        {
            handCollider.isTrigger = false;
        }
        else
        {
            handCollider.isTrigger = true;
        }
    }
}
