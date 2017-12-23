using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.PowerLines.Scripts;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour
{

    public Transform PowerLinePrefab;
    public Transform TankPrefab;
    public Transform MinerPrefab;
    public LineRenderer LinePrefab;



    public Transform DestroyMarker;

    public LayerMask RaycastMask;
    public int GhostLayer;

    private Mode mode;


    private Dictionary<KeyCode, Mode> modes = new Dictionary<KeyCode, Mode>();

    private class Mode
    {
        public bool isPowerLinesMode;
        public bool isBuildMode;
        public bool isDeleteMode;
        public Transform Prefab;
        public PlayerInputScript playerInputScript;




        private Transform buildGhost;
        private WirePoleScript previousPole;

        public void Start()
        {
            if (isBuildMode)
            {
                buildGhost = Instantiate(Prefab, playerInputScript.transform);
                makeGhost(buildGhost);
            }
            if (isDeleteMode)
            {
                Prefab.gameObject.SetActive(true);
                makeGhost(Prefab);

            }
        }

        private void makeGhost(Transform ghost)
        {
            foreach (var c in ghost.GetComponentsInChildren<Collider>()) c.gameObject.layer = playerInputScript.GhostLayer;
        }

        public void Stop()
        {
            if (buildGhost != null)
                Destroy(buildGhost.gameObject);

            if (isDeleteMode)
                Prefab.gameObject.SetActive(false);

            previousPole = null;

        }

        public void Update()
        {
            var target = getTarget();

            WirePoleScript targetPole = null;

            if (isPowerLinesMode && target.HasValue)
            {
                targetPole = target.Value.collider.GetComponentInParent<WirePoleScript>();
                if (targetPole != null)
                {
                    var change = target.Value;
                    change.point = targetPole.transform.position;
                    target = change;
                }
            }

            if (isBuildMode)
                updateGhostPosition(target, buildGhost);
            else if (isDeleteMode)
                updateGhostPosition(target, Prefab);


            if (target.HasValue && Input.GetMouseButtonDown(0))
            {
                if (isPowerLinesMode)
                {
                    if (targetPole == null)
                    {
                        targetPole = buildNew().GetComponentInParent<WirePoleScript>();
                    }
                    var newPole = targetPole;
                    if (previousPole != null)
                        WireSystemScript.Instance.ConnectPoles(newPole, previousPole);

                    previousPole = targetPole;
                }
                else if (isBuildMode)
                {
                    buildNew();

                }
                else if (isDeleteMode)
                {
                    var removeable = target.Value.collider.GetComponentInParent<IPlayerRemoveable>();
                    if (removeable != null)
                        removeable.Remove();
                }
            }
        }

        private Transform buildNew()
        {
            var newObj = Instantiate(Prefab, playerInputScript.transform);

            newObj.position = buildGhost.position;
            return newObj;
        }

        private void updateGhostPosition(RaycastHit? target, Transform ghost)
        {
            ghost.gameObject.SetActive(target.HasValue);
            if (!target.HasValue) return;
            ghost.transform.position = target.Value.point;
        }


        private RaycastHit? getTarget()
        {
            RaycastHit raycastHit;

            var res = Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out raycastHit, 100, playerInputScript.RaycastMask);
            if (!res) return null;

            return raycastHit;
        }
    }


    // Use this for initialization
    void Start()
    {
        var noneMode = new Mode { playerInputScript = this };
        mode = noneMode;

        modes.Add(KeyCode.Alpha1, noneMode);
        modes.Add(KeyCode.Alpha2, new Mode { isBuildMode = true, isPowerLinesMode = true, Prefab = PowerLinePrefab, playerInputScript = this });
        modes.Add(KeyCode.Alpha3, new Mode { isBuildMode = true, Prefab = TankPrefab, playerInputScript = this });
        modes.Add(KeyCode.Alpha4, new Mode { isDeleteMode = true, Prefab = DestroyMarker, playerInputScript = this });
        modes.Add(KeyCode.Alpha5, new Mode { isBuildMode = true, Prefab = MinerPrefab, playerInputScript = this });



    }

    // Update is called once per frame
    void Update()
    {
        foreach (var f in modes.Keys)
        {
            if (Input.GetKeyDown(f)) changeMode(modes[f]);
        }

        mode.Update();

    }

    void changeMode(Mode m)
    {
        if (mode == m) return;
        mode.Stop();

        mode = m;

        mode.Start();
    }




}
