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

	public void Update(){
		//Temporary trail movement
		GameObject[] trails = GameObject.FindGameObjectsWithTag("TrailContainer");
		Vector3 speed = new Vector3(0,0,1) * GameTravelSpeed * Time.deltaTime;
		foreach(GameObject go in trails){
			for(int i = 0; i < go.transform.childCount; i++){
				go.transform.GetChild(i).position -= speed;
			}
			go.transform.localPosition += speed;
		}
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
