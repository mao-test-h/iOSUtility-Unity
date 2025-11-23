namespace iOSUtility.NativeEventListener
{
    /// <summary>
    /// iOS アプリケーションのライフサイクルイベントと Unity のライフサイクルイベントを受け取るためのリスナーインターフェース。
    /// </summary>
    /// <remarks>
    /// 参考: <see href="https://developer.apple.com/documentation/uikit/uiapplicationdelegate">UIApplicationDelegate</see>
    /// </remarks>
    public interface ILifeCycleListener
    {
        /// <summary>
        /// アプリケーションの起動処理が完了した直後に呼ばれます。
        /// </summary>
        void OnDidFinishLaunchingCallbacks();

        /// <summary>
        /// アプリケーションがアクティブになった直後に呼ばれます。
        /// </summary>
        void OnDidBecomeActiveCallbacks();

        /// <summary>
        /// アプリケーションが非アクティブになる直前に呼ばれます。
        /// </summary>
        void OnWillResignActiveCallbacks();

        /// <summary>
        /// アプリケーションがバックグラウンドに移行した直後に呼ばれます。
        /// </summary>
        void OnDidEnterBackgroundCallbacks();

        /// <summary>
        /// アプリケーションがフォアグラウンドに復帰する直前に呼ばれます。
        /// </summary>
        void OnWillEnterForegroundCallbacks();

        /// <summary>
        /// アプリケーションが終了する直前に呼ばれます。
        /// </summary>
        void OnWillTerminateCallbacks();

        /// <summary>
        /// Unity がアンロードされた直後に呼ばれます。
        /// </summary>
        void OnUnityDidUnloadCallbacks();

        /// <summary>
        /// Unity が終了した直後に呼ばれます。
        /// </summary>
        void OnUnityDidQuitCallbacks();
    }
}
