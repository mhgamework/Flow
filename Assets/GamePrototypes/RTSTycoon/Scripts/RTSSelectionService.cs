using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;

public class RTSSelectionService : SingletonBehaviour<RTSSelectionService>
{
    private List<RTSSelectable> selected = new List<RTSSelectable>();

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
        
	}


    public IEnumerable<RTSSelectable> Selection
    {
        get
        {
            return selected;
        }
    }

    public bool HasSelection()
    {
        return selected.Any();
    }

    public bool IsSelected(RTSSelectable s)
    {
        return selected.Contains(s);
    }

    public void Select(RTSSelectable s)
    {
        if (selected.Contains(s)) return;
        selected.Add(s);

        s.OnSelected();

        if (OnSelectionChanged != null) OnSelectionChanged();

    }

    public void Deselect(RTSSelectable s)
    {
        if (!IsSelected(s)) return;
        selected.Remove(s);
        s.OnDeselected();

        if (OnSelectionChanged != null) OnSelectionChanged();
        
    }

    public void DeselectAll()
    {
        foreach (var s in selected.ToArray()) Deselect(s);
    }


    public event Action OnSelectionChanged;

}
