using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleGame.Multiplayer.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleGame.Multiplayer
{
    /// <summary>
    /// Simple text based ui for the Player lives using the ScoreManager
    /// </summary>
    public class PlayerLivesUIScript : MonoBehaviour
    {
        public RectTransform LinePrefab;
        private static MultiplayerScenePlayerScript _multiplayerPlayerScript;

        public void Start()
        {
            map(ScoreManager.Instance.Players,
                p =>
                {
                    var ret = Instantiate(LinePrefab, transform);
                    ret.gameObject.SetActive(true);
                    return ret;
                },
                (p, o) => { updateLiveLine(o, p); },
                (o) => { Destroy(o.gameObject); },
                0.1f
            );
        }

        private static void updateLiveLine(RectTransform o, PlayerLivesScript p)
        {
            _multiplayerPlayerScript = p.GetComponent<MultiplayerScenePlayerScript>();
            var pColor = _multiplayerPlayerScript.GetComponent<PlayerColorScript>().GetPlayerColor();

            string you = "";
            if (_multiplayerPlayerScript.isLocalPlayer)
                you = " (you)";

            o.GetComponent<Text>().text = "Player: " + p.Lives + you;
            o.GetComponent<Text>().color = pColor;
        }

        private void map<TData, TCreated>(List<TData> list, Func<TData, TCreated> create,
            Action<TData, TCreated> update, Action<TCreated> delete,
            float interval) where TData : class
        {
            StartCoroutine(mapRoutine(list, create, update, delete, interval).GetEnumerator());
        }

        private IEnumerable<YieldInstruction> mapRoutine<TData, TCreated>(List<TData> list,
            Func<TData, TCreated> create,
            Action<TData, TCreated> update, Action<TCreated> delete,
            float interval) where TData : class
        {
            var items = new List<TCreated>();
            var datas = new List<TData>();

            for (;;)
            {
                bool recreate = datas.Count != list.Count;
                if (!recreate)
                    for (int i = 0; i < list.Count; i++)
                    {
                        var data = list[i];
                        if (datas[i] != data)
                        {
                            recreate = true;
                            break;
                        }
                    }

                try
                {
                    if (recreate)
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            delete(items[i]);
                        }

                        datas.Clear();
                        items.Clear();
                        for (int i = 0; i < list.Count; i++)
                        {
                            var d = list[i];
                            datas.Add(d);
                            items.Add(create(d));
                        }
                    }

                    // Update
                    for (int i = 0; i < list.Count; i++)
                    {
                        update(datas[i], items[i]);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }


                yield return new WaitForSeconds(interval);
            }
        }
    }
}