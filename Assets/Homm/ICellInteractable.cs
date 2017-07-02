using System.Collections.Generic;
using UnityEngine;

namespace Assets.Homm
{
    public interface ICellInteractable
    {
        IEnumerable<YieldInstruction> Interact();
    }
}