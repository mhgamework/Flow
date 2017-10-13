using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Homm.UI;
using UnityEngine;

namespace Assets.Homm
{
    public class GameMaster : Singleton<GameMaster>
    {
        private PlayerState playerState;

        public float StepSpeed = 0.2f;
        private HommMain main;

        public void Start()
        {
            GetComponentInChildren<GridUserInput>(true).ActivateState();
            playerState = PlayerState.Instance;
            main = HommMain.Instance;
        }
        public void Update()
        {

        }

        public void MoveHero()
        {
            GetComponentInChildren<GridUserInput>(true).DeactivateState();
            StartCoroutine(moveHeroRoutine().GetEnumerator());

        }


        private IEnumerable<YieldInstruction> moveHeroRoutine()
        {
            var w = playerState.Wizard;

            foreach (var f in w.MoveStep(StepSpeed))
                yield return f;

           

            GetComponentInChildren<GridUserInput>(true).ActivateState();

        }

        public void EndPlayerTurn()
        {
            GetComponentInChildren<GridUserInput>(true).DeactivateState();
            StartCoroutine(EndPlayerTurnRoutine().GetEnumerator());

        }
        private IEnumerable<YieldInstruction> EndPlayerTurnRoutine()
        {
            foreach (var f in DialogWindow.Instance.ShowCoroutine("A new day dawns.", "Next turn"))
                yield return f;


            playerState.Wizard.MovementLeft = playerState.Wizard.Movement;


            GetComponentInChildren<GridUserInput>(true).ActivateState();

        }
    }
}