using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using Assets.MineMinecraft.DummyWorldImpl;

public class BlockInteractScript : MonoBehaviour,IPointerClickHandler {

    public void OnPointerClick(PointerEventData eventData)
    {
        //eventData.selectedObject.transform.position
        IWorld world=DummyWorldManager.Instance().World;
        //world.SetBlockAt(transform.position, null);

        
    }

}
