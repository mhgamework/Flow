using UnityEngine;
using UnityEngine.UI;

namespace Assets.Homm.UI
{
    public class ResourceAmountPanel:MonoBehaviour
    {
        public ResourceType Type;
        private PlayerState playerState;
        private Text text;

        public void Start()
        {
            playerState = PlayerState.Instance;
            text = GetComponentInChildren<Text>();
        }

        public void Update()
        {
            text.text = Type + ": " + playerState.GetResourceAmount(Type);
        }
    }
}