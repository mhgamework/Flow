using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GamePrototypes.DontStarve;
using UnityEngine;

namespace Assets
{
    public interface IInteractable
    {
        float getDistance(PlayerScript player);
        Vector3 getPosition();
        void Interact(PlayerScript player);
    }
}
