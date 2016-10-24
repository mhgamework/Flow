using UnityEngine;
using System.Collections;
using Assets;

public class WizardTowerScript : MonoBehaviour,IPlayerInteractable {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Interact(PlayerMovementScript playerMovementScript)
    {
        playerMovementScript.MoveTo(transform.position, () =>
        {
            WizardTowerUIScript.Instance().Show();
        });
    }
}
