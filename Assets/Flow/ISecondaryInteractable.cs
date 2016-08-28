using UnityEngine;

namespace Assets.Flow
{
    /// <summary>
    /// Interactables which are subpart of the 'main' interactable, but operate in paralle
    /// probably shitty programming
    /// </summary>
    public interface ISecondaryInteractable
    {
        void OnUnfocus(Player p);
        void OnFocus(Player p);
    }
}