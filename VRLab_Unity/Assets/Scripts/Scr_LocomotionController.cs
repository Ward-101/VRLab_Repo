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
    public GameObject[] objectToSpawn;
    public List<GameObject> objectSpawned = new List<GameObject>();

    public GameObject[] startSpawn;
    public GameObject _spawn;
    private GameObject prefabToSpawn;
    private int index;
    private void Start()
    {
        tableTransform = GameObject.FindGameObjectWithTag("Table").transform;
       
        GameObject _table = Instantiate(table, tablefolow.position +table.transform.position,Quaternion.identity);
        _table.transform.SetParent(transform);
        tableManagment = _table.GetComponent<TableManagment>();

        tableManagment.tableFollow = tablefolow;
        if (!isLocalPlayer)
            tableManagment.enabled = false;
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
                /*objectToSpawn = new GameObject[3];
                 objectToSpawn  =  tableManagment.ActiavetTetro();*/
                for (int i = 0; i < objectToSpawn.Length; i++)
                {
                    _spawn = Instantiate(objectToSpawn[i], tableManagment.transform.position + objectToSpawn[i].transform.position, Quaternion.identity);
                    _spawn.SetActive(false);

                    NetworkServer.Spawn(_spawn, gameObject);
                }
                /*ActivateObject();
                if (!isLocalPlayer)
                    return;
                if (isServer)
                {
                    RpcActivateObject();
                }
                else
                {
                    CmdActivate();
                }*/
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
            NetworkServer.Spawn(_spawn, gameObject);
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
            _spawn = Instantiate(startSpawn[i],  startSpawn[i].transform.position, Quaternion.identity);
            NetworkServer.Spawn(_spawn, authority);
        }
/*        for (int x = 0; x < 5; x++)
        {
            for (int i = 0; i < objectToSpawn.Length; i++)
            {
                _spawn = Instantiate(objectToSpawn[i], tableManagment.transform.position + objectToSpawn[i].transform.position, Quaternion.identity);
                _spawn.SetActive(false);
               *//* RcpSetOff();
                if (!isServer)
                    RcpAddPlayer(authority);
                authority.GetComponent<SCR_LocomotionController>().objectSpawned.Add(_spawn);*//*

                NetworkServer.Spawn(_spawn, authority);
            }
        }*/
      //  StartCoroutine(debugSpaw());
    }
    [ClientRpc]
    void RcpSetOff()
    {
        _spawn.SetActive(false);
    }

    IEnumerator debugSpaw()
    {
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < objectToSpawn.Length; i++)
        {
            _spawn = Instantiate(objectToSpawn[i], tableManagment.transform.position + objectToSpawn[i].transform.position, Quaternion.identity);
            NetworkServer.Spawn(_spawn, gameObject);
        }
/*        ActivateObject();
        if (!isLocalPlayer)
            yield return null;
        if (isServer)
        {
            RpcActivateObject();
        }
        else
        {
            CmdActivate();
        }*/
    }
    [ClientRpc]
    void RcpAddPlayer(GameObject authority)
    {
        authority.GetComponent<SCR_LocomotionController>().objectSpawned.Add(_spawn);

    }

    [Command]
    private void CmdActivate()
    {
        ActivateObject();
        RpcActivateObject();
    }
    private void ActivateObject()
    {
        for (int i = index; i < objectToSpawn.Length+index; i++)
        {
            objectSpawned[i].SetActive(true);
        }
        index += objectToSpawn.Length -1;

    }
    [ClientRpc]
    void RpcActivateObject()
    {
        if (isLocalPlayer)
            return;
        ActivateObject();
    }

    public void SpawnPiece(GameObject peiceToSpawn, GameObject authority, Transform parent)
    {
        if (isLocalPlayer)
        {
            prefabToSpawn = peiceToSpawn;
            CmdSpawnPieces( authority, parent);
        }
    }
    [Command]
    private void CmdSpawnPieces(  GameObject authority, Transform parent )
    {
            _spawn = Instantiate(prefabToSpawn,tableManagment.transform.position+prefabToSpawn.transform.position, Quaternion.identity);
            _spawn.transform.SetParent(parent);
            _spawn.GetComponent<SCR_XROffsetGrabbable>().follow = tableManagment.transform;
            NetworkServer.Spawn(_spawn, authority);
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
