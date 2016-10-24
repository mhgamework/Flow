using UnityEngine;
using System.Collections;

public class FogOfWarScript : MonoBehaviour
{
    [SerializeField]
    private float GridSize = 5;
    [SerializeField]
    private float WorldSize = 100;

    [SerializeField]
    private Camera PlayerCamera;

    [SerializeField]
    private Camera FogCamera;


    public MeshRenderer PlaneMesh;
    private Texture2D mainTexture;
    private int texSize;

    // Use this for initialization
    void Start()
    {
        FogCamera.transform.parent = PlayerCamera.transform;
        FogCamera.transform.localRotation = Quaternion.identity;
        FogCamera.transform.localPosition = new Vector3();
        FogCamera.transform.localScale = Vector3.one;
        texSize = Mathf.CeilToInt(WorldSize / GridSize);
        // Adjust worldsize to match texsize
        WorldSize = GridSize * texSize;

        transform.localScale = new Vector3(WorldSize, 1, WorldSize);

        PlaneMesh.enabled = true;
        mainTexture = new Texture2D(texSize, texSize);
        var colors = mainTexture.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0, 0, 0, 1);
            //if (i%4 == 0)
            //colors[i] = new Color(1, 0, 0, 1);
        }
        // For calibrating
        colors[0] = new Color(0, 1, 0, 1);
        colors[texSize - 1] = new Color(1, 0, 0, 1);
        colors[(texSize - 1) * texSize] = new Color(0, 0, 1, 1);

        mainTexture.SetPixels(colors);
        mainTexture.Apply();
        PlaneMesh.material.mainTexture = mainTexture;
        //PlaneMesh.material.SetTexture("Albedo", tex);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector2 posToTexture(Vector3 pos)
    {
        // The 0,0 texcoord is at worldspace (-WorldSize/2+GridSize/2)
        pos -= new Vector3(1, 0, 1) * (-WorldSize + GridSize) / 2;
        pos /= GridSize;
        return new Vector2(pos.x, pos.z);
    }

    public void MakeVisible(Vector3 pos, float radius)
    {
        radius /= GridSize; // texel space
        var texPos = posToTexture(pos);

        var tex = mainTexture;
        var colors = tex.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            int x = i % tex.width;
            int y = i / tex.width;
            var distSq = (texPos - new Vector2(x, y)).sqrMagnitude;
            if (distSq < radius * radius)
            {
                var alpha = distSq/radius/radius;
                alpha = Mathf.Pow(alpha, 5);
                colors[i] = new Color(0, 0, 0, Mathf.Min(alpha, colors[i].a));

            }

        }

        tex.SetPixels(colors);
        tex.Apply();
    }
}
