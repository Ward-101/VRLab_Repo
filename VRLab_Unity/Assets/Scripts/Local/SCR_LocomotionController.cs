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
        public InputHelpers.Button spawnButton2;
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
        private bool isPressTable, isPressTable2;
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

        [Header("PlayerStatsForMovement")]
        public Transform vrHeadSett;
        public float upOffset;
        public Vector3 forwardOffset;
        public Vector3 rotateOffset;
        public Vector3 deltaPos;
        public Vector3 tdeltaPos;
        public Transform zClamp;
        public Transform upClamp;
        public Transform downClamp;

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
            InputHelpers.IsPressed(rightHand.inputDevice, spawnButton2, out isPressTable2);

            if (isTimerPress)
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
                Movement();
                initMidle = movementMidle;
            }

            if (ispressLeftGrip && ispressLeftTrigger && isPressRightGrip && isPressRightTrigger)
            {
                if (!canMove)
                {
                    canMove = true;
                    initMidle = Vector3.Lerp(lefttHand.transform.position, rightHand.transform.position, 0.5f);
                }


            }
            else if (isPressTable || isPressTable2)
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
                if (spawnTransform.childCount == 0)
                    spawnOnce = false;
            }

        }

        private void Movement()
        {

            movementMidle = Vector3.Lerp(lefttHand.transform.position, rightHand.transform.position, 0.5f);

            //get delat pos 
            deltaPos = new Vector3((movementMidle.x - initMidle.x), (movementMidle.y - initMidle.y), (movementMidle.z - initMidle.z));
            //get delta on player forward referential
            //neeed to find a better idea
            // tdeltaPos = vrHeadSett.transform.right * deltaPos.x+ vrHeadSett.transform.up * deltaPos.y+ vrHeadSett.transform.forward * deltaPos.z;
            //tdeltaPos = vrHeadSett.InverseTransformPoint(deltaPos);
            // tdeltaPos = vrHeadSett.InverseTransformPoint(deltaPos);

            //seems good, 
            tdeltaPos = vrHeadSett.InverseTransformDirection(deltaPos);


            //if the amplutde of the movemnt is mor forward then side
            if (Mathf.Abs(tdeltaPos.x) < Mathf.Abs(tdeltaPos.z))
            {
                // if the movemnt is enough to move
                if (forwardOffset.z < Mathf.Abs(tdeltaPos.z))
                {
                    Debug.Log("moveforward");
                    //move the pos to the table
                    transform.position += new Vector3(tableTransform.transform.position.x - vrHeadSett.transform.position.x, 0, tableTransform.transform.position.z - vrHeadSett.transform.position.z).normalized * tdeltaPos.z * zPower;
                    // the movement is enough to move forward and/or move upward
                    if (forwardOffset.y < Mathf.Abs(tdeltaPos.y))
                    {
                        transform.position += Vector3.up * (tdeltaPos.y) * yPower;
                    }
                }
                
                //else if can move up
                else if (upOffset < Mathf.Abs(deltaPos.y))
                {
                    transform.position += Vector3.up * (tdeltaPos.y) * yPower;
                }

            }

            else if (Mathf.Abs(tdeltaPos.x) > Mathf.Abs(tdeltaPos.z))
            {
                // if the movemnt is enough to rotate
                if (rotateOffset.x < Mathf.Abs( tdeltaPos.x))
                {
                    Debug.Log("rotate");

                    //move the pos to the table
                    transform.RotateAround(tableTransform.position, Vector3.up, tdeltaPos.x *180* xPower);
                    // the movement is enough to move forward and/or move upward
                    if (rotateOffset.y < tdeltaPos.y)
                    {
                        transform.position += Vector3.up * (tdeltaPos.y) * yPower;
                    }
                }
                //else if can move up
                else if (upOffset < Mathf.Abs(tdeltaPos.y))
                {
                    transform.position += Vector3.up * (deltaPos.y) * yPower;
                }
            }
            //in the weird case that the player move the same amount in x and in z
            //if move enough in y axis
            else
            {
                Debug.Log("moveup");
                
                if (upOffset < Mathf.Abs(tdeltaPos.y))
                {
                    transform.position += Vector3.up * tdeltaPos.y * yPower;
                }
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
