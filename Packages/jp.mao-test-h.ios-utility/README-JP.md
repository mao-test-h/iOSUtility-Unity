# iOSUtility-Unity

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?logo=unity)
![iOS](https://img.shields.io/badge/iOS-13.0%2B-000000?logo=apple)
![License](https://img.shields.io/badge/License-MIT-blue.svg)

Unity から iOS のネイティブ機能にアクセスするための Unity パッケージです。クリーンな C# API を通じて、イベントリスナーやネイティブ共有機能などの iOS ネイティブ機能を簡単に統合できます。

## 動作環境

- Unity 2022.3+
- iOS 13.0+

## インストール

Unity Package Manager 経由でインストールします。

1. Unity エディタでプロジェクトを開く
2. Window > Package Manager を選択
3. "+" ボタンをクリック
4. "Add package from git URL..." を選択し、以下の URL を入力

```
https://github.com/mao-test-h/iOSUtility-Unity.git?path=Packages/jp.mao-test-h.ios-utility
```

または、`Packages/manifest.json` に以下を追加:

```json
{
  "dependencies": {
    "jp.mao-test-h.ios-utility": "https://github.com/mao-test-h/iOSUtility-Unity.git?path=Packages/jp.mao-test-h.ios-utility"
  }
}
```

## 機能

### Native Share

iOS のネイティブ共有シート（UIActivityViewController）を使ってファイルを共有します。

```csharp
NativeShare.ShareFile(
    filePath: "/path/to/file.png",
    subject: "Screenshot",
    text: "Check this out!"
);
```

### Native Event Listener

Unity から iOS のネイティブイベントを監視できます。3種類のリスナーが用意されています:

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
- `OnOpenURL` - URL スキームによるディープリンクの処理
- `OnApplicationWillFinishLaunchingWithOptions`
- `OnHandleEventsForBackgroundURLSession` - バックグラウンド URL セッションの処理
- `OnApplicationDidReceiveMemoryWarning` - メモリ警告の通知
- `OnApplicationSignificantTimeChange` - 時刻の大幅な変更イベント



## 使用方法

### Native Event Listener の使用例

```csharp
using System;
using iOSUtility.NativeEventListener;
using UnityEngine;

public class EventListenerExample : MonoBehaviour
{
    private IDisposable _lifeCycleListenerBridge;

    private void Start()
    {
        // ライフサイクルリスナーを作成して登録
        _lifeCycleListenerBridge = LifeCycleListenerBuilder
            .Build(new LifeCycleListener());
    }

    private void OnDestroy()
    {
        // リスナーの登録を解除
        _lifeCycleListenerBridge?.Dispose();
    }

    // リスナーインターフェースの実装
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

        // その他のインターフェースメソッドを実装...
    }
}
```

### Native Share の使用例

```csharp
using UnityEngine;
using iOSUtility.NativeShare;

public class ShareExample : MonoBehaviour
{
    public void ShareScreenshot()
    {
        // スクリーンショットをキャプチャ
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytes = screenshot.EncodeToPNG();

        // 一時パスに保存
        string filePath = Path.Combine(Application.temporaryCachePath, "screenshot.png");
        File.WriteAllBytes(filePath, bytes);

        // ネイティブ共有シートを使用して共有
        NativeShare.ShareFile(
            filePath: filePath,
            subject: "Screenshot from Unity",
            text: "Check out this screenshot!"
        );

        Destroy(screenshot);
    }
}
```

## サンプルプロジェクト

`Assets/_Example/` ディレクトリに全ての機能を実演する動作サンプルが含まれています:

- **Screenshot Sharing**: iOS のネイティブ共有シートを使ったスクリーンショットのキャプチャと共有
- **Event Logging**: iOS のライフサイクルとデリゲートイベントの監視とログ出力
- **Video Playback**: フルスクリーン動画再生による ViewController 遷移のテスト

## ライセンス

MIT License
