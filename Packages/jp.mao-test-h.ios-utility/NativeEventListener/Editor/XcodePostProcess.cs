using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace iOSUtility.NativeEventListener.Editor
{
    internal static class XcodePostProcess
    {
        private const string LogPrefix = "[NativeEventListener.PostProcess]";
        private const string NotificationKeyWillChangeOrientation = "kUnityInterfaceWillChangeOrientation";
        private const string NotificationKeyDidChangeOrientation = "kUnityInterfaceDidChangeOrientation";

        [PostProcessBuild]
        private static void OnPostProcessBuild(BuildTarget target, string xcodeprojPath)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            PatchUnityAppController(xcodeprojPath);
        }

        private static void PatchUnityAppController(string xcodeprojPath)
        {
            var filePath = Path.Combine(xcodeprojPath, "Classes", "UI", "UnityAppController+ViewHandling.mm");
            if (!File.Exists(filePath))
            {
                Debug.LogError(
                    $"{LogPrefix} UnityAppController+ViewHandling.mm not found at: {filePath}\n" +
                    "This may be due to a Unity version update. Orientation change notifications will not work.");
                return;
            }

            // ソースに対してパッチ適用
            var fileContent = File.ReadAllText(filePath);
            fileContent = PatchMethod(
                fileContent,
                "- (void)interfaceWillChangeOrientationTo:(UIInterfaceOrientation)toInterfaceOrientation",
                $"AppController_SendUnityViewControllerNotification(@\"{NotificationKeyWillChangeOrientation}\");    // Added by iOSUtility.NativeEventListener",
                NotificationKeyWillChangeOrientation
            );

            fileContent = PatchMethod(
                fileContent,
                "- (void)interfaceDidChangeOrientationFrom:(UIInterfaceOrientation)fromInterfaceOrientation",
                $"AppController_SendUnityViewControllerNotification(@\"{NotificationKeyDidChangeOrientation}\");    // Added by iOSUtility.NativeEventListener",
                NotificationKeyDidChangeOrientation
            );

            File.WriteAllText(filePath, fileContent);
            Debug.Log($"{LogPrefix} Successfully patched UnityAppController+ViewHandling.mm");
        }

        private static string PatchMethod(string fileContent, string methodSignature, string notificationCall, string notificationKey)
        {
            // 既にパッチ済みかチェック
            if (fileContent.Contains($"AppController_SendUnityViewControllerNotification(@\"{notificationKey}\")"))
            {
                Debug.Log($"{LogPrefix} UnityAppController+ViewHandling.mm is already patched for '{notificationKey}'.");
                return fileContent;
            }

            // メソッドシグネチャを検索
            var methodIndex = fileContent.IndexOf(methodSignature, StringComparison.Ordinal);
            if (methodIndex == -1)
            {
                Debug.LogError($"Method signature not found: {methodSignature}");
                return fileContent;
            }

            // メソッドの開始括弧を検索
            var openBraceIndex = fileContent.IndexOf('{', methodIndex);
            if (openBraceIndex == -1)
            {
                Debug.LogError($"Opening brace not found for method: {methodSignature}");
                return fileContent;
            }

            // 開始括弧の次の行を挿入位置とする
            var insertIndex = openBraceIndex + 1;
            while (insertIndex < fileContent.Length && (fileContent[insertIndex] == '\n' || fileContent[insertIndex] == '\r'))
            {
                insertIndex++;
            }

            // 次の行のインデントを取得し、コードを挿入
            var indent = GetIndent(fileContent, insertIndex);
            var codeToInsert = $"{indent}{notificationCall}\n{indent}\n";
            return fileContent.Insert(insertIndex, codeToInsert);
        }

        private static string GetIndent(string fileContent, int position)
        {
            var indent = "";
            while (position < fileContent.Length && (fileContent[position] == ' ' || fileContent[position] == '\t'))
            {
                indent += fileContent[position];
                position++;
            }

            return indent;
        }
    }
}
