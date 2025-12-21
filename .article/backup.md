この記事は [Applibot Advent Calendar 2025](https://qiita.com/advent-calendar/2025/applibot) 24日目の記事です。

# はじめに

Unity で iOS 向けのプラグインを開発している時に「ビュー関連のイベントを取得できないか？」と調べていたところ、iOS ビルドで出力されるプロジェクト以下に `Classes/PluginBase` と言うディレクトリがあることを知り、さらにその中にある各種プロトコル[^1]と登録メソッドを用いれば簡単に取得できることがわかりました。

[^1]: protocol とは C# で言うところの `interface` に相当するもの。詳細は後述するが、こちらの protocol (interface) を実装したクラスを定義し、そちらを登録メソッドに渡す流れとなる。

具体的には以下 3 つの機能が提供されており、本記事ではこれらの概要とプラグインの実装例について解説します。

| Protocol | 概要 |
|:-:|:-:|
| `UnityViewControllerListener` | ビュー関連のイベント |
| `LifeCycleListener` | アプリのライフサイクルイベント |
| `AppDelegateListener` | システムイベント |

**検証環境**

- Unity `2022.3.62f2`, `6000.0.64f1`, `6000.3.1f1`
- Xcode 26.2





# 各種イベントについて

## `UnityViewControllerListener`

`UnityViewControllerListener` は、`UIViewController` に関連するライフサイクルイベントを受け取るためのリスナーです。[^2]

こちらを用いればネイティブプラグインで `UIView` を追加したり、他の `UIViewController` に切り替えた際のイベントなどを取得できるようになります。

[^2]: ちなみに Unity も内部的には `UIViewController` を生成し、その中の `UIView` を持つような構成となってます。

https://developer.apple.com/documentation/UIKit/UIViewController

### プロトコルの定義と取得可能なイベント

こちらは `Classess/PluginBase/UnityViewControllerListener.h` に定義されており、一部引用すると以下のようなプロトコルとなります。

::: note
詳細は後述しますが、基本的にはこちらのプロトコルを実装したクラスをプラグイン側で用意し、そのクラスのインスタンスを `UnityRegisterViewControllerListener` に渡して登録する流れとなります。
:::

```objc
#pragma once

#import <Foundation/NSNotification.h>

// view changes on the main view controller

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

// 中略

void UnityRegisterViewControllerListener(id<UnityViewControllerListener> obj);
void UnityUnregisterViewControllerListener(id<UnityViewControllerListener> obj);
```

また、イベント自体もネイティブ側の `UIViewController` が持つものとほぼ同一であり、具体的には以下のようなイベントを取得できます。[^3]

[^3]: 上記のプロトコルには他にも `interfaceWillChangeOrientation` と `interfaceDidChangeOrientation` がありますが...関連する情報が見当たらなかった上に、どこで呼び出されているのかも不明だったため、説明中では取り扱ってません...。 :bow: 

| イベント | 説明 (公式ドキュメントより引用) |
|---------|------|
| [`viewWillLayoutSubviews`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwilllayoutsubviews()) | Notifies the view controller that its view is about to lay out its subviews. |
| [`viewDidLayoutSubviews`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdidlayoutsubviews()) | Notifies the view controller when its view finishes laying out its subviews. |
| [`viewWillAppear`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwillappear(_:)) | Notifies the view controller that its view is about to be added to a view hierarchy. |
| [`viewDidAppear`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdidappear(_:)) | Notifies the view controller that its view was added to a view hierarchy. |
| [`viewWillDisappear`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwilldisappear(_:)) | Notifies the view controller that its view is about to be removed from a view hierarchy. |
| [`viewDidDisappear`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdiddisappear(_:)) | Notifies the view controller that its view was removed from a view hierarchy. |


## `LifeCycleListener`

`LifeCycleListener` は、アプリのライフサイクルイベントを受け取るためのリスナーです。

一応 Unity もサスレジのタイミングであれば [`OnApplicationPause`](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.OnApplicationPause.html) とかで取得することも可能ですが、さらに細かいタイミングが必要な場合には覚えておくと使えるかもしれません。

### プロトコルの定義と取得可能なイベント

こちらは `Classess/PluginBase/LifeCycleListener.h` に定義されており、一部引用すると以下のようなプロトコルとなります。

```objc
#pragma once

// important app life-cycle events

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

void UnityRegisterLifeCycleListener(id<LifeCycleListener> obj);
void UnityUnregisterLifeCycleListener(id<LifeCycleListener> obj);
```

イベントの方は標準で定義されている以下のイベントを `LifeCycleListener.mm` で登録し、そのまま流しているように思われました。

<details><summary> LifeCycleListener.mm (クリックで展開) </summary><div>

```objc
#include "LifeCycleListener.h"

#define DEFINE_NOTIFICATION(name) extern "C" __attribute__((visibility ("default"))) NSString* const name = @#name;
DEFINE_NOTIFICATION(kUnityDidUnload);
DEFINE_NOTIFICATION(kUnityDidQuit);

void UnityRegisterLifeCycleListener(id<LifeCycleListener> obj)
{
    #define REGISTER_SELECTOR(sel, notif_name)                  \
    if([obj respondsToSelector:sel])                            \
        [[NSNotificationCenter defaultCenter]   addObserver:obj \
                                                selector:sel    \
                                                name:notif_name \
                                                object:nil      \
        ];                                                      \

    REGISTER_SELECTOR(@selector(didFinishLaunching:), UIApplicationDidFinishLaunchingNotification);
    REGISTER_SELECTOR(@selector(didBecomeActive:), UIApplicationDidBecomeActiveNotification);
    REGISTER_SELECTOR(@selector(willResignActive:), UIApplicationWillResignActiveNotification);
    REGISTER_SELECTOR(@selector(didEnterBackground:), UIApplicationDidEnterBackgroundNotification);
    REGISTER_SELECTOR(@selector(willEnterForeground:), UIApplicationWillEnterForegroundNotification);
    REGISTER_SELECTOR(@selector(willTerminate:), UIApplicationWillTerminateNotification);
    REGISTER_SELECTOR(@selector(unityDidUnload:), kUnityDidUnload);
    REGISTER_SELECTOR(@selector(unityDidQuit:), kUnityDidQuit);

    #undef REGISTER_SELECTOR
}

void UnityUnregisterLifeCycleListener(id<LifeCycleListener> obj)
{
    [[NSNotificationCenter defaultCenter] removeObserver: obj name: UIApplicationDidFinishLaunchingNotification object: nil];
    [[NSNotificationCenter defaultCenter] removeObserver: obj name: UIApplicationDidBecomeActiveNotification object: nil];
    [[NSNotificationCenter defaultCenter] removeObserver: obj name: UIApplicationWillResignActiveNotification object: nil];
    [[NSNotificationCenter defaultCenter] removeObserver: obj name: UIApplicationDidEnterBackgroundNotification object: nil];
    [[NSNotificationCenter defaultCenter] removeObserver: obj name: UIApplicationWillEnterForegroundNotification object: nil];
    [[NSNotificationCenter defaultCenter] removeObserver: obj name: UIApplicationWillTerminateNotification object: nil];
    [[NSNotificationCenter defaultCenter] removeObserver: obj name: kUnityDidUnload object: nil];
    [[NSNotificationCenter defaultCenter] removeObserver: obj name: kUnityDidQuit object: nil];
}
```

</div></details>

| イベント | 説明 |
|---------|------|
| [`didFinishLaunching`](https://developer.apple.com/documentation/uikit/uiapplication/didfinishlaunchingnotification) | A notification that posts immediately after the app finishes launching. |
| [`didBecomeActive`](https://developer.apple.com/documentation/uikit/uiapplication/didbecomeactivenotification) | A notification that posts when the app becomes active. |
| [`willResignActive`](willResignActiveNotification) | A notification that posts when the app is no longer active and loses focus. |
| [`didEnterBackground`](https://developer.apple.com/documentation/uikit/uiapplication/didenterbackgroundnotification) | A notification that posts when the app enters the background. |
| [`willEnterForeground`](https://developer.apple.com/documentation/uikit/uiapplication/willenterforegroundnotification) | A notification that posts shortly before an app leaves the background state on its way to becoming the active app. |
| [`willTerminate`](https://developer.apple.com/documentation/uikit/uiapplication/willterminatenotification) | A notification that posts when the app is about to terminate. |

また、これ以外にも Unity 独自て定義したイベントも持っており、こちらもアプリのエントリーポイントのタイミングなどで発火しているのが確認できました。

| イベント | 説明 |
|---------|------|
| `unityDidUnload` | Unity がアンロードされた直後に呼ばれる |
| `unityDidQuit` | Unity が終了した直後に呼ばれる |




## AppDelegateListener について

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




# Unity で活用する際のプラグインの実装例

それでは、実際に Unity でこれらのリスナーを活用する方法を見ていきましょう。本記事では `UnityViewControllerListener` を中心に解説しますが、他のリスナーも同様のパターンで実装されています。

## 全体のアーキテクチャ

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

## C# インターフェース定義

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

## Builder パターンによるインスタンス生成

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

## 使用例

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

## イベント発火の確認

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

## ObjC++ で実装している理由

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



# 各種 Listener がどう呼び出されているのか？

ここでは、ビルド後の iOS プロジェクトの内部を見て、リスナーがどのように呼び出されているのかを補足します。

## NSNotificationCenter ベースの設計

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

## PluginBase のプロトコル定義

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

## 通知の発行元

**UnityViewControllerListener の場合:**
- `UnityViewControllerBase.mm` の lifecycle メソッドから通知が発行されます
- 例えば、`viewWillAppear:` が呼ばれると、内部で `kUnityViewWillAppear` という通知名で NSNotificationCenter に通知が投稿されます

**LifeCycleListener の場合:**
- iOS システムの標準通知（`UIApplicationDidBecomeActiveNotification` など）を利用
- Unity 固有のイベント（`kUnityDidUnload`、`kUnityDidQuit`）は `UnityAppController.mm` から発行されます

**AppDelegateListener の場合:**
- `UnityAppController.mm` の AppDelegate メソッド内から通知が発行されます
- 例えば、`application:openURL:` が呼ばれると、`kUnityOnOpenURL` という通知が投稿されます

## 通知フロー

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



# おわりに

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

