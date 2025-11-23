using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using iOSUtility.NativeShare;

namespace _Example
{
    public sealed class ViewController : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        private Button _shareButton;

        private void Start()
        {
            if (uiDocument == null)
            {
                Debug.LogError("UIDocument is not assigned.");
                return;
            }

            var root = uiDocument.rootVisualElement;
            _shareButton = root.Q<Button>("ShareButton");

            if (_shareButton != null)
            {
                _shareButton.clicked += OnShareButtonClicked;
            }
            else
            {
                Debug.LogError("ShareButton not found in UXML.");
            }
        }

        private void OnDestroy()
        {
            if (_shareButton != null)
            {
                _shareButton.clicked -= OnShareButtonClicked;
            }
        }

        private void OnShareButtonClicked()
        {
            StartCoroutine(CaptureAndShareScreenshot());
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
