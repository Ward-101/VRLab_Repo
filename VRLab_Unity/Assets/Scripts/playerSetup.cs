using Mirror;
using UnityEngine;

public class playerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentToDisable;
    private void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentToDisable.Length; i++)
            {
                componentToDisable[i].enabled = false;
            }
        }
    }
}
