# 【Unity】iOS でアプリのライフサイクルイベントやビューのイベントを受け取れるようにする

## はじめに

Unity で iOS アプリを開発していると、アプリケーションのライフサイクルイベントや UIViewController のイベントを細かく制御したい場面に遭遇することがあります。Unity 標準の `MonoBehaviour.OnApplicationPause` や `Application.lowMemory` などでは取得できない、より詳細なネイティブレベルのイベントを扱いたいケースです。

そんな時、Unity の iOS ビルドで出力されるプロジェクトを探っていたところ、`Classes/PluginBase` というディレクトリを見つけました。このディレクトリには、iOS のネイティブイベントを Unity から受け取るための仕組みが用意されています。

具体的には、以下の 3 つのリスナーインターフェースが提供されています。

- **UnityViewControllerListener** - UIViewController のライフサイクルイベント
- **LifeCycleListener** - アプリケーションのライフサイクルイベント
- **AppDelegateListener** - AppDelegate レベルのシステムイベント

本記事では、これらのリスナーの概要と、Unity プロジェクトで実際に活用する方法について解説します。

## 各種イベントについて

### UnityViewControllerListener について

`UnityViewControllerListener` は、iOS の UIViewController に関連するライフサイクルイベントを受け取るためのリスナーです。

**取得可能なイベント:**

| イベント | 説明 |
|---------|------|
| `viewWillLayoutSubviews` | ビューがサブビューをレイアウトする直前に呼ばれる |
| `viewDidLayoutSubviews` | ビューがサブビューをレイアウトした直後に呼ばれる |
| `viewWillAppear` | ビューが表示される直前に呼ばれる（アニメーションフラグ付き） |
| `viewDidAppear` | ビューが表示された直後に呼ばれる（アニメーションフラグ付き） |
| `viewWillDisappear` | ビューが非表示になる直前に呼ばれる（アニメーションフラグ付き） |
| `viewDidDisappear` | ビューが非表示になった直後に呼ばれる（アニメーションフラグ付き） |
| `interfaceWillChangeOrientation` | デバイスの画面方向が変更される直前に呼ばれる |
| `interfaceDidChangeOrientation` | デバイスの画面方向が変更された直後に呼ばれる |

**ユースケース:**
- ネイティブ UI との連携（ネイティブビューの追加・削除時の処理）
- 画面遷移の詳細な検知
- レイアウト変更のタイミングでの処理

### LifeCycleListener について

`LifeCycleListener` は、iOS アプリケーションと Unity ランタイムのライフサイクルイベントを受け取るためのリスナーです。

**取得可能なイベント:**

| イベント | 説明 |
|---------|------|
| `didFinishLaunching` | アプリケーションの起動処理が完了した直後に呼ばれる |
| `didBecomeActive` | アプリケーションがアクティブになった直後に呼ばれる |
| `willResignActive` | アプリケーションが非アクティブになる直前に呼ばれる |
| `didEnterBackground` | アプリケーションがバックグラウンドに移行した直後に呼ばれる |
| `willEnterForeground` | アプリケーションがフォアグラウンドに復帰する直前に呼ばれる |
| `willTerminate` | アプリケーションが終了する直前に呼ばれる |
| `unityDidUnload` | Unity がアンロードされた直後に呼ばれる |
| `unityDidQuit` | Unity が終了した直後に呼ばれる |

**ユースケース:**
- リソースの適切な管理（バックグラウンド移行時の解放など）
- 状態の保存・復元
- Unity エンジンのアンロード・終了タイミングでのクリーンアップ処理

### AppDelegateListener について

`AppDelegateListener` は、UIApplicationDelegate のイベントを受け取るためのリスナーです。このインターフェースは **LifeCycleListener を継承している** ため、LifeCycleListener のすべてのイベントに加えて、以下の AppDelegate 固有のイベントも受け取ることができます。

**取得可能なイベント（AppDelegate 固有）:**

| イベント | 説明 |
|---------|------|
| `onOpenURL` | URL スキームでアプリが起動された際に呼ばれる |
| `applicationWillFinishLaunchingWithOptions` | アプリケーションの起動処理が開始される直前に呼ばれる |
| `onHandleEventsForBackgroundURLSession` | バックグラウンド URL セッションのイベント処理時に呼ばれる |
| `applicationDidReceiveMemoryWarning` | メモリ不足の警告を受け取った際に呼ばれる |
| `applicationSignificantTimeChange` | 重要な時刻変更（日付変更、タイムゾーン変更など）時に呼ばれる |
| `applicationWillChangeStatusBarFrame` | ステータスバーのフレームが変更される直前に呼ばれる |
| `applicationWillChangeStatusBarOrientation` | ステータスバーの向きが変更される直前に呼ばれる |

