using UnityEngine.UI;

namespace Assets.MarchingCubes
{
    public class DebugText : Singleton<DebugText>
    {
        public void Start()
        {
            text = GetComponent<Text>();
        }
        private string newText = "";
        private Text text;

        public void LateUpdate()
        {
            text.text = newText;
            newText = "";
        }

        public void SetText(string label, string value)
        {
            newText += label + ": " + value + "\n";
        }
    }
}