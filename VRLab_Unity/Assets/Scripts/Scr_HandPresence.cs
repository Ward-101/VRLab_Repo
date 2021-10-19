using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mirror;
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
                NetworkServer.Spawn(spawnedController);
            }

            spawnedHandModel = Instantiate(handModelPrefab, this.transform);
            NetworkServer.Spawn(spawnedHandModel);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
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
}
