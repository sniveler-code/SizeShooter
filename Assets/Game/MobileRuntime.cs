using UnityEngine;

namespace SizeShooter
{
    public static class MobileRuntime
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            bool webgl = Application.platform == RuntimePlatform.WebGLPlayer;
            if (!webgl && !Application.isMobilePlatform) return;

            int level = Mathf.Min(2, QualitySettings.names.Length - 1);
            if (QualitySettings.GetQualityLevel() > level) QualitySettings.SetQualityLevel(level, true);

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 0;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
