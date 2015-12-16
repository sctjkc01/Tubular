using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    /// <summary>
    /// Value read by everything that travels.  Value is in Unity Units per Second.
    /// </summary>
    [Tooltip("In Unity Units per Second.  Read by everything that travels.")]
    public float GameTravelSpeed = 5.0f;

	private ObjectSpawner objSpawn;

    [SerializeField]
    private float SpeedIncreasePerSecond = 1.0f;

    /// <summary>
    /// Are we playing a game right now?
    /// </summary>
    [SyncVar (hook="onSync") ]
    private bool _live = false;

    public static GameManager inst;

	public GameObject cam;

    void Awake() {
        inst = this;
        //DontDestroyOnLoad(this.gameObject);
        //else Destroy(this.gameObject);
    }

	void Start(){
		cam = GameObject.Find("Main Camera");
		objSpawn = GameObject.Find ("Obj Spawn").GetComponent<ObjectSpawner>();
	}

	public void Update(){
#if UNITY_EDITOR
        if(inst == null) inst = this;
#endif

		//Temporary trail movement
		GameObject[] trails = GameObject.FindGameObjectsWithTag("TrailContainer");
		Vector3 speed = new Vector3(0,0,1) * GameTravelSpeed * Time.deltaTime;
		foreach(GameObject go in trails){
			for(int i = 0; i < go.transform.childCount; i++){
				go.transform.GetChild(i).position -= speed;
			}
			go.transform.localPosition += speed;
		}

		GameTravelSpeed += SpeedIncreasePerSecond * Time.deltaTime;
		objSpawn.SpawnChance += 1/20.0f * Time.deltaTime;

		foreach(PlayerController pc in GameObject.FindObjectsOfType<PlayerController>()){
			if(pc.alive) pc.CmdAddPoint(2*Time.deltaTime);
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
