# iOSUtility-Unity

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?logo=unity)
![iOS](https://img.shields.io/badge/iOS-13.0%2B-000000?logo=apple)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

A Unity package that provides access to native iOS functionality through a clean C# API. This library allows Unity developers to easily integrate iOS native features such as event listeners and native sharing capabilities.

## Requirements

- Unity 2022.3 or later
- iOS 13.0+

## Installation

Install via Unity Package Manager.

1. Open your project in Unity Editor
2. Select Window > Package Manager
3. Click the "+" button
4. Select "Add package from git URL..." and enter the following URL

```
https://github.com/mao-test-h/iOSUtility-Unity.git?path=Packages/jp.mao-test-h.ios-utility
```

Or add the following to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "jp.mao-test-h.ios-utility": "https://github.com/mao-test-h/iOSUtility-Unity.git?path=Packages/jp.mao-test-h.ios-utility"
  }
}
```

## Features

### Native Share

Share files using iOS native share sheet (UIActivityViewController).

```csharp
NativeShare.ShareFile(
    filePath: "/path/to/file.png",
    subject: "Screenshot",
    text: "Check this out!"
);
```

### Native Event Listener

Monitor iOS native events from Unity with three types of listeners:

#### UnityViewController Listener
- `OnViewWillLayoutSubviews` / `OnViewDidLayoutSubviews`
- `OnViewWillAppear` / `OnViewDidAppear`
- `OnViewWillDisappear` / `OnViewDidDisappear`
- `OnInterfaceWillChangeOrientation` / `OnInterfaceDidChangeOrientation`

#### LifeCycle Listener
- `OnDidFinishLaunching`
- `OnDidBecomeActive` / `OnWillResignActive`
- `OnDidEnterBackground` / `OnWillEnterForeground`
- `OnWillTerminate`
- `OnUnityDidUnload` / `OnUnityDidQuit`

#### AppDelegate Listener
- `OnOpenURL` - Handle URL scheme deep links
- `OnApplicationWillFinishLaunchingWithOptions`
- `OnHandleEventsForBackgroundURLSession` - Background URL session handling
- `OnApplicationDidReceiveMemoryWarning` - Memory warning notifications
- `OnApplicationSignificantTimeChange` - Significant time change events



## Usage

### Native Event Listener Example

```csharp
using System;
using iOSUtility.NativeEventListener;
using UnityEngine;

public class EventListenerExample : MonoBehaviour
{
    private IDisposable _lifeCycleListenerBridge;

    private void Start()
    {
        // Create and register a lifecycle listener
        _lifeCycleListenerBridge = LifeCycleListenerBuilder
            .Build(new LifeCycleListener());
    }

    private void OnDestroy()
    {
        // Unregister listener
        _lifeCycleListenerBridge?.Dispose();
    }

    // Implement the listener interface
    private sealed class LifeCycleListener : ILifeCycleListener
    {
        public void OnDidBecomeActiveCallbacks()
        {
            Debug.Log("App became active");
        }

        public void OnDidEnterBackgroundCallbacks()
        {
            Debug.Log("App entered background");
        }

        // Implement other interface methods...
    }
}
```

### Native Share Example

```csharp
using UnityEngine;
using iOSUtility.NativeShare;

public class ShareExample : MonoBehaviour
{
    public void ShareScreenshot()
    {
        // Capture screenshot
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytes = screenshot.EncodeToPNG();

        // Save to temporary path
        string filePath = Path.Combine(Application.temporaryCachePath, "screenshot.png");
        File.WriteAllBytes(filePath, bytes);

        // Share using native share sheet
        NativeShare.ShareFile(
            filePath: filePath,
            subject: "Screenshot from Unity",
            text: "Check out this screenshot!"
        );

        Destroy(screenshot);
    }
}
```

## Example Project

The `Assets/_Example/` directory contains a working demo that showcases all features:

- **Screenshot Sharing**: Capture and share screenshots using native iOS share sheet
- **Event Logging**: Monitor and log all iOS lifecycle and delegate events
- **Video Playback**: Test ViewController transitions with fullscreen video playback

## License

MIT License
