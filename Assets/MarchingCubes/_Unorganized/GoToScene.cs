using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToScene : MonoBehaviour
{
    public string SceneName;
    // Use this for initialization
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(onClick);
    }

    private void onClick()
    {
        SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
