using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PowerupMultijump : PowerupBase
{

    [SerializeField]
    private GameObject doubleJumpVisual;

    [SerializeField]
    private int maxJumps;

    [SerializeField]
    private int jumpsPerPowerup;

    private int currentJumps;

    // Use this for initialization
    void Start()
    {
        if(doubleJumpVisual) doubleJumpVisual.SetActive(false);
        this.isActive = true; //Always active, always checking
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnCollected()
    {
        this.isActive = true;
        this.currentJumps = (int)Mathf.Min(currentJumps + jumpsPerPowerup, maxJumps);
    }
    public override float OnJumpPressed(bool grounded) 
    {
        if (!grounded)
        {
            if (currentJumps > 0)
            {
                currentJumps--;
                return 1.0f;
            }
            else
                return 0.0f;
        }
        else
        {
            return 1.0f;
        }
    }
}
