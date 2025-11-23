namespace iOSUtility.NativeEventListener
{
    /// <summary>
    /// iOS の AppDelegate のイベントを受け取るためのリスナーインターフェース。
    /// </summary>
    /// <remarks>
    /// このインターフェースは Unity の PluginBase にある @protocol AppDelegateListener に対応しています。
    /// 各メソッドは iOS の UIApplicationDelegate のメソッドに対応する通知を受け取ります。
    /// </remarks>
    public interface IAppDelegateListener
    {
        /// <summary>
        /// 他のアプリケーションから URL を開く要求があった際に呼ばれます。
        /// URL スキームでアプリを起動された場合などに使用されます。
        /// </summary>
        /// <param name="url">開かれる URL</param>
        /// <param name="sourceApplication">URL を開く要求を送ったアプリケーションの Bundle ID</param>
        void OnOpenURL(string url, string sourceApplication);

        /// <summary>
        /// アプリケーションの起動処理が開始される直前に呼ばれます。
        /// OnDidFinishLaunchingCallbacks より前に呼ばれます。
        /// </summary>
        void OnApplicationWillFinishLaunchingWithOptions();

        /// <summary>
        /// バックグラウンド URL セッションのイベントを処理する必要がある際に呼ばれます。
        /// </summary>
        /// <param name="identifier">バックグラウンド URL セッションの識別子</param>
        void OnHandleEventsForBackgroundURLSession(string identifier);

        /// <summary>
        /// アプリケーションがメモリ不足の警告を受け取った際に呼ばれます。
        /// </summary>
        void OnApplicationDidReceiveMemoryWarning();

        /// <summary>
        /// 重要な時刻の変更があった際に呼ばれます。
        /// 例: 日付が変わった、タイムゾーンが変更された、サマータイムの切り替えなど。
        /// </summary>
        void OnApplicationSignificantTimeChange();
    }
}
