using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;
using UnityEngine.InputSystem;
public class SCR_LocomotionController : NetworkBehaviour
{
    public XRController teleportRay;
    public XRController rightHand;
    public XRController lefttHand;
    public InputHelpers.Button teleportActivationButton;
    public InputHelpers.Button fingerButton;
    public InputHelpers.Button gripBtton;
    public InputHelpers.Button spawnButton;
    public SphereCollider leftHandColider;
    public SphereCollider rightHandColider;
    public float activationThreshold = 0.1f;
    [Header("Movement")]
    public float anglePower=5;
    /// <summary>
    /// If negative will invers the vertical axe
    /// </summary>
    public float ySpeed;
    public float zSpeed;

    public bool EnableTeleportRay { get; set; } = true;

    public Animator handLeftAnimator;
    public Animator handRightAnimator;
    public GameObject table;
    public Transform tablefolow;

    private bool isFingerLeft, isFingerRight;
    private bool cantPressLeft, cantpressRight;
    private bool ispressLeftTrigger, isPressRightTrigger, ispressLeftGrip, isPressRightGrip;
    private bool canMove;
    private Vector3 initMidle;
    private Vector3 movementMidle;
    public float deltaLeft;
    private float deltaRight;
    private Transform tableTransform;
    private bool isPressTable;
    private bool spawnOnce;
    private TableManagment tableManagment;
    private GameObject[] objectToSpawn;

    public GameObject[] startSpawn;
    public GameObject _spawn;
    private void Start()
    {
        tableTransform = GameObject.FindGameObjectWithTag("Table").transform;
       
       GameObject _table = Instantiate(table, transform);
        tableManagment = _table.GetComponent<TableManagment>();

        tableManagment.tableFollow = tablefolow;
        if (!isLocalPlayer)
            tableManagment.enabled = false;
        else if(isServer)
        {
           /* for (int i = 0; i < startSpawn.Length; i++)
            {
                 _spawn = Instantiate(startSpawn[i], startSpawn[i].transform.position, Quaternion.identity);
            }*/
        }
        
    }
    private void Update()
    {
        /*if (teleportRay)
        {
            teleportRay.gameObject.SetActive(EnableTeleportRay && CheckIfActivated(teleportRay));
        }*/
       
            InputHelpers.IsPressed(rightHand.inputDevice, fingerButton, out isPressRightTrigger);
            InputHelpers.IsPressed(lefttHand.inputDevice, fingerButton, out  ispressLeftTrigger);
            InputHelpers.IsPressed(lefttHand.inputDevice, gripBtton, out  ispressLeftGrip);
            InputHelpers.IsPressed(rightHand.inputDevice, gripBtton, out  isPressRightGrip);
            InputHelpers.IsPressed(rightHand.inputDevice, spawnButton, out isPressTable);

        if (canMove)
        {
            movementMidle =  Vector3.Lerp(lefttHand.transform.localPosition, rightHand.transform.localPosition, 0.5f);
            deltaLeft = Vector3.SignedAngle(movementMidle,initMidle, Vector3.up);
            transform.RotateAround(tableTransform.position, Vector3.up, deltaLeft*anglePower);
            transform.position += Vector3.up * (movementMidle.y - initMidle.y)*ySpeed;

           // transform.position += Vector3.forward * (movementMidle.z - initMidle.z)*zSpeed;
            initMidle = movementMidle;
        }

        if (ispressLeftGrip && ispressLeftTrigger && isPressRightGrip && isPressRightTrigger )
        {
            if (!canMove)
            {
                canMove = true;
                initMidle = Vector3.Lerp(lefttHand.transform.localPosition,rightHand.transform.localPosition, 0.5f);
            }


        }
        else if (isPressTable)
        {
            if (!spawnOnce)
            {
                objectToSpawn = new GameObject[3];
                objectToSpawn  =  tableManagment.ActiavetTetro();
                CmdSpawntetro();
                spawnOnce = true;
            }
        }
        else if(canMove)
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

            if (ispressLeftTrigger )
            {
                if (!isFingerLeft)
                {
                    if (handLeftAnimator == null)
                        handLeftAnimator = lefttHand.GetComponent<XRController>().modelParent.GetComponentInChildren<SCR_HandPresence>().spawnedHandModel.GetComponent<Animator>();
                    isFingerLeft = true;
                    leftHandColider.enabled = !isFingerLeft;
                    handLeftAnimator.SetBool("FingerOn", isFingerLeft);
                }
            }
            else
            {
                if (isFingerLeft)
                {
                    isFingerLeft = false;
                    leftHandColider.enabled = !isFingerLeft;
                    handLeftAnimator.SetBool("FingerOn", isFingerLeft);
                }
            }
        }
        
        if(!isPressTable && spawnOnce)
        {
            spawnOnce = false;
        }

    }
    [Command]
    private void CmdSpawntetro()
    {
       
        foreach (var item in objectToSpawn)
        {
            NetworkServer.Spawn(item);
        }
    }
   /* [Command]
    public void CmdSpawnObject(GameObject gameObjectToSpawn)
    {
        NetworkServer.Spawn(gameObjectToSpawn);
    }*/

    public  void SpawnObecjt(GameObject authority, Transform trnas)
    {
        if (isLocalPlayer)
        {
        CmdSpawnObject(authority, trnas);
        }
    }
    [Command]
    private void CmdSpawnObject(GameObject authority, Transform trnas )
    {
        for (int i = 0; i < startSpawn.Length; i++)
        {
            _spawn = Instantiate(startSpawn[i], startSpawn[i].transform.position + trnas.position, Quaternion.identity);
            NetworkServer.Spawn(_spawn, authority);
        }
    }
    IEnumerator TresholdRight()
    {
        cantpressRight = true;
        yield return new WaitUntil(() => !isPressRightTrigger);
        cantpressRight = false;

    }
    IEnumerator TresholdLeft()
    {
        cantPressLeft = true;
        yield return new WaitUntil(()=>!ispressLeftTrigger);
        cantPressLeft = false;

    }

    /*public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
    } */
}