**ユースケース:**
- ディープリンク処理（URL スキーム起動への対応）
- メモリ管理の最適化
- システムレベルのイベントへの対応

## Unity で活用する際のプラグインの実装例

それでは、実際に Unity でこれらのリスナーを活用する方法を見ていきましょう。本記事では `UnityViewControllerListener` を中心に解説しますが、他のリスナーも同様のパターンで実装されています。

### 全体のアーキテクチャ

プラグインの実装は、以下のような層構造になっています。

```
C# Unity Code (MonoBehaviour)
    ↓
Builder.Build(listener)  ← Builder パターンでインスタンス生成
    ↓
ListenerBridge (C#) - P/Invoke でネイティブと通信
    ↓
Native Bridge (Objective-C++) - NSNotification を受信
    ↓
iOS Native APIs (PluginBase) - Unity が提供するリスナープロトコル
```

### C# インターフェース定義

まず、C# 側でリスナーのインターフェースを定義します。

```csharp
namespace iOSUtility.NativeEventListener
{
    /// <summary>
    /// Unity の UIViewController のライフサイクルイベントを受け取るためのリスナーインターフェース。
    /// </summary>
    public interface IUnityViewControllerListener
    {
        /// <summary>
        /// ビューがサブビューをレイアウトする直前に呼ばれます。
        /// </summary>
        void OnViewWillLayoutSubviewsCallbacks();

        /// <summary>
        /// ビューがサブビューをレイアウトした直後に呼ばれます。
        /// </summary>
        void OnViewDidLayoutSubviewsCallbacks();

        /// <summary>
        /// ビューが表示される直前に呼ばれます。
        /// </summary>
        /// <param name="animated">アニメーション付きで表示される場合は true</param>
        void OnViewWillAppearCallbacks(bool animated);

        /// <summary>
        /// ビューが表示された直後に呼ばれます。
        /// </summary>
        /// <param name="animated">アニメーション付きで表示された場合は true</param>
        void OnViewDidAppearCallbacks(bool animated);

        /// <summary>
        /// ビューが非表示になる直前に呼ばれます。
        /// </summary>
        /// <param name="animated">アニメーション付きで非表示になる場合は true</param>
        void OnViewWillDisappearCallbacks(bool animated);

        /// <summary>
        /// ビューが非表示になった直後に呼ばれます。
        /// </summary>
        /// <param name="animated">アニメーション付きで非表示になった場合は true</param>
        void OnViewDidDisappearCallbacks(bool animated);

        /// <summary>
        /// デバイスの画面方向が変更される直前に呼ばれます。
        /// </summary>
        void OnInterfaceWillChangeOrientationCallbacks();

        /// <summary>
        /// デバイスの画面方向が変更された直後に呼ばれます。
        /// </summary>
        void OnInterfaceDidChangeOrientationCallbacks();
    }
}
```

### Builder パターンによるインスタンス生成

リスナーのインスタンス生成には Builder パターンを使用します。これにより、エディタ環境と iOS 実機環境で自動的に適切な実装を切り替えることができます。

```csharp
public static class UnityViewControllerListenerBuilder
{
    public static IDisposable Build(IUnityViewControllerListener listener)
    {
#if UNITY_IOS && !UNITY_EDITOR
        // iOS 実機の場合は実際のブリッジを生成
        var instance = new UnityViewControllerListenerBridge(listener);
#else
        // エディタ環境では何もしないダミーを返す
        var instance = new DummyBridge();
#endif
        return instance;
    }
}
```

返却される `IDisposable` を通じて、リスナーのリソース管理を適切に行うことができます。

### 使用例

実際の使用例を見てみましょう。

