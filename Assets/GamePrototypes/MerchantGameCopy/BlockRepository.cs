using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;

/// <summary>
/// Ideas is that a blockrepository is a sort of namespace of blockdefinitiosn, which are block factories
/// There is advanced basic block definition which is both a block and a factory = prototype based, not implemented yet 
/// </summary>

public class BlockRepository : MonoBehaviour {
    private IBlockTypeDefinition[] definitions;

    // Use this for initialization
	void Start ()
	{
	    definitions = GetComponentsInChildren(typeof (IBlockTypeDefinition)).Cast<IBlockTypeDefinition>().ToArray();
	}

    // Update is called once per frame
	void Update () {
	
	}

    public IEnumerable<IBlockTypeDefinition> GetDefinitions()
    {
        return definitions;
    }

    public IBlockTypeDefinition GetByName(string name)
    {
        return definitions.FirstOrDefault(d => d.TypeName == name);
    }
}
