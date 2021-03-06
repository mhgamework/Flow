﻿using System;
using System.Collections.Generic;
using Assets.Gandalf.Domain;

namespace Assets.Gandalf.Scripts
{
    public class GandalfDIScript : Singleton<GandalfDIScript>
    {
        public int GridSize = 10;
        public float GridCellSize = 1;

        Grid grid;
        public GridElementFactory GridElementFactory;

        private Dictionary<Type, Func<Object>> constructors;
        public void Start()
        {
            init();
        }

        private void init()
        {
            if (constructors != null) return;
            constructors = new Dictionary<Type, Func<object>>();

            RegisterSingleton(ci => new Grid(GridSize, GridCellSize));
            RegisterSingleton(ci => new TilePlaceHelper(ci.Get<Grid>()));
            RegisterSingleton(ci => new Wizard(ci.Get<IGridElementFactory>(),ci.Get<ExplorationService>()));
            RegisterSingleton(ci => new ExplorationService());
            RegisterSingleton(ci => new MagicChargeService());
            RegisterSingleton<IGridElementFactory>(ci => GridElementFactory);
        }

        private void RegisterSingleton<T>(Func<GandalfDIScript, T> ctr) where T : class
        {
            T theValue = null;
            constructors.Add(typeof(T), () =>
            {
                if (theValue == null)
                    theValue = ctr(this);
                return theValue;

            });
        }



        public T Get<T>() where T : class
        {
            init();
            if (!constructors.ContainsKey(typeof(T))) throw new Exception(typeof(T).FullName);

            return constructors[typeof(T)]() as T;

        }

    }
}
