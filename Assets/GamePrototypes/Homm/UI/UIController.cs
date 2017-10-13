using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Homm.UI
{
    public class UIController : MonoBehaviour
    {
        private Button btnEndTurn;
        public GridUserInput GridUserInput;
        private Text txtDescription;
        private HommMain hommMain;

        public void Start()
        {
            btnEndTurn = GetComponentsInChildren<Button>().First(f => f.name == "btnEndTurn");
            btnEndTurn.onClick.AddListener(onClick);

            txtDescription = GetComponentsInChildren<Text>().First(f => f.name == "txtDescription");

            hommMain = HommMain.Instance;

        }

        public void Update()
        {
            txtDescription.text = GridUserInput.HoveredCell.HasValue ? hommMain.Grid.Get(GridUserInput.HoveredCell.Value).Description : "";
        }

        private void onClick()
        {
            GameMaster.Instance.EndPlayerTurn();
        }
    }
}