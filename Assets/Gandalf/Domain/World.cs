using System.Collections.Generic;

namespace Assets.Gandalf.Domain
{
    public class World
    {
        public Grid Grid { get; private set; }
        public List<Goblin> Goblins = new List<Goblin>();
        public Wizard Wizard { get; set; }

    }
}