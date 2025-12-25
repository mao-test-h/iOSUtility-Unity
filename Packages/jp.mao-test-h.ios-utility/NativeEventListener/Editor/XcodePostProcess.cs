using UnityEditor;
using UnityEditor.Callbacks;

namespace iOSUtility.NativeEventListener.Editor
{
    internal static class XcodePostProcess
    {
        [PostProcessBuild]
        private static void OnPostProcessBuild(BuildTarget target, string xcodeprojPath)
        {
            if (target != BuildTarget.iOS) return;

            // TODO:
        }
    }
}
