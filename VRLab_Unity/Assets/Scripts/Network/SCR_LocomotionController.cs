using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;
using TMPro;
namespace Network
{
    public class SCR_LocomotionController : NetworkBehaviour
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
        public GameObject table;
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
        private bool isPressTable, isPressTable2;
        private bool spawnOnce;
        private TableManagment tableManagment;
        public GameObject[] objectToSpawn;
        public List<GameObject> objectSpawned = new List<GameObject>();

        public GameObject[] startSpawn;
        public GameObject _spawn;
        private GameObject prefabToSpawn;
        private int index;
        public NetworkConnection net;
        public bool restart;

        private IEnumerator Start()
        {
            if (!isServer)
                tableTransform = GameObject.FindGameObjectWithTag("Table2").transform;
            else
                tableTransform = GameObject.FindGameObjectWithTag("Table").transform;

            fingerButton = playerStats.fingerButton;
            gripBtton = playerStats.gripBtton;
            spawnButton = playerStats.spawnButton;
            xPower = playerStats.xPower;
            yPower = playerStats.yPower;
            zPower = playerStats.zPower;


            GameObject _table = Instantiate(table, new Vector3(tablefolow.forward.x  +0.7f, 0.9f + transform.position.y,  transform.position.z+0.7f), Quaternion.identity);
            _table.transform.SetParent(transform);
            tableManagment = _table.GetComponent<TableManagment>();
            tableManagment.tableFollow = tablefolow;
            if (!isLocalPlayer)
                tableManagment.enabled = false;

            yield return new WaitForSeconds(3f);
        }

        private void Update()
        {
            /*if (teleportRay)
            {
                teleportRay.gameObject.SetActive(EnableTeleportRay && CheckIfActivated(teleportRay));
            }*/

            InputHelpers.IsPressed(rightHand.inputDevice, fingerButton, out isPressRightTrigger);
            InputHelpers.IsPressed(lefttHand.inputDevice, fingerButton, out ispressLeftTrigger);
            InputHelpers.IsPressed(lefttHand.inputDevice, gripBtton, out ispressLeftGrip);
            InputHelpers.IsPressed(rightHand.inputDevice, gripBtton, out isPressRightGrip);
            InputHelpers.IsPressed(rightHand.inputDevice, timerButton, out isTimerPress);
            InputHelpers.IsPressed(rightHand.inputDevice, spawnButton, out isPressTable);
            InputHelpers.IsPressed(rightHand.inputDevice, spawnButton2, out isPressTable2);

            if (isTimerPress)
            {
                 if (restart)
                {
                    NetworkManagerMain.instance.Restart();
                }
            }
            if (canMove)
            {
                movementMidle = Vector3.Lerp(lefttHand.transform.localPosition, rightHand.transform.localPosition, 0.5f);
                deltaLeft = Vector3.SignedAngle(new Vector3(movementMidle.x, 0, movementMidle.z), new Vector3(initMidle.x, 0, initMidle.z), Vector3.up);
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
            else if (isPressTable ||isPressTable2)
            {
                if (!spawnOnce)
                {
                    /*objectToSpawn = new GameObject[3];
                     objectToSpawn  =  tableManagment.ActiavetTetro();*/
                    CmdSpawnThisObject();
                    spawnOnce = true;
                    StartCoroutine(WaiToSpawn());
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
                       // handLeftAnimator.SetBool("FingerOn", isFingerLeft);
                    }
                }
            }

            /*if (!isPressTable && spawnOnce)
            {
                spawnOnce = false;
            }*/

        }
        IEnumerator WaiToSpawn()
        {
            yield return new WaitForSeconds(5f);
            spawnOnce = false;
        }
        [Command]
        private void CmdSpawnThisObject()
        {
            for (int i = 0; i < objectToSpawn.Length; i++)
            {
                _spawn = Instantiate(objectToSpawn[i], tableManagment.transform.position + objectToSpawn[i].transform.position, Quaternion.identity);
                _spawn.GetComponent<SCR_XROffsetGrabbable>().follow = tableManagment._transform;
               
                NetworkServer.Spawn(_spawn, connectionToClient);
                RpcSyncUnits(_spawn);
            }
        }

        [ClientRpc]
        void RpcSyncUnits(GameObject x)
        {
            _spawn = x;
            _spawn.GetComponent<SCR_XROffsetGrabbable>().follow = tableManagment._transform;
        }
        [Command]
        private void CmdSpawntetro()
        {

            foreach (var item in objectToSpawn)
            {
                NetworkServer.Spawn(_spawn, gameObject);
            }
        }
        /* [Command]
         public void CmdSpawnObject(GameObject gameObjectToSpawn)
         {
             NetworkServer.Spawn(gameObjectToSpawn);
         }*/

        public void SpawnObecjt(GameObject authority, Transform trnas)
        {
            if (isLocalPlayer)
            {
                CmdSpawnObject(authority, trnas);
            }
        }
        private int indexStart = 0;
        [Command]
        private void CmdSpawnObject(GameObject authority, Transform trnas)
        {
            for (int i = 0; i < startSpawn.Length; i++)
            {
                if (indexStart == 0)
                    _spawn = Instantiate(startSpawn[i], startSpawn[i].transform.position + tableTransform.position, Quaternion.identity);
                else
                {
                    GameObject _table = GameObject.FindGameObjectWithTag("Table2");
                    _spawn = Instantiate(startSpawn[i], startSpawn[i].transform.position + _table.transform.position, Quaternion.identity);

                }
                NetworkServer.Spawn(_spawn, authority);
            }
            indexStart++;
        }


        public void SpawnPiece(GameObject peiceToSpawn, GameObject authority, Transform parent)
        {
            if (isLocalPlayer)
            {
                prefabToSpawn = peiceToSpawn;
                CmdSpawnPieces(authority, parent);
            }
        }
        [Command]
        private void CmdSpawnPieces(GameObject authority, Transform parent)
        {
            _spawn = Instantiate(prefabToSpawn, tableManagment.transform.position + prefabToSpawn.transform.position, Quaternion.identity);
            _spawn.transform.SetParent(parent);
            _spawn.GetComponent<SCR_XROffsetGrabbable>().follow = tableManagment.transform;
            NetworkServer.Spawn(_spawn, authority);
        }

        [Command(requiresAuthority = false)]
        public void CmdEndTurn()
        {
            RcpWin();
        }
        [Command(requiresAuthority = false)]
        public void CmdEndTurn1()
        {
            RcpWin1();
        }

        [ClientRpc]
        void RcpWin()
        {
            GameObject _text = GameObject.FindGameObjectWithTag("WinText");
            TextMeshProUGUI wintext = _text.GetComponent<TextMeshProUGUI>();
            wintext.text = "Player 2 won";
        }
        [ClientRpc]
        void RcpWin1()
        {
            GameObject _text = GameObject.FindGameObjectWithTag("WinText");
            TextMeshProUGUI wintext = _text.GetComponent<TextMeshProUGUI>();
            //player win
            wintext.text = "Player 1 won";
        }
        /*public bool CheckIfActivated(XRController controller)
        {
            InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
            return isActivated;
        } */
    }

}
