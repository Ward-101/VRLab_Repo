using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mirror;

namespace Network
{
    public class SCR_HandPresence : NetworkBehaviour
    {
        public bool showController = false;
        public InputDeviceCharacteristics controllerCharacteristics;
        public List<GameObject> controllerPrefabs;
        public GameObject handModelPrefab;

        public SCR_LocomotionController parent;
        public GameObject spawnedHandModel;
        private InputDevice targetDevice;
        private GameObject spawnedController;
        private Animator handAnimator;
        private Collider indexCollider;
        private IEnumerator Start()
        {
            parent = GetComponentInParent<SCR_LocomotionController>();
            if (!parent.isLocalPlayer)
                this.enabled = false;
            yield return new WaitUntil(() => NetworkClient.ready);
            yield return new WaitForSeconds(0.5f);
            if (!targetDevice.isValid)
            {
                TryInitialize();
            }
        }


        private void Update()
        {
            if (!indexCollider && transform.childCount > 0)
            {
                indexCollider = transform.GetChild(0).GetComponentInChildren<Collider>();
            }
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
                else if (parent.isLocalPlayer)
                {
                    /*if (spawnedController.activeSelf)
                    {
                        spawnedController.SetActive(false);
                        spawnedHandModel.SetActive(true);
                    }*/

                    if (indexCollider)
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
                /*GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
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
    */
                spawnedHandModel = Instantiate(handModelPrefab, this.transform);
                handAnimator = spawnedHandModel.GetComponent<Animator>();
            }
            if (targetDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool triggerValue) && triggerValue)
            {
                Debug.Log("yo");
            }
        }

        private void UpdateHandAnimation()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
            {
                handAnimator.SetFloat("Trigger", triggerValue);

                if (triggerValue > 0.5f)
                {
                    indexCollider.isTrigger = false;
                }
                else
                {
                    indexCollider.isTrigger = true;
                }
            }
            else
            {
                handAnimator.SetFloat("Trigger", 0);
                indexCollider.isTrigger = true;
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

}
