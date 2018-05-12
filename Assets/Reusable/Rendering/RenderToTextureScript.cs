using UnityEngine;
using System.Collections;
using System.Linq;
using Assets;
using UnityEngine.UI;

/// <summary>
/// Utility script to render objects to textures.
/// Requires the corresponding preconfigured prefab with camera to work.
/// </summary>
public class RenderToTextureScript : MonoBehaviour
{
    public string RenderingLayer = "ObjectSnapshot";
    public GameObject prefab;
    public RawImage img;
    public float ZoomFactor = 1.2f;

    public static RenderToTextureScript Instance()
    {
        var ret = FindObjectOfType<RenderToTextureScript>();
        if (!ret) Debug.LogError("No instance of RenderToTextureScript found in scene!");
        return ret;
    }


    // Use this for initialization
    void Start()
    {
        GetComponent<Camera>().enabled = false;
    }

    Texture2D RTImage(Camera cam)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;
        return image;
    }

    void SetLayerRecursively(GameObject o, int layer)
    {
        foreach (Transform t in o.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = layer;
    }

    // Update is called once per frame
    void Update()
    {
        /*var texture = PrefabToTexture(prefab);

        img.texture = texture;*/
    }

    public Texture2D PrefabToTexture(GameObject prefab)
    {
        var gameObject = InstantiateInFrontOfCamera(prefab);
        var layerId = LayerMask.NameToLayer(RenderingLayer);
        if (layerId == -1)
            throw new System.Exception("Layer does not exist: " + RenderingLayer);
        SetLayerRecursively(gameObject, layerId);


        transform.position = new Vector3(-10, 10, -10);
        transform.LookAt(new Vector3(0, 0, 0));

        var camera = GetComponent<Camera>();
        camera.targetTexture = RenderTexture.GetTemporary(256, 256, 16);
        var texture = RTImage(camera);
        camera.targetTexture = null;
        RenderTexture.ReleaseTemporary(camera.targetTexture);
        GameObject.DestroyImmediate(gameObject);
        return texture;
    }


    private GameObject instance;

    private GameObject InstantiateInFrontOfCamera(GameObject prefab)
    {
        GameObject gameObject;
        //if (instance == null)
        {
            //GameObject.DestroyImmediate(instance);
            gameObject = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            instance = gameObject;
        }
        gameObject = instance;

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;

        var bounds = gameObject.GetTotalMeshRendererBounds();


        var scale = Vector3.one / (Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z));

        gameObject.transform.localScale = scale * ZoomFactor;



        bounds = gameObject.GetTotalMeshRendererBounds();

        gameObject.transform.position = -bounds.center;


        return gameObject;
    }
}
