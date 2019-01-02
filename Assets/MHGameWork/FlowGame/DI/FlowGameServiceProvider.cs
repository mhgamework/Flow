using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.MHGameWork.FlowGame.DI
{
    public class FlowGameServiceProvider
    {
        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        public T GetService<T>() where T : class
        {
            if (!services.ContainsKey(typeof(T)))
            {
                debugLogServices();
                throw new Exception("Cannot find service of type: " + typeof(T));
            }

            var obj = services[typeof(T)];
            if (!(obj is T)) throw new Exception("Service in dictionary does not match requested service type: " + typeof(T) + " - " + obj.GetType());
            return obj as T;
        }

        private void debugLogServices()
        {
            foreach (var serviceType in services.Keys)
            {
                Debug.Log("FlowGameServiceProvider: " + serviceType.Name + " - " + serviceType.Namespace);
            }
        }

        public void RegisterService<T>(T instance)
        {
            if (services.ContainsKey(typeof(T)))
                throw new Exception("Already has a service of type registered: " + typeof(T));
            services[typeof(T)] = instance;
        }


        public static FlowGameServiceProvider Instance { get; private set; }

        static FlowGameServiceProvider()
        {
            Instance = new FlowGameServiceProvider();
        }
    }
}