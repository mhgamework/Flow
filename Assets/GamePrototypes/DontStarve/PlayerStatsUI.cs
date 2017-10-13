using UnityEngine;
using System.Collections;
using Assets.Flow;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{

    public Slider WaterSlider;
    public Slider FireSlider;
    public Text Text;

    public Player player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{

	    WaterSlider.value = player.WaterMagic/player.MaxMagic;
        FireSlider.value = player.FireMagic/player.MaxMagic;

	    Text.text = "";
	    Text.text += player.Clay + "/" + player.MaxClay;
	}
}
