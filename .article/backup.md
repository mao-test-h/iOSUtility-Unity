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

こちらを用いればネイティブプラグインで `UIView` を追加したり、他の `UIViewController` に切り替えた際のイベントなどを検知できるようになります。

[^2]: ちなみに Unity も内部的には `UIViewController` を生成し、その中の `UIView` を持つような構成となってます。

https://developer.apple.com/documentation/UIKit/UIViewController

### プロトコルの定義と取得可能なイベント

こちらは `Classess/PluginBase/UnityViewControllerListener.h` に定義されており、一部引用すると以下のようなプロトコルとなります。


```objc:UnityViewControllerListener.h
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

::: note
詳細は後述しますが、基本的にはこちらのプロトコルを実装したクラスをプラグイン側で用意し、そのクラスのインスタンスを `UnityRegisterViewControllerListener` に渡して登録する流れとなります。
:::

また、イベント自体もネイティブ側の `UIViewController` が持つものとほぼ同一であり、具体的には以下のようなイベントを取得できます。[^3]

[^3]: 上記のプロトコルには他にも `interfaceWillChangeOrientation` と `interfaceDidChangeOrientation` がありますが...関連する情報が見当たらなかった上に、どこで呼び出されているのかも不明だったため、説明中では取り扱ってません...。 :bow: 

| メソッド | 対応するイベント |
|---------|------|
| `viewWillLayoutSubviews` | [`UIViewController.viewWillLayoutSubviews()`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwilllayoutsubviews()) |
| `viewDidLayoutSubviews` | [`UIViewController.viewDidLayoutSubviews()`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdidlayoutsubviews()) |
| `viewWillAppear` | [`UIViewController.viewWillAppear(_:)`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwillappear(_:)) |
| `viewDidAppear` | [`UIViewController.viewDidAppear(_:)`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdidappear(_:)) |
| `viewWillDisappear` | [`UIViewController.viewWillDisappear(_:)`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwilldisappear(_:)) |
| `viewDidDisappear` | [`UIViewController.viewDidDisappear(_:)`](https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdiddisappear(_:)) |



## `LifeCycleListener`

`LifeCycleListener` は、アプリのライフサイクルイベントを受け取るためのリスナーです。

一応 Unity もサスレジのタイミングであれば [`OnApplicationPause`](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.OnApplicationPause.html) とかで取得することも可能ですが、さらに細かいタイミングが必要な場合には覚えておくと使えるかもしれません。

### プロトコルの定義と取得可能なイベント

こちらは `Classess/PluginBase/LifeCycleListener.h` に定義されており、一部引用すると以下のようなプロトコルとなります。

```objc:LifeCycleListener.h
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

