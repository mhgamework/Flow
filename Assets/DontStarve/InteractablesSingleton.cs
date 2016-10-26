using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class InteractablesSingleton : Singleton<InteractablesSingleton>
    {
        private List<IInteractable> interactables = new List<IInteractable>();
        public void RegisterInteractable(IInteractable interactable)
        {
            interactables.Add(interactable);
        }
        public IEnumerable<IInteractable> GetAllInteractables()
        {
            return interactables;
        }

        internal void UnRegisterInteractable(IInteractable interactable)
        {
            interactables.Remove(interactable);
        }
    }
}
