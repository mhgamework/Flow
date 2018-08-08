using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.Multiplayer
{
    public class MultiplayerGameManager : Singleton<MultiplayerGameManager>
    {
        public List<Color> PlayerColors;


        public List<MultiplayerPlayerScript> GetActivePlayers()
        {
            return NetworkServer.connections.SelectMany(f => f.playerControllers)
                .Select(f => f.gameObject.GetComponent<MultiplayerPlayerScript>())
                .Where(f => f != null)
                .ToList();
        }

        public Color GetFreePlayerColor()
        {
            return PlayerColors.Except(GetActivePlayers().Select(p => p.GetPlayerColor())).First();
        }
    }
}