| メソッド | 対応するイベント・通知|
|---------|------|
| `didFinishLaunching` | [`UIApplicationDidFinishLaunchingNotification`](https://developer.apple.com/documentation/uikit/uiapplication/didfinishlaunchingnotification?language=objc) |
| `didBecomeActive` | [`UIApplicationDidBecomeActiveNotification`](https://developer.apple.com/documentation/uikit/uiapplication/didbecomeactivenotification?language=objc) |
| `willResignActive` | [`UIApplicationWillResignActiveNotification`](https://developer.apple.com/documentation/uikit/uiapplication/willresignactivenotification?language=objc) |
| `didEnterBackground` | [`UIApplicationDidEnterBackgroundNotification`](https://developer.apple.com/documentation/uikit/uiapplication/didenterbackgroundnotification?language=objc) |
| `willEnterForeground` | [`UIApplicationWillEnterForegroundNotification`](https://developer.apple.com/documentation/uikit/uiapplication/willenterforegroundnotification?language=objc) |
| `willTerminate` | [`UIApplicationWillTerminateNotification`](https://developer.apple.com/documentation/uikit/uiapplication/willterminatenotification?language=objc) |

また、これ以外にも Unity 独自て定義したイベントも持っており、こちらもアプリのエントリーポイントのタイミングなどで発火しているのが確認できました。

| メソッド | 説明 |
|---------|------|
| `unityDidUnload` | Unity がアンロードされた直後に呼ばれる |
| `unityDidQuit` | Unity が終了した直後に呼ばれる |




## AppDelegateListener について

`AppDelegateListener` は、`UIApplicationDelegate` のイベントを受け取るためのリスナーです。

こちらのインターフェースは上述の **`LifeCycleListener` を継承しており**、`LifeCycleListener`のイベントに加えて、以下の `UIApplicationDelegate` のイベントも受け取ることができます。

### プロトコルの定義と取得可能なイベント

こちらは `Classess/PluginBase/AppDelegateListener.h` に定義されており、一部引用すると以下のようなプロトコルとなります。

```objc:AppDelegateListener.h
#pragma once

#include "LifeCycleListener.h"


@protocol AppDelegateListener<LifeCycleListener>
@optional
// these do not have apple defined notifications, so we use our own notifications

// notification will be posted from
// - (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
// notification user data is the NSDictionary containing all the params
- (void)onOpenURL:(NSNotification*)notification;

// notification will be posted from
// - (BOOL)application:(UIApplication*)application willFinishLaunchingWithOptions:(NSDictionary*)launchOptions
// notification user data is the NSDictionary containing launchOptions
- (void)applicationWillFinishLaunchingWithOptions:(NSNotification*)notification;
// notification will be posted from
// - (void)application:(UIApplication*)application handleEventsForBackgroundURLSession:(nonnull NSString *)identifier completionHandler:(nonnull void (^)())completionHandler
// notification user data is NSDictionary with one item where key is session identifier and value is completion handler
- (void)onHandleEventsForBackgroundURLSession:(NSNotification*)notification;

// these are just hooks to existing notifications
- (void)applicationDidReceiveMemoryWarning:(NSNotification*)notification;
- (void)applicationSignificantTimeChange:(NSNotification*)notification;
- (void)applicationWillChangeStatusBarFrame:(NSNotification*)notification;
- (void)applicationWillChangeStatusBarOrientation:(NSNotification*)notification;
@end

void UnityRegisterAppDelegateListener(id<AppDelegateListener> obj);
void UnityUnregisterAppDelegateListener(id<AppDelegateListener> obj);
```

取得できるイベントは標準に定義されている通知に加え、幾つかは Unity が内部的に実装している `UIApplicationDelegate` の実装クラスより呼び出されます。


| メソッド | 対応するイベント・通知|
|---------|------|
| `onOpenURL` | [`UIApplicationDelegate.application:openURL:options:`](https://developer.apple.com/documentation/uikit/uiapplicationdelegate/application(_:open:options:)?language=objc) |
| `applicationWillFinishLaunchingWithOptions` |[`UIApplicationDelegate.application:willFinishLaunchingWithOptions:`](https://developer.apple.com/documentation/uikit/uiapplicationdelegate/application(_:willfinishlaunchingwithoptions:)?language=objc) |
| `onHandleEventsForBackgroundURLSession` | [`UIApplicationDelegate.application:handleEventsForBackgroundURLSession:completionHandler:`](https://developer.apple.com/documentation/uikit/uiapplicationdelegate/application(_:handleeventsforbackgroundurlsession:completionhandler:)?language=objc) |
| `applicationDidReceiveMemoryWarning` | [`UIApplicationDidReceiveMemoryWarningNotification`](https://developer.apple.com/documentation/uikit/uiapplication/didreceivememorywarningnotification?language=objc) |
| `applicationSignificantTimeChange` | [`UIApplicationSignificantTimeChangeNotification`](https://developer.apple.com/documentation/uikit/uiapplication/significanttimechangenotification?language=objc) |
| `applicationWillChangeStatusBarFrame` | [`UIApplicationWillChangeStatusBarFrameNotification`](https://developer.apple.com/documentation/uikit/uiapplication/willchangestatusbarframenotification?language=objc) |
| `applicationWillChangeStatusBarOrientation` | [`UIApplicationWillChangeStatusBarOrientationNotification`](https://developer.apple.com/documentation/uikit/uiapplication/willchangestatusbarorientationnotification?language=objc) |






# Unity で活用する際の実装例

それでは、実際に Unity でこれらのリスナーを活用する方法について簡単に解説していきます。
本記事では `UnityViewControllerListener` を中心に解説しますが、他のリスナーも同様のパターンで実装されています。


また、今回解説したリスナーの登録周りを機能化したパッケージの方も公開しており、こちらの方も実装サンプルとして参考にしていただけると幸いです。

https://github.com/mao-test-h/iOSUtility-Unity


## ネイティブプラグインの実装

まずは `UnityViewControllerListener` を受け取るための実装から入ります。

まずはイベントを受け取るためのクラスとして、`UnityViewControllerListener.h` に定義されている `UnityViewControllerListener` を実装したクラスを用意します。[^4]

[^4]: 全部定義すると数が多いので、ここでは解説向けに `viewWillLayoutSubviews`, `viewDidLayoutSubviews` だけ定義してます。

```objc:UnityViewControllerListenerBridge.mm
#include "PluginBase/UnityViewControllerListener.h" 
#include <stdint.h>

// view changes on the main view controller
typedef void (*ViewWillLayoutSubviewsCallback)(void* context);
typedef void (*ViewDidLayoutSubviewsCallback)(void* context);
// (中略)

@interface UnityViewControllerListenerBridge : NSObject<UnityViewControllerListener>
@property (nonatomic, assign) ViewWillLayoutSubviewsCallback viewWillLayoutSubviewsCallback;
@property (nonatomic, assign) ViewDidLayoutSubviewsCallback viewDidLayoutSubviewsCallback;
// (中略)
@end

@implementation UnityViewControllerListenerBridge

- (void)viewWillLayoutSubviews:(NSNotification*)notification
{
    if (self.viewWillLayoutSubviewsCallback) {
        self.viewWillLayoutSubviewsCallback((__bridge void*)self);
    }
}

- (void)viewDidLayoutSubviews:(NSNotification*)notification
{
    if (self.viewDidLayoutSubviewsCallback) {
        self.viewDidLayoutSubviewsCallback((__bridge void*)self);
    }
}

// (中略)

```

あとは P/Invoke から上述のクラスの生成・破棄を行うためのメソッドと、こちらを `UnityViewControllerListener.h` にある **`UnityRegisterViewControllerListener`, `UnityUnregisterViewControllerListener`** へ登録するためのメソッドも用意します。


```objc:UnityViewControllerListenerBridge.mm
#ifdef __cplusplus
extern "C" {
#endif

// 通知を受け取るためのクラスの生成
void* iOSUtility_NativeEventListener_CreateUnityViewControllerListenerBridge(
    ViewWillLayoutSubviewsCallback viewWillLayoutSubviewsCallback,
    ViewDidLayoutSubviewsCallback viewDidLayoutSubviewsCallback,
    // (中略)
{
    // 生成したクラスに対し、引数から渡されたコールバックを登録していく
    UnityViewControllerListenerBridge* bridge = [[UnityViewControllerListenerBridge alloc] init];
    bridge.viewWillLayoutSubviewsCallback = viewWillLayoutSubviewsCallback;
    bridge.viewDidLayoutSubviewsCallback = viewDidLayoutSubviewsCallback;
    // (中略)
    return (__bridge_retained void*)bridge;
}

// 生成したクラスの破棄
void iOSUtility_NativeEventListener_ReleaseUnityViewControllerListenerBridge(void* ptr)
{
    UnityViewControllerListenerBridge* bridge = (__bridge_transfer UnityViewControllerListenerBridge*)ptr;
    bridge.viewWillLayoutSubviewsCallback = nil;
    bridge.viewDidLayoutSubviewsCallback = nil;
    // (中略)
}

void iOSUtility_NativeEventListener_UnityRegisterViewControllerListener(void* ptr)
{
    UnityViewControllerListenerBridge* bridge = (__bridge UnityViewControllerListenerBridge*)ptr;

    // NOTE: UnityViewControllerListener.h にある登録用関数
    UnityRegisterViewControllerListener(bridge);
}

void iOSUtility_NativeEventListener_UnityUnregisterViewControllerListener(void* ptr)
{
    UnityViewControllerListenerBridge* bridge = (__bridge UnityViewControllerListenerBridge*)ptr;
    UnityUnregisterViewControllerListener(bridge);
}

#ifdef __cplusplus
}
#endif
```

:::note
ちなみに iOS 向けのネイティブプラグインは [Swift で実装することも可能](https://qiita.com/mao_/items/07466e169d08cbeff221)ですが、今回は意図的に ObjC++ で実装してます。

と言うのも `UnityViewControllerListener` が定義されているヘッダーファイルはそのままだと Swift からアクセスすることができず、参照するためには Umbrella Header から見えるようにするといった対応が必要になります。

こうなってくると Umbrella Header に該当する `UnityFramework.h` を書き換えるための対応が必要だったりと、色々と手間が発生するので、今回は ObjC++ で実装したと言う経緯があります。
:::


## C# 側の実装

対応する C# 側の実装は次のようになります。

ポイントとしては、複数のインスタンスの登録に対応できるように、自身のインスタンスのポインタとインターフェースを `Dictionary` で持っておき、呼び出しに応じて対象のインスタンスを指定できるようにしてあります。

```csharp:UnityViewControllerListenerBridge.cs
internal sealed class UnityViewControllerListenerBridge : IDisposable
{
    private static readonly Dictionary<IntPtr, IUnityViewControllerListener> Listeners = new();
    private readonly IntPtr _ptr;
    private bool _disposed;

    public UnityViewControllerListenerBridge(IUnityViewControllerListener lifecycleListener)
    {
        var ptr = CreateUnityViewControllerListenerBridge(
            ViewWillLayoutSubviewsCallbackStatic,
            ViewDidLayoutSubviewsCallbackStatic,
            // (中略)
            );

        Assert.IsNotNull(lifecycleListener);
        Listeners[ptr] = lifecycleListener;
        UnityRegisterViewControllerListener(ptr);
        
        _ptr = ptr;
    }

    ~UnityViewControllerListenerBridge()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            Assert.IsTrue(_ptr != IntPtr.Zero);

            if (disposing)
            {
                // release managed resources
            }

            Listeners.Remove(_ptr);
            UnityUnregisterViewControllerListener(_ptr);
            ReleaseUnityViewControllerListenerBridge(_ptr);
            _disposed = true;
        }
    }


    [DllImport("__Internal", EntryPoint = "iOSUtility_NativeEventListener_CreateUnityViewControllerListenerBridge")]
    private static extern IntPtr CreateUnityViewControllerListenerBridge(
        ViewWillLayoutSubviewsCallback viewWillLayoutSubviewsCallback,
        ViewDidLayoutSubviewsCallback viewDidLayoutSubviewsCallback,
        // (中略)
        );

    [DllImport("__Internal", EntryPoint = "iOSUtility_NativeEventListener_ReleaseUnityViewControllerListenerBridge")]
    private static extern void ReleaseUnityViewControllerListenerBridge(IntPtr ptr);

    [DllImport("__Internal", EntryPoint = "iOSUtility_NativeEventListener_UnityRegisterViewControllerListener")]
    private static extern void UnityRegisterViewControllerListener(IntPtr ptr);

    [DllImport("__Internal", EntryPoint = "iOSUtility_NativeEventListener_UnityUnregisterViewControllerListener")]
    private static extern void UnityUnregisterViewControllerListener(IntPtr ptr);


    private delegate void ViewWillLayoutSubviewsCallback(IntPtr context);
    private delegate void ViewDidLayoutSubviewsCallback(IntPtr context);

    [MonoPInvokeCallback(typeof(ViewWillLayoutSubviewsCallback))]
    private static void ViewWillLayoutSubviewsCallbackStatic(IntPtr context)
    {
        if (Listeners.TryGetValue(context, out var listenerInstance))
        {
            listenerInstance.OnViewWillLayoutSubviewsCallbacks();
        }
    }

    [MonoPInvokeCallback(typeof(ViewDidLayoutSubviewsCallback))]
    private static void ViewDidLayoutSubviewsCallbackStatic(IntPtr context)
    {
        if (Listeners.TryGetValue(context, out var listenerInstance))
        {
            listenerInstance.OnViewDidLayoutSubviewsCallbacks();
        }
    }
}
```








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

