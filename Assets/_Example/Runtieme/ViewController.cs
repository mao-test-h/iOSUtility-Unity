using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using iOSUtility.NativeShare;
using UnityEngine.Assertions;

namespace _Example
{
    public sealed class ViewController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private Button _shareButton;
        private Button _playVideoButton;

        private void Start()
        {
            Assert.IsTrue(uiDocument != null, "UIDocument is not assigned.");

            var root = uiDocument.rootVisualElement;

            _shareButton = root.Q<Button>("ShareButton");
            Assert.IsTrue(_shareButton != null, "ShareButton not found in UXML.");
            _shareButton.clicked += OnShareButtonClicked;

            _playVideoButton = root.Q<Button>("PlayVideoButton");
            Assert.IsTrue(_playVideoButton != null, "PlayVideoButton not found in UXML.");
            _playVideoButton.clicked += OnPlayVideoButtonClicked;
        }

        private void OnDestroy()
        {
            _shareButton.clicked -= OnShareButtonClicked;
            _playVideoButton.clicked -= OnPlayVideoButtonClicked;
        }

        private void OnShareButtonClicked()
        {
            StartCoroutine(CaptureAndShareScreenshot());
        }

        private void OnPlayVideoButtonClicked()
        {
            // UIVIewController が内部的に切り替わった際のイベント確認用にフルスクリーン動画を再生
            const string url = "https://devstreaming-cdn.apple.com/videos/streaming/examples/bipbop_16x9/bipbop_16x9_variant.m3u8";
            var ret = Handheld.PlayFullScreenMovie(url);
            Debug.Log($"Handheld.PlayFullScreenMovie returned {ret}");
        }

        private IEnumerator CaptureAndShareScreenshot()
        {
            // Wait for end of frame to ensure the screen is fully rendered
            yield return new WaitForEndOfFrame();

            // Capture screenshot as Texture2D
            Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

            if (screenshot == null)
            {
                Debug.LogError("Failed to capture screenshot.");
                yield break;
            }

            try
            {
                // Encode to PNG
                byte[] bytes = screenshot.EncodeToPNG();

                // Save to temporary cache path
                string fileName = $"screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
                string filePath = Path.Combine(Application.temporaryCachePath, fileName);
                File.WriteAllBytes(filePath, bytes);

                Debug.Log($"Screenshot saved to: {filePath}");

                // Call NativeShare
                NativeShare.ShareFile(
                    filePath: filePath,
                    subject: "Screenshot from Unity",
                    text: "Check out this screenshot!"
                );
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save or share screenshot: {e.Message}");
            }
            finally
            {
                // Clean up texture
                Destroy(screenshot);
            }
        }
    }
}
