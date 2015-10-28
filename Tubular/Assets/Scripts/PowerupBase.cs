using UnityEngine;
using UnityEngine.Networking;

public class PowerupBase : NetworkBehaviour {

	[SyncVar(hook="OnActivated")]
	protected bool isActive;

	public bool Active { get { return isActive; } }

	/// <summary>
	/// Called on Server via SendMessage when this powerup is collected.
	/// </summary>
	public virtual void OnCollected() { }

	/// <summary>
	/// Called upon lethal collision with obstacle.
	/// Return false if collision should be ignored.
	/// </summary>
	public virtual bool OnObstacleCollision(GameObject obstacle) { return true; }

	/// <summary>
	/// Called when jump is attempted.
	//  Returns jump strength multiplier
	/// </summary>
	/// <param name="grounded">If set to <c>true</c> grounded.</param>
	public virtual float OnJumpPressed(bool grounded) { return 1.0f; }

	public virtual void OnActivated(bool newValue){
		this.isActive = newValue;
	}
}
