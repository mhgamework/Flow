using UnityEngine;
using System.Collections;
using Assets;

public class CrystalScript : MonoBehaviour
{

    public float MagicContents;
    public float MaxContents = 10;
    public string MagicType = "empty";

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var factor = MagicContents / MaxContents;

        GetComponentInChildren<MeshRenderer>().material.color = Color.Lerp(MagicService.Get.GetColor("empty"),
            MagicService.Get.GetColor(MagicType), factor);

    }

    public void OnTriggerStay(Collider other)
    {
        var magicArea = other.GetComponentInParent<MagicAreaScript>();
        if (!magicArea) return;


        if (magicArea.MagicType == MagicType || MagicType == "empty")
        {
            MagicContents = Mathf.Min(MagicContents + Time.deltaTime, MaxContents);
            MagicType = magicArea.MagicType;
        }
        else
        {
            MagicContents = Mathf.Max(MagicContents - Time.deltaTime, 0);
            if (MagicContents == 0) MagicType = "empty";
        }



    }
}
