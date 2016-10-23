using UnityEngine;
using System.Collections;

public class HudScript : MonoBehaviour {

    public UnityEngine.UI.Slider FoodSlider;
    public UnityEngine.UI.Slider HealthSlider;
    public PlayerScript playerScript;


    // Use this for initialization
    void Start () {
	
        

	}
	
	// Update is called once per frame
	void Update () {
        FoodSlider.value = playerScript.Food;
        HealthSlider.value = playerScript.Health;
	}
}
