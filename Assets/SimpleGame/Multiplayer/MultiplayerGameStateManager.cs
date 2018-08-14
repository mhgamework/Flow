using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Manager gameplay state for the multiplayer game, for now just player colors
    /// </summary>
    public class MultiplayerGameStateManager : Singleton<MultiplayerGameStateManager>
    {
        public List<Color> PlayerColors;


        public List<MultiplayerScenePlayerScript> GetActivePlayers()
        {
            return NetworkServer.connections.SelectMany(f => f.playerControllers)
                .Select(f => f.gameObject.GetComponent<MultiplayerScenePlayerScript>())
                .Where(f => f != null)
                .ToList();
        }

        public Color GetFreePlayerColor()
        {
            return PlayerColors.Except(GetActivePlayers().Select(p => p.GetPlayerColor())).First();
        }
    }
}