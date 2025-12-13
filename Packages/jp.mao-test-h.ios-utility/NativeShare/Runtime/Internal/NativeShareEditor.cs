using System.Text;
using UnityEngine;

namespace iOSUtility.NativeShare
{
    internal sealed class NativeShareEditor : INativeShare
    {
        public void ShareFile(string filePath, string subject, string text)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Share file (Editor Mode): {filePath}");
            builder.AppendLine($"Subject: {subject}");
            builder.AppendLine($"Text: {text}");
            Debug.Log(builder.ToString());

            ShowInFileExplorer(filePath);
        }

        private static void ShowInFileExplorer(string filePath)
        {
            try
            {
#if UNITY_EDITOR_WIN
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
#elif UNITY_EDITOR_OSX
                System.Diagnostics.Process.Start("open", $"-R \"{filePath}\"");
#elif UNITY_EDITOR_LINUX
                System.Diagnostics.Process.Start("xdg-open", System.IO.Path.GetDirectoryName(filePath));
#endif
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to show file in explorer: {ex.Message}");
            }
        }
    }
}
