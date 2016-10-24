using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickedPositionIndicatorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        transform.GetChild(0).gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update () {
	
	}

    public void Show(Vector3 pos)
    {
        StopAllCoroutines();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.position = pos;
        StartCoroutine(ShowClickPosition().GetEnumerator());
    }

    public IEnumerable<YieldInstruction> ShowClickPosition()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        transform.GetChild(0).gameObject.SetActive(false);


    }
}
