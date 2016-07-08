using UnityEngine;
using System.Collections;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;

public class FreeLookSwitcher : MonoBehaviour
{

    public GameObject SpectaterCamera;
    public GameObject PlayerCharacterController;

    public bool FreeLookEnabled = false;
    public KeyCode ToggleKey = KeyCode.I;

    // Use this for initialization
    void Start()
    {
        UpdateCameraState();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(ToggleKey))
        {
            FreeLookEnabled = !FreeLookEnabled;
            UpdateCameraState();
            if (!FreeLookEnabled)
            {
                PlayerCharacterController.transform.position = SpectaterCamera.transform.position;
                PlayerCharacterController.transform.rotation = SpectaterCamera.transform.rotation;
            }
        }
    }

    public void UpdateCameraState()
    {
        SpectaterCamera.SetActive(FreeLookEnabled);
        PlayerCharacterController.SetActive(!FreeLookEnabled);
    }

}
