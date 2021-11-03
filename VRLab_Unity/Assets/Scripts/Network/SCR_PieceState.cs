using UnityEngine;
using Mirror;

namespace Network{
public class SCR_PieceState : NetworkBehaviour
{
    [SyncVar(hook =nameof(Grab))]
    public bool IsGrab  = false;

    [Command]
    public void Grabing()
    {
        IsGrab = !IsGrab;
    }

    public void Grab(bool oldValue, bool newValue)
    {
            IsGrab = newValue;
    }
            
    }

}
