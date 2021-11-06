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
        private float upOffset;
        private Vector3 forwardOffset;
        private Vector3 rotateOffset;
        private Vector3 deltaPos;
        public Transform zClampMax;
        public Transform zClampMin;
        public Transform upClamp;
        public Transform downClamp;
        public Renderer tableRenderer;
        private void Start()
        {
            tableRenderer = tableTransform.GetComponentInChildren<Renderer>();
            fingerButton = playerStats.fingerButton;
            gripBtton = playerStats.gripBtton;
            spawnButton = playerStats.spawnButton;
            xPower = playerStats.xPower;
            yPower = playerStats.yPower;
            zPower = playerStats.zPower;
            forwardOffset = new Vector3(0, playerStats.forwardYOffset, playerStats.forwardZOffset);
            rotateOffset = new Vector3(playerStats.RotateXOffset, playerStats.RotateYOffset,0);
            upOffset = playerStats.upOffset;
        }
        private void FixedUpdate()
        {
            
            if (canMove)
            {
                Movement();
                initMidle = movementMidle;
            }
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
            

            if (ispressLeftGrip && ispressLeftTrigger && isPressRightGrip && isPressRightTrigger)
            {
                if (!canMove)
                {
                    canMove = true;
                    initMidle = Vector3.Lerp(lefttHand.transform.localPosition, rightHand.transform.localPosition, 0.5f);
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
            //check if table is on screen
            Vector3 pointOnScreen = vrHeadSett.GetComponent<Camera>().WorldToScreenPoint(tableRenderer.bounds.center);
            if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
                (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
            {
                return;
            }
            if (pointOnScreen.z < 0)
            {
                return;
            }
            movementMidle = Vector3.Lerp(lefttHand.transform.localPosition, rightHand.transform.localPosition, 0.5f);

            //get delat pos 
            deltaPos = new Vector3((movementMidle.x - initMidle.x), (movementMidle.y - initMidle.y), (movementMidle.z - initMidle.z));
            //get delta on player forward referential
            //neeed to find a better idea
             //tdeltaPos = transform.transform.right * deltaPos.x+ transform.transform.up * deltaPos.y+ transform.transform.forward * deltaPos.z;
            //tdeltaPos = vrHeadSett.InverseTransformPoint(deltaPos);
             //tdeltaPos = vrHeadSett.InverseTransformPoint(deltaPos);

            //seems good, 
           //tdeltaPos = transform.InverseTransformDirection(deltaPos);
           

            //if the amplutde of the movemnt is mor forward then side
            if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.z))
            {
               
                // if the movemnt is enough to move
                if (forwardOffset.z < Mathf.Abs(deltaPos.z) )
                {
                    //if is to close and want to come closer, return
                    if (Vector2.Distance(ToVector2XZ(vrHeadSett.position), ToVector2XZ(tableTransform.position)) < Vector2.Distance(ToVector2XZ(zClampMin.position), ToVector2XZ(tableTransform.position)) && deltaPos.z < 0)
                    {
                        return;
                    }
                    //if is to far and want to go farer (is that english ?)
                    else if (Vector2.Distance(ToVector2XZ(vrHeadSett.position), ToVector2XZ(tableTransform.position)) > Vector2.Distance(ToVector2XZ(zClampMax.position), ToVector2XZ(tableTransform.position)) && deltaPos.z > 0)
                        return;
                    //move the pos to the table
                    transform.position += new Vector3(tableTransform.position.x - vrHeadSett.transform.position.x, 0, tableTransform.position.z - vrHeadSett.transform.position.z).normalized * deltaPos.z * zPower;
                    // the movement is enough to move forward and/or move upward
                    MoveUp(forwardOffset);
                }
                 //else if can move up
                else
                {
                    MoveUp(upOffset);

                }

            }
            else if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.z))
            {
                // if the movemnt is enough to rotate
                if (rotateOffset.x < Mathf.Abs(deltaPos.x))
                {
                    //move the pos to the table
                    transform.RotateAround(tableTransform.position, Vector3.up, deltaPos.x * 180 * xPower);
                    // the movement is enough to move forward and/or move upward
                    MoveUp(rotateOffset);
                }
                //else if can move up
                else 
                {
                    MoveUp(upOffset);
                }
            }
            //in the weird case that the player move the same amount in x and in z
            //if move enough in y axis
            else
            {
                MoveUp(upOffset);
            }
        }

        private void MoveUp(Vector3 offset)
        {
           
                if (vrHeadSett.position.y - upClamp.position.y > 0 && deltaPos.y < 0)
                    return;
                
                else if (vrHeadSett.position.y - downClamp.position.y < 0 && deltaPos.y > 0)
                    return;

                transform.position += Vector3.up * (deltaPos.y) * yPower;
            
        }
        private void MoveUp(float offset)
        {
            
                if (vrHeadSett.position.y - upClamp.position.y > 0 && deltaPos.y < 0)
                    return;
                else if (vrHeadSett.position.y - downClamp.position.y < 0 && deltaPos.y > 0)
                    return;
                transform.position += Vector3.up * (deltaPos.y) * yPower;
        }
        private Vector2 ToVector2XZ(Vector3 a)
        {
            return new Vector2(a.x, a.z);
        }

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
