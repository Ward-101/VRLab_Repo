using UnityEngine;

public class SCR_HoverEvent : MonoBehaviour
{
    public Material hoverMat;

    private MeshRenderer meshRenderer;
    private Material baseMat;

    private void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        baseMat = meshRenderer.material;
    }

    public void ChangeMatOnHoverEntering()
    {
        meshRenderer.material = hoverMat;
    }

    public void ChangeMatOnHoverExiting()
    {
        meshRenderer.material = baseMat;
    }
}
