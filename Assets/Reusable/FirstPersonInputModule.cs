using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Gameplay
{
    /// <summary>
    /// This alternative input module fixes problems with the Standard StandaloneInputModule in unity
    /// It fixes problems when using a first person camera with locked cursor.
    /// </summary>
    [AddComponentMenu("Flow/FirstPersonInputModule")]
    public class FirstPersonInputModule : StandaloneInputModule
    {
        private readonly PointerInputModule.MouseState m_MouseState = new PointerInputModule.MouseState();

        protected override void ProcessMove(PointerEventData pointerEvent)
        {
            GameObject newEnterTarget = pointerEvent.pointerCurrentRaycast.gameObject;
            base.HandlePointerExitAndEnter(pointerEvent, newEnterTarget);
        }


        protected override MouseState GetMousePointerEventData(int id)
        {
            // Copied from unity base method
            PointerEventData data1;
            bool pointerData = this.GetPointerData(-1, out data1, true);
            data1.Reset();
            if (pointerData)
                data1.position = this.input.mousePosition;
            Vector2 mousePosition = this.input.mousePosition;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // Original line: data1.position = new Vector2(-1f, -1f);
                data1.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
                data1.delta = Vector2.zero;
            }
            else
            {
                data1.delta = mousePosition - data1.position;
                data1.position = mousePosition;
            }
            data1.scrollDelta = this.input.mouseScrollDelta;
            data1.button = PointerEventData.InputButton.Left;
            this.eventSystem.RaycastAll(data1, this.m_RaycastResultCache);
            RaycastResult firstRaycast = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
            data1.pointerCurrentRaycast = firstRaycast;
            this.m_RaycastResultCache.Clear();
            PointerEventData data2;
            this.GetPointerData(-2, out data2, true);
            this.CopyFromTo(data1, data2);
            data2.button = PointerEventData.InputButton.Right;
            PointerEventData data3;
            this.GetPointerData(-3, out data3, true);
            this.CopyFromTo(data1, data3);
            data3.button = PointerEventData.InputButton.Middle;
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, this.StateForMouseButton(0), data1);
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, this.StateForMouseButton(1), data2);
            this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, this.StateForMouseButton(2), data3);
            return this.m_MouseState;
        }


    }
}