using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeForMainScreen : MonoBehaviour {
    public string SceneName;

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
	        SceneManager.LoadScene(SceneName);
        }
    }
}
