using System;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.WardDrawing;

namespace Assets.SimpleGame.Wards
{
    [Serializable]
    public abstract class AbstractWardSpell : WardScriptableObject
    {
        public Ward Ward { get; protected set; }

        public abstract void Cast(PlayerScript player);

    }
}