using UnityEngine;
using System.Collections;
using System.Linq;
using DirectX11;

[ExecuteInEditMode]
public class NeighbourConnectScript : MonoBehaviour
{

    public string ConnectionType = "Default";

    public int GridSize = 5;

    public Transform RoadPartsParent;

    private Point3 lastPos;
    private static readonly Point3[] dirs = new[] { new Point3(1, 0, 0), new Point3(0, 0, 1), new Point3(-1, 0, 0), new Point3(0, 0, -1) };

    // Use this for initialization
    void Start()
    {
    }

    public void OnDestroy()
    {
        updateModel();
    }

    public void OnDisable()
    {

    }

    public void OnEnable()
    {
        updateModel();

    }




    // Update is called once per frame
    void Update()
    {
        var pos = CalculatePos();
        if (lastPos != pos)
        {
            foreach (var n in dirs.Select(d => getNeighbour(lastPos, d))) if (n != null) n.updateModel(false);
            updateModel();
        }
        lastPos = pos;
    }

    private void updateModel(bool updateNeighbours = true)
    {
        for (int i = 0; i < dirs.Length; i++)
        {
            var dir = dirs[i];
            var neighbour = getNeighbour(CalculatePos(), dir);

            var connects = neighbour != null;

            RoadPartsParent.GetChild(i).gameObject.SetActive(connects);

            if (neighbour != null && updateNeighbours) neighbour.updateModel(false);
        }
    }

    private NeighbourConnectScript getNeighbour(Point3 point3, Point3 dir)
    {
        
        return FindObjectsOfType<NeighbourConnectScript>().FirstOrDefault(r => r.ConnectionType == ConnectionType &&  r.CalculatePos() == point3 + dir);
    }

    public Point3 CalculatePos()
    {
        return Point3.Floor(transform.position / GridSize);
    }
}
