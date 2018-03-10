using System;
using Assets.SimpleGame.Scripts;
using Assets.SimpleGame.WardDrawing;
using DirectX11;

namespace Assets.SimpleGame.Wards
{
    [Serializable]
    public abstract class AbstractWardSpell : WardScriptableObject
    {
        public string WardShape;

        public Ward Ward { get; protected set; }

        public abstract void Cast(PlayerScript player);

        private void OnEnable()
        {
            if (WardShape == "light")
                Ward = Ward.Create(new Point3(0, 1, 0), new Point3(1, 0, 0), new Point3(0, -1, 0), new Point3(-1, 0, 0), new Point3(0, 1, 0));
            else if (WardShape == "invisibility")
                Ward = Ward.Create(
                    Ward.CreateLine(new Point3(0, 3, 0), new Point3(1, 2, 0), new Point3(2, 3, 0), new Point3(3, 2, 0), new Point3(4, 3, 0)),
                    Ward.CreateLine(new Point3(0, 0, 0), new Point3(1, 1, 0), new Point3(2, 0, 0), new Point3(3, 1, 0), new Point3(4, 0, 0))
                    );
            else if (WardShape == "explosion")
                Ward = Ward.Create(
                    Ward.CreateLine(new Point3(1, 1, 0), new Point3(-1, -2, 0)),
                    Ward.CreateLine(new Point3(1, -1, 0), new Point3(-1, 1, 0)));
            else if (WardShape == "haste")
                Ward = Ward.Create(
                    Ward.CreateLine(new Point3(0, 0, 0), new Point3(2, 1, 0), new Point3(4, 0, 0)),
                    Ward.CreateLine(new Point3(0, 0, 0), new Point3(1, 4, 0), new Point3(2, 5, 0), new Point3(3, 4, 0), new Point3(4, 0, 0))
                );
            else
                Ward = Ward.Create(new Point3(0, 1, 0), new Point3(1, 0, 0), new Point3(0, -1, 0), new Point3(-1, 0, 0), new Point3(0, 1, 0));
        }

    }
}