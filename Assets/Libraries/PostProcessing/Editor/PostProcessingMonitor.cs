using System;
using UnityEngine;
using UnityEngine.PostProcessing;

namespace UnityEditor.PostProcessing
{
    //MH: Disabled because ide problemsusing MonitorSettings = UnityEngine.PostProcessing.PostProcessingProfile.MonitorSettings;

    public abstract class PostProcessingMonitor : IDisposable
    {
        protected PostProcessingProfile.MonitorSettings m_MonitorSettings;
        protected PostProcessingInspector m_BaseEditor;

        public void Init(PostProcessingProfile.MonitorSettings monitorSettings, PostProcessingInspector baseEditor)
        {
            m_MonitorSettings = monitorSettings;
            m_BaseEditor = baseEditor;
        }

        public abstract bool IsSupported();

        public abstract GUIContent GetMonitorTitle();

        public virtual void OnMonitorSettings()
        {}

        public abstract void OnMonitorGUI(Rect r);

        public virtual void OnFrameData(RenderTexture source)
        {}

        public virtual void Dispose()
        {}
    }
}
