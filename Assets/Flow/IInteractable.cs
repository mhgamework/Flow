using UnityEngine;

namespace Assets.Flow
{
    public interface IInteractable
    {
        void OnUnfocus(Player p);
        void OnFocus(Player p);
        void TryInteract(Player p, Vector3 point);

    }
}