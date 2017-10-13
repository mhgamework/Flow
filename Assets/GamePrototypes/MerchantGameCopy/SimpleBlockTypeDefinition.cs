using UnityEngine;
using System.Collections;
using Assets;
using Assets.MineMinecraft;
using Assets.MineMinecraft.Blocks;

public class SimpleBlockTypeDefinition : MonoBehaviour, IBlockTypeDefinition
{

    public Transform BlockPrefab;
    public string Name = "SETTHISPLEASE";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public string TypeName { get { return Name; } }

    public IBlock CreateBlockInstance()
    {
        var ret = new SimpleBlock(BlockPrefab);

        return ret;

    }
}