```csharp
using System;
using iOSUtility.NativeEventListener;
using UnityEngine;

public sealed class NativeEventListener : MonoBehaviour
{
    private IDisposable _unityViewControllerListenerBridge;
    private IDisposable _lifeCycleListenerBridge;
    private IDisposable _appDelegateListenerBridge;

    private void Start()
    {
        // Builder でリスナーを構築
        _unityViewControllerListenerBridge = UnityViewControllerListenerBuilder
            .Build(new UnityViewControllerListener());

        _lifeCycleListenerBridge = LifeCycleListenerBuilder
            .Build(new LifeCycleListener());

        _appDelegateListenerBridge = AppDelegateListenerBuilder
            .Build(new AppDelegateListener());
    }

    private void OnDestroy()
    {
        // リソースの解放（Dispose 呼び出し）
        _unityViewControllerListenerBridge?.Dispose();
        _lifeCycleListenerBridge?.Dispose();
        _appDelegateListenerBridge?.Dispose();
    }

    // UnityViewControllerListener の実装例
    private sealed class UnityViewControllerListener : IUnityViewControllerListener
    {
        public void OnViewWillLayoutSubviewsCallbacks()
        {
            Debug.Log("[UnityViewControllerListener] OnViewWillLayoutSubviews");
        }

        public void OnViewDidLayoutSubviewsCallbacks()
        {
            Debug.Log("[UnityViewControllerListener] OnViewDidLayoutSubviews");
        }

        public void OnViewWillAppearCallbacks(bool animated)
        {
            Debug.Log($"[UnityViewControllerListener] OnViewWillAppear (animated: {animated})");
        }

        public void OnViewDidAppearCallbacks(bool animated)
        {
            Debug.Log($"[UnityViewControllerListener] OnViewDidAppear (animated: {animated})");
        }

        public void OnViewWillDisappearCallbacks(bool animated)
        {
            Debug.Log($"[UnityViewControllerListener] OnViewWillDisappear (animated: {animated})");
        }

        public void OnViewDidDisappearCallbacks(bool animated)
        {
            Debug.Log($"[UnityViewControllerListener] OnViewDidDisappear (animated: {animated})");
        }

        public void OnInterfaceWillChangeOrientationCallbacks()
        {
            Debug.Log("[UnityViewControllerListener] OnInterfaceWillChangeOrientation");
        }

        public void OnInterfaceDidChangeOrientationCallbacks()
        {
            Debug.Log("[UnityViewControllerListener] OnInterfaceDidChangeOrientation");
        }
    }

    // 他のリスナーも同様に実装...
}
```

### イベント発火の確認

プロジェクトには、イベントが実際に発火することを確認するための `NativeUIController` が実装されています。このコントローラーを使って、ネイティブ側でビューの追加・削除を行うと、対応する `UnityViewControllerListener` のイベントが発火されます。

```csharp
private readonly INativeUIController _nativeUIController =
    NativeUIControllerFactory.CreateNativeUIController();

// ネイティブビューを追加（viewWillAppear などが発火）
private void OnAddNativeViewButtonClicked()
{
    _nativeUIController.AddSubview();
}

// ネイティブビューを削除（viewWillDisappear などが発火）
private void OnRemoveNativeViewButtonClicked()
{
    _nativeUIController.RemoveSubview();
}

// 破棄時にリソースを解放
private void OnDestroy()
{
    _nativeUIController.Dispose();
}
```

### ObjC++ で実装している理由

ネイティブプラグインの実装は Objective-C++ (.mm ファイル) で行っています。Swift でも実装可能ですが、以下の理由から Objective-C++ を採用しています。

**Unity の PluginBase へのアクセス:**
- `UnityViewControllerListener` などのプロトコルは、Unity が生成する Objective-C のヘッダーファイルで定義されています
- Swift からこれらにアクセスするには、Umbrella Header に登録する必要があり、手順が煩雑になります
- Objective-C++ であれば、直接 `#import` でヘッダーをインクルードできます

```objc
// Objective-C++ (.mm) での実装例
#import <Foundation/Foundation.h>

// Unity の PluginBase を直接インポート可能
@protocol UnityViewControllerListener;
void UnityRegisterViewControllerListener(id<UnityViewControllerListener> obj);
void UnityUnregisterViewControllerListener(id<UnityViewControllerListener> obj);

@interface UnityViewControllerListenerBridge : NSObject<UnityViewControllerListener>
// リスナーの実装...
@end
```

このように、Objective-C++ を使用することで、より簡潔にプラグインを実装できます。

## 各種 Listener がどう呼び出されているのか？

ここでは、ビルド後の iOS プロジェクトの内部を見て、リスナーがどのように呼び出されているのかを補足します。

### NSNotificationCenter ベースの設計

すべてのリスナーは、iOS の `NSNotificationCenter` を使った通知メカニズムで実装されています。

Unity の iOS ビルド出力には、以下のような登録・解除関数が用意されています。

```objc
// UnityViewControllerListener の登録
void UnityRegisterViewControllerListener(id<UnityViewControllerListener> obj);
// UnityViewControllerListener の解除
void UnityUnregisterViewControllerListener(id<UnityViewControllerListener> obj);

// LifeCycleListener の登録
void UnityRegisterLifeCycleListener(id<LifeCycleListener> obj);
// LifeCycleListener の解除
void UnityUnregisterLifeCycleListener(id<LifeCycleListener> obj);

// AppDelegateListener の登録
void UnityRegisterAppDelegateListener(id<AppDelegateListener> obj);
// AppDelegateListener の解除
void UnityUnregisterAppDelegateListener(id<AppDelegateListener> obj);
```

これらの関数内部では、`NSNotificationCenter` に対してオブザーバーを登録しています。

### PluginBase のプロトコル定義

Unity が生成する `Classes/PluginBase` ディレクトリには、以下のようなプロトコル定義があります。

**UnityViewControllerListener.h:**
```objc
@protocol UnityViewControllerListener<NSObject>
@optional
- (void)viewWillLayoutSubviews:(NSNotification*)notification;
- (void)viewDidLayoutSubviews:(NSNotification*)notification;
- (void)viewWillDisappear:(NSNotification*)notification;
- (void)viewDidDisappear:(NSNotification*)notification;
- (void)viewWillAppear:(NSNotification*)notification;
- (void)viewDidAppear:(NSNotification*)notification;
- (void)interfaceWillChangeOrientation:(NSNotification*)notification;
- (void)interfaceDidChangeOrientation:(NSNotification*)notification;
@end
```

**LifeCycleListener.h:**
```objc
@protocol LifeCycleListener<NSObject>
@optional
- (void)didFinishLaunching:(NSNotification*)notification;
- (void)didBecomeActive:(NSNotification*)notification;
- (void)willResignActive:(NSNotification*)notification;
- (void)didEnterBackground:(NSNotification*)notification;
- (void)willEnterForeground:(NSNotification*)notification;
- (void)willTerminate:(NSNotification*)notification;
- (void)unityDidUnload:(NSNotification*)notification;
- (void)unityDidQuit:(NSNotification*)notification;
@end
```

**AppDelegateListener.h:**
```objc
// LifeCycleListener を継承
@protocol AppDelegateListener<LifeCycleListener>
@optional
- (void)onOpenURL:(NSNotification*)notification;
- (void)applicationWillFinishLaunchingWithOptions:(NSNotification*)notification;
- (void)onHandleEventsForBackgroundURLSession:(NSNotification*)notification;
- (void)applicationDidReceiveMemoryWarning:(NSNotification*)notification;
- (void)applicationSignificantTimeChange:(NSNotification*)notification;
- (void)applicationWillChangeStatusBarFrame:(NSNotification*)notification;
- (void)applicationWillChangeStatusBarOrientation:(NSNotification*)notification;
@end
```

すべてのメソッドは `@optional` として定義されているため、必要なイベントだけを実装すればよい柔軟な設計になっています。

### 通知の発行元

**UnityViewControllerListener の場合:**
- `UnityViewControllerBase.mm` の lifecycle メソッドから通知が発行されます
- 例えば、`viewWillAppear:` が呼ばれると、内部で `kUnityViewWillAppear` という通知名で NSNotificationCenter に通知が投稿されます

**LifeCycleListener の場合:**
- iOS システムの標準通知（`UIApplicationDidBecomeActiveNotification` など）を利用
- Unity 固有のイベント（`kUnityDidUnload`、`kUnityDidQuit`）は `UnityAppController.mm` から発行されます

**AppDelegateListener の場合:**
- `UnityAppController.mm` の AppDelegate メソッド内から通知が発行されます
- 例えば、`application:openURL:` が呼ばれると、`kUnityOnOpenURL` という通知が投稿されます

### 通知フロー

全体の流れをまとめると、以下のようになります。

```
iOS システムイベント発生
    ↓
UnityAppController / UnityViewControllerBase がイベントを受信
    ↓
NSNotificationCenter に通知を投稿
    ↓
登録されているリスナー（プラグイン）がセレクタを通じてメソッド呼び出し
    ↓
C++ コールバック関数が実行される
    ↓
P/Invoke を通じて C# 側のメソッドが呼び出される
    ↓
Unity の MonoBehaviour で処理
```

このアーキテクチャにより、iOS ネイティブのイベントを Unity 側で安全かつ効率的に受け取ることができます。

## おわりに

本記事では、Unity の iOS ビルドで利用可能な `PluginBase` の各種リスナーについて解説しました。

Unity には標準でいくつかのライフサイクルイベントが用意されています。

- [`MonoBehaviour.OnApplicationPause`](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.OnApplicationPause.html) - アプリケーションの一時停止/再開
- [`Application.lowMemory`](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Application-lowMemory.html) - メモリ不足の警告

これらの標準機能で要件を満たせる場合は、わざわざネイティブプラグインを作成する必要はありません。

しかし、以下のようなケースでは、本記事で紹介した Native Event Listener が有用です。

- Unity 標準のイベントだけでは完結できない細かい制御が必要な場合
- ネイティブ UI を Unity のビューと組み合わせて使用する場合
- UIViewController の切り替えタイミングで特定の処理を行いたい場合
- AppDelegate レベルのシステムイベントに対応する必要がある場合

`PluginBase` は Unity が公式に提供している仕組みであり、ビルド出力に必ず含まれます。これを活用することで、iOS ネイティブの詳細なイベントを Unity から扱えるようになり、より高度なアプリケーション開発が可能になります。
