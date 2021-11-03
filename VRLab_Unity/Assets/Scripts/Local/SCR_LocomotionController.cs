using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
namespace Local
{
    public class SCR_LocomotionController : MonoBehaviour
    {
        public XRController teleportRay;
        public XRController rightHand;
        public XRController lefttHand;
        public InputHelpers.Button teleportActivationButton;
        private InputHelpers.Button fingerButton;
        private InputHelpers.Button gripBtton;
        private InputHelpers.Button spawnButton;
        public InputHelpers.Button timerButton;

        public TimerManager timer;
        public SphereCollider leftHandColider;
        public SphereCollider rightHandColider;
        public float activationThreshold = 0.1f;
        [Header("Movement")]
        public CharacterStats playerStats;
        private float xPower = 5;
        /// <summary>
        /// If negative will invers the vertical axe
        /// </summary>
        private float yPower;
        private float zPower;

        public bool EnableTeleportRay { get; set; } = true;

        public Animator handLeftAnimator;
        public Animator handRightAnimator;
        public Transform tablefolow;

        private bool isFingerLeft, isFingerRight;
        private bool cantPressLeft, cantpressRight;
        private bool ispressLeftTrigger, isPressRightTrigger, ispressLeftGrip, isPressRightGrip, isTimerPress;
        private bool canMove;
        private Vector3 initMidle;
        private Vector3 movementMidle;
        public float deltaLeft;
        private float deltaRight;
        public Transform tableTransform;
        public Transform spawnTransform;
        private bool isPressTable;
        private bool spawnOnce;
        private TableManagment tableManagment;
        public GameObject[] objectToSpawn;
        public List<GameObject> objectSpawned = new List<GameObject>();

        public GameObject[] startSpawn;
        public GameObject _spawn;
        private GameObject prefabToSpawn;
        private int index;
        public bool activateTimer;
        public bool restart;
        private void Start()
        {

            fingerButton = playerStats.fingerButton;
            gripBtton = playerStats.gripBtton;
            spawnButton = playerStats.spawnButton;
            xPower = playerStats.xPower;
            yPower = playerStats.yPower;
            zPower = playerStats.zPower;                      

        }

        private void Update()
        {
            /*if (teleportRay)
            {
                teleportRay.gameObject.SetActive(EnableTeleportRay && CheckIfActivated(teleportRay));
            }*/
            

            InputHelpers.IsPressed(rightHand.inputDevice, fingerButton, out isPressRightTrigger);
            InputHelpers.IsPressed(rightHand.inputDevice, timerButton, out isTimerPress);
            InputHelpers.IsPressed(lefttHand.inputDevice, fingerButton, out ispressLeftTrigger);
            InputHelpers.IsPressed(lefttHand.inputDevice, gripBtton, out ispressLeftGrip);
            InputHelpers.IsPressed(rightHand.inputDevice, gripBtton, out isPressRightGrip);
            InputHelpers.IsPressed(rightHand.inputDevice, spawnButton, out isPressTable);

            if (isTimerPress )
            {
                if (!activateTimer)
                {
                    timer.StartTimer();
                    activateTimer = true;
                }
                else if (restart)
                {
                    SceneManager.LoadScene(0);
                }
            }
                if (canMove)
            {
                movementMidle = Vector3.Lerp(lefttHand.transform.localPosition, rightHand.transform.localPosition, 0.5f);
                deltaLeft = Vector3.SignedAngle(new Vector3( movementMidle.x,0,movementMidle.z),new Vector3( initMidle.x,0,initMidle.z), Vector3.up);
                transform.RotateAround(tableTransform.position, Vector3.up, deltaLeft * xPower);
                transform.position += Vector3.up * (movementMidle.y - initMidle.y) * yPower;

                // transform.position += Vector3.forward * (movementMidle.z - initMidle.z)*zSpeed;
                initMidle = movementMidle;
            }

            if (ispressLeftGrip && ispressLeftTrigger && isPressRightGrip && isPressRightTrigger)
            {
                if (!canMove)
                {
                    canMove = true;
                    initMidle = Vector3.Lerp(lefttHand.transform.localPosition, rightHand.transform.localPosition, 0.5f);
                }


            }
            else if (isPressTable)
            {
                if (!spawnOnce)
                {
                    SpawnPiece(objectToSpawn);

                    spawnOnce = true;
                }
            }
            else if (canMove)
            {
                canMove = false;
                deltaLeft = 0;
                deltaRight = 0;
            }
            else
            {
                if (!canMove)
                    if (isPressRightTrigger)
                    {
                        if (!isFingerRight)
                        {
                            if (handRightAnimator == null)
                                handRightAnimator = rightHand.GetComponent<XRController>().modelParent.GetComponentInChildren<SCR_HandPresence>().spawnedHandModel.GetComponent<Animator>();
                            isFingerRight = true;
                            rightHandColider.enabled = !isFingerRight;
                            handRightAnimator.SetBool("FingerOn", isFingerRight);
                            //StartCoroutine(TresholdRight());
                        }
                    }
                    else
                    {
                        if (isFingerRight)
                        {
                            isFingerRight = false;
                            rightHandColider.enabled = !isFingerRight;
                            handRightAnimator.SetBool("FingerOn", isFingerRight);
                        }
                    }

                if (ispressLeftTrigger)
                {
                    if (!isFingerLeft)
                    {
                        if (handLeftAnimator == null)
                            handLeftAnimator = lefttHand.GetComponent<XRController>().modelParent.GetComponentInChildren<SCR_HandPresence>().spawnedHandModel.GetComponent<Animator>();
                        isFingerLeft = true;
                        //leftHandColider.enabled = !isFingerLeft;
                        //handLeftAnimator.SetBool("FingerOn", isFingerLeft);
                    }
                }
                else
                {
                    if (isFingerLeft)
                    {
                        isFingerLeft = false;
                        leftHandColider.enabled = !isFingerLeft;
                        //handLeftAnimator.SetBool("FingerOn", isFingerLeft);
                    }
                }
            }

            if (!isPressTable && spawnOnce)
            {
                if(tablefolow.childCount ==0)
                    spawnOnce = false;
            }

        }


        private int indexStart = 0;

        public void SpawnPiece(GameObject[] peiceToSpawn)
        {
            for (int i = 0; i < peiceToSpawn.Length; i++)
            {
               _spawn = Instantiate(peiceToSpawn[i], spawnTransform);
               _spawn.transform.SetParent(spawnTransform);
            }
        }

    }

}
