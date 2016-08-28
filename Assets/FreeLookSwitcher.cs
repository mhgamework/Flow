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
        setPlayerFromSpecaterOrientation();
        UpdateCameraState();
    }

    private void setPlayerFromSpecaterOrientation()
    {

        PlayerCharacterController.transform.position = SpectaterCamera.transform.position;
        PlayerCharacterController.transform.rotation = SpectaterCamera.transform.rotation;

    }

    private void setSpectatorFromPlayerOrientation()
    {

        SpectaterCamera.transform.position = PlayerCharacterController.transform.position;
        SpectaterCamera.transform.rotation = PlayerCharacterController.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(ToggleKey))
        {
            FreeLookEnabled = !FreeLookEnabled;
            UpdateCameraState();
            if (!FreeLookEnabled)
                setPlayerFromSpecaterOrientation();
            else
                setSpectatorFromPlayerOrientation();
        }
    }

    public void UpdateCameraState()
    {
        SpectaterCamera.SetActive(FreeLookEnabled);
        PlayerCharacterController.SetActive(!FreeLookEnabled);
    }

}
