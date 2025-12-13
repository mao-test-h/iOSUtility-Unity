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

        /// <summary>
        /// ステータスバーのフレームが変更される直前に呼ばれます。
        /// </summary>
        /// <param name="x">新しいステータスバーフレームの X 座標</param>
        /// <param name="y">新しいステータスバーフレームの Y 座標</param>
        /// <param name="width">新しいステータスバーフレームの幅</param>
        /// <param name="height">新しいステータスバーフレームの高さ</param>
        void OnApplicationWillChangeStatusBarFrame(float x, float y, float width, float height);

        /// <summary>
        /// ステータスバーの向きが変更される直前に呼ばれます。
        /// </summary>
        /// <param name="orientation">新しい画面の向き</param>
        void OnApplicationWillChangeStatusBarOrientation(UIInterfaceOrientation orientation);
    }

    /// <summary>
    /// iOS の UIInterfaceOrientation に対応する画面の向き。
    /// </summary>
    /// <remarks>
    /// 参考: <see href="https://developer.apple.com/documentation/uikit/uiinterfaceorientation">UIInterfaceOrientation</see>
    /// </remarks>
    public enum UIInterfaceOrientation
    {
        /// <summary>向きが不明</summary>
        Unknown = 0,

        /// <summary>縦向き (ホームボタンが下)</summary>
        Portrait = 1,

        /// <summary>縦向き逆さま (ホームボタンが上)</summary>
        PortraitUpsideDown = 2,

        /// <summary>横向き左 (ホームボタンが左)</summary>
        LandscapeLeft = 3,

        /// <summary>横向き右 (ホームボタンが右)</summary>
        LandscapeRight = 4
    }
}
