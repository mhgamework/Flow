using System.Collections.Generic;

namespace Assets.SimpleGame.Scripts.Enemies
{
    /// <summary>
    /// Helper to make the SleepUntilVoxelsLoadedScript work
    /// </summary>
    public class SleepUntilVoxelsLoadedSingleton : Singleton<SleepUntilVoxelsLoadedSingleton>
    {
        private HashSet<SleepUntilVoxelsLoadedScript> scripts = new HashSet<SleepUntilVoxelsLoadedScript>();
        private List<SleepUntilVoxelsLoadedScript> remover = new List<SleepUntilVoxelsLoadedScript>();
        public void Update()
        {
            foreach (var s in scripts)
            {
                if (s.TryWakeup()) remover.Add(s);
            }
            foreach(var r in remover) scripts.Remove(r);
       
        }

        public void Register(SleepUntilVoxelsLoadedScript sleepUntilVoxelsLoadedScript)
        {
            scripts.Add(sleepUntilVoxelsLoadedScript);
        }
    }
}