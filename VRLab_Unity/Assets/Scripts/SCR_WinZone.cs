using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_WinZone : MonoBehaviour
{
    public float countdown;
    private Material baseMat;
    [SerializeField] private Material processMat;
    [SerializeField] private Material winMat;

    private float startTime;
    private bool canCount;

    private List<Collider> colliderList = new List<Collider>();

    private MeshRenderer meshRenderer;
 
    private void OnTriggerStay(Collider other)
    {
        if (!colliderList.Contains(other) && other.tag == "Piece")
        {
            if (other.TryGetComponent(out SCR_PieceState pieceState))
            {
                if (!other.transform.GetComponent<SCR_PieceState>().IsGrab)
                {
                    colliderList.Add(other);
                }
            }
            else
            {
                if (!other.transform.parent.parent.GetComponent<SCR_PieceState>().IsGrab)
                {
                    colliderList.Add(other);
                }
            }

            if (colliderList.Count == 1)
            {
                StartCountdown();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Piece" && colliderList.Contains(other))
        {
            colliderList.Remove(other);

            if (colliderList.Count == 0)
            {
                EndCountdown();
            }
        }
    }

    private void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        baseMat = meshRenderer.material;
    }

    private void Update()
    {
        if (canCount && AudioSettings.dspTime - startTime >= countdown)
        {
            meshRenderer.material = winMat;
        }
    }
    private void StartCountdown()
    {
        startTime = (float)AudioSettings.dspTime;
        canCount = true;
        meshRenderer.material = processMat;
    }

    private void EndCountdown()
    {
        startTime = 0f;
        canCount = false;
        meshRenderer.material = baseMat;
    }

}
