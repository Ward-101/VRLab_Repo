using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Scr_HandPresence : MonoBehaviour
{
    public List<GameObject> controllerPrefabs;


    private InputDevice targetDevice;
    private GameObject spawnedController;

    private void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        //InputDevices.GetDevices(devices);
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);

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
        }
    }

    private void Update()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
        {
            print("Pressing Primary Button");
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
        {
            print("Trigger Pressed : " + triggerValue);
        }        
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue) && primary2DAxisValue.magnitude != 0f)
        {
            print("Primary Touchpad :" + primary2DAxisValue);
        }
    }
}
