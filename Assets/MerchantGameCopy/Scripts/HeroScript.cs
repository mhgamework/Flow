using UnityEngine;
using System.Collections;
using System.Linq;
using DirectX11;

/// <summary>
/// Represents a moving hero like homm that can have an army and additionally items
/// </summary>
public class HeroScript : MonoBehaviour
{
    public float NormalMoveSpeed = 3;
    public float RoadFactor = 2;
    public float ActiveMoveSpeed { get; private set; }

    public int NumberOfUnits;

    [SerializeField]
    private Transform HeroTransform;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var pos = Point3.Floor(HeroTransform.position / 5);
        pos.Y = 0;
        var road = FindObjectsOfType<RoadScript>().FirstOrDefault(r => r.CalculatePos() == pos);
        ActiveMoveSpeed = NormalMoveSpeed;

        if (road != null) { ActiveMoveSpeed += RoadFactor; ; }
    }
}
