using UnityEngine;
using System.Collections;

public class RTSCameraController : MonoBehaviour
{
    public float CameraVelocity = 5;

    // Use this for initialization
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        var trans = GetComponent<Transform>();

        var delta = new Vector3();

        if (Input.GetKey(KeyCode.UpArrow))
            delta += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.DownArrow))
            delta += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.RightArrow))
            delta += new Vector3(1, 0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
            delta += new Vector3(-1, 0, 0);

        delta.Normalize();

        trans.position += delta * CameraVelocity * Time.deltaTime;

    }


}
