using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Homm.UI
{
    public class UIController : MonoBehaviour
    {
        private Button btnEndTurn;

        public void Start()
        {
            btnEndTurn = GetComponentsInChildren<Button>().First(f => f.name == "btnEndTurn");
            btnEndTurn.onClick.AddListener(onClick);
        }

        private void onClick()
        {
            GameMaster.Instance.EndPlayerTurn();
        }
    }
}