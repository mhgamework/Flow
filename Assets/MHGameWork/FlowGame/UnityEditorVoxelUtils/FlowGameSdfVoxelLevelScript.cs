using System.Linq;
using Assets.MHGameWork.FlowEngine.SdfWorldGeneration;
using Assets.MHGameWork.FlowEngine._Cleanup.EditorVoxelWorldGen;
using Assets.MHGameWork.FlowGame.Scenes.BoxLevelGenerator;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.UnityEditorVoxelUtils
{
    /// <summary>
    /// Represents an in-unity editor build voxel level using SDF objects.
    /// Used for the FlowGame initial world building.
    /// Attach to the level root gameobject
    /// Has root box "All"
    /// 3 Layers: "Extractors" "Placeables" "Ores"
    /// </summary>
    public class FlowGameSdfVoxelLevelScript : MonoBehaviour
    {
        public void Start()
        {
         
        }

        public ISdfObjectWorld CreateSdfObjectWorld()
        {
            var sdfWorld = new SimpleSdfObjectWorld();

            sdfWorld.Objects.Add(transform.Find("All").GetComponent<BaseVoxelObjectScript>());
            sdfWorld.Objects.AddRange(transform.Find("Extractors").GetComponentsInChildren<BoxVoxelObjectScript>(true).Cast<IVoxelObject>().ToList());
            sdfWorld.Objects.AddRange(transform.Find("Placeables").GetComponentsInChildren<BoxVoxelObjectScript>(true).Cast<IVoxelObject>().ToList());
            sdfWorld.Objects.AddRange(transform.Find("Ores").GetComponentsInChildren<BoxVoxelObjectScript>(true).Cast<IVoxelObject>().ToList());

            Debug.Log("Found " + sdfWorld.Objects + " VoxelObjects");
            return sdfWorld;
        }

        public void HideEditorMeshes()
        {
            transform.Find("All").gameObject.SetActive(false);
            transform.Find("Extractors").gameObject.SetActive(false);
            transform.Find("Placeables").gameObject.SetActive(false);
            transform.Find("Ores").gameObject.SetActive(false);
        }
    }
}