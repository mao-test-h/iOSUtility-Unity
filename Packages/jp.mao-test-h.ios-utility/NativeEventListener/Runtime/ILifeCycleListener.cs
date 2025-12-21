namespace iOSUtility.NativeEventListener
{
    /// <summary>
    /// アプリケーションのライフサイクルイベントと Unity のライフサイクルイベントを受け取るためのリスナーインターフェース
    /// </summary>
    public interface ILifeCycleListener
    {
        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/didfinishlaunchingnotification?language=objc">`UIApplicationDidFinishLaunchingNotification`</see>
        /// </summary>
        void OnDidFinishLaunchingCallbacks();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/didbecomeactivenotification?language=objc">`UIApplicationDidBecomeActiveNotification`</see>
        /// </summary>
        void OnDidBecomeActiveCallbacks();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/willresignactivenotification?language=objc">`UIApplicationWillResignActiveNotification`</see>
        /// </summary>
        void OnWillResignActiveCallbacks();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/didenterbackgroundnotification?language=objc">`UIApplicationDidEnterBackgroundNotification`</see>
        /// </summary>
        void OnDidEnterBackgroundCallbacks();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/willenterforegroundnotification?language=objc">`UIApplicationWillEnterForegroundNotification`</see>
        /// </summary>
        void OnWillEnterForegroundCallbacks();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/willterminatenotification?language=objc">`UIApplicationWillTerminateNotification`</see>
        /// </summary>
        void OnWillTerminateCallbacks();

        /// <summary>
        /// Unity がアンロードされた直後に呼ばれます
        /// </summary>
        void OnUnityDidUnloadCallbacks();

        /// <summary>
        /// Unity が終了した直後に呼ばれます
        /// </summary>
        void OnUnityDidQuitCallbacks();
    }
}
