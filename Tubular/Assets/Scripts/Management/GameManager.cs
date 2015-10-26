using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    /// <summary>
    /// Value read by everything that travels.  Value is in Unity Units per Second.
    /// </summary>
    [Tooltip("In Unity Units per Second.  Read by everything that travels.")]
    public float GameTravelSpeed = 5.0f;

    /// <summary>
    /// Are we playing a game right now?
    /// </summary>
    [SyncVar (hook="onSync") ]
    private bool _live = false;

    public static GameManager inst;

    void Awake() {
        //if (inst == null) 
            inst = this;
        //else Destroy(this.gameObject);
    }

    public void SetLive(bool val) { _live = val; }
    public bool IsLive() { return _live; }

    public bool live { 
        get { return IsLive(); }
        set { 
            SetLive(value);  //Unity's syncVars are weird
            //_live = value;
        }
    }

    [Client]
    public void onSync(bool syncLive){
        _live = syncLive;
    }
}
