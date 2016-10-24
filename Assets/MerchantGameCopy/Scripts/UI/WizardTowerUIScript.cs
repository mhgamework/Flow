using UnityEngine;
using System.Collections;

public class WizardTowerUIScript : MonoBehaviour
{

    [SerializeField]
    private GameObject Panel;
	// Use this for initialization
	void Start ()
	{
	    Hide();

	}


    public static WizardTowerUIScript Instance()
    {
        var ret = FindObjectOfType<WizardTowerUIScript>();
        if (!ret) Debug.LogError("No instance of WizardTowerUIScript found in scene!");
        return ret;
    }


    public void Hide()
    {
        Panel.SetActive(false);
    }

    public void Show()
    {
        Panel.SetActive(true);

    }

    // Update is called once per frame
    void Update () {
	
	}
}
