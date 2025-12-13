#if UNITY_IOS
using System.Runtime.InteropServices;

namespace iOSUtility.NativeShare
{
    internal sealed class NativeShareIOS : INativeShare
    {
        public void ShareFile(string filePath, string subject, string text)
        {
            NativeShareFile(filePath, subject, text);
        }

        [DllImport("__Internal", EntryPoint = "iOSUtility_NativeShare_ShareFile")]
        private static extern void NativeShareFile(string filePath, string subject, string text);
    }
}
#endif
