using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.PowerLines.Scripts
{
    public class WireSystemScript : Singleton<WireSystemScript>
    {
        public Transform LinePrefab;
        public float Height = 0.5f;
        public float MaxLength = 5;
        public float FlowSpeed = 1;

        public float PressureDecay = 0.95f;

        private HashSet<Connection> connections = new HashSet<Connection>();

        public void ConnectPoles(WirePoleScript a, WirePoleScript b)
        {
            var conn = new Connection(a, b);
            if (conn.Length() > MaxLength) return;
            if (!connections.Contains(conn))
                addConnection(conn);
        }

        public void OnRemovePole(WirePoleScript f)
        {
            foreach (var c in connections.Where(k => k.Has(f)).ToArray())
            {
                removeConnection(c);
            }
        }

        private void addConnection(Connection conn)
        {
            connections.Add(conn);
            // Draw
            var line = Instantiate(LinePrefab, transform);
            line.position = conn.a.transform.position;
            line.LookAt(conn.b.transform.position);
            var len = (conn.a.transform.position - conn.b.transform.position).magnitude;
            line.position += Vector3.up * Height;
            line.localScale = new Vector3(1, 1, len);
            conn.Renderer = line;
        }

        private void removeConnection(Connection conn)
        {
            connections.Remove(conn);
            Destroy(conn.Renderer.gameObject);
        }

        public void Update()
        {
            var points = connections.SelectMany(c => new[] { c.a, c.b }).Select(k => k.GetComponentInChildren<EnergyPointScript>()).Distinct().ToArray();
            var newPressure = new Dictionary<EnergyPointScript, float>();

            foreach (var point in points)
            {
                var neighbours = connections.Where(k => k.Has(point.GetComponentInChildren<WirePoleScript>()))
                    .Select(k => k.Other(point.GetComponentInChildren<WirePoleScript>())).ToArray();

                // average excess pressure
                var avg = neighbours.Average(k => Mathf.Max(0, k.GetComponentInChildren<EnergyPointScript>().Pressure - point.Energy));

                newPressure[point] = point.Energy+avg* PressureDecay;

            }
            foreach (var f in newPressure.Keys)
            {
                f.Pressure = newPressure[f];
            }

            foreach (var c in connections)
            {
                var a = c.a.GetComponent<EnergyPointScript>();
                var b = c.b.GetComponent<EnergyPointScript>();

                if (a.Pressure < b.Pressure)
                {
                    var t = b;
                    b = a;
                    a = t;
                }

                var diff = a.Pressure - b.Pressure;

                var flow = Mathf.Min(FlowSpeed * Time.deltaTime);
                flow = Mathf.Min(flow, b.MaxEnergy - b.Energy);
                a.Energy -= flow;
                b.Energy += flow;


            }
        }



        private class Connection
        {
            public WirePoleScript a;
            public WirePoleScript b;
            public Transform Renderer;

            public Connection(WirePoleScript a, WirePoleScript b)
            {
                this.a = a;
                this.b = b;
                Renderer = null;
            }

            public bool Has(WirePoleScript f)
            {
                return a == f || b == f;
            }

            public WirePoleScript Other(WirePoleScript f)
            {
                if (a == f) return b;
                if (b == f) return a;
                throw new System.Exception();
            }

            public float Length()
            {
                return (a.transform.position - b.transform.position).magnitude;
            }

            public bool Equals(Connection other)
            {
                return Equals(a, other.a) && Equals(b, other.b);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Connection && Equals((Connection)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((a != null ? a.GetHashCode() : 0) * 397) ^ (b != null ? b.GetHashCode() : 0);
                }
            }
        }
    }
}