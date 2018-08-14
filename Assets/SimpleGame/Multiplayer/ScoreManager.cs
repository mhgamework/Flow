using System.Collections.Generic;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Logic for managing player lives, not very good design
    /// </summary>
    public class ScoreManager : Singleton<ScoreManager>
    {
        public int StartLives = 5;

        public List<PlayerLivesScript> Players = new List<PlayerLivesScript>();

        public void RegisterPlayer(PlayerLivesScript player)
        {
            Players.Add(player);
            player.ResetLives(StartLives);
        }

        public void UnRegisterPlayer(PlayerLivesScript player)
        {
            Players.Remove(player);
        }


        public void OnPlayerDeath(PlayerLivesScript player)
        {
            foreach (var p in Players)
                p.ResetLives(StartLives);
        }

        public void Clear()
        {
            Players.Clear();
        }
    }
}