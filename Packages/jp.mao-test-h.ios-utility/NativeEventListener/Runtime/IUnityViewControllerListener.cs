namespace iOSUtility.NativeEventListener
{
    /// <summary>
    /// Unity の UIViewController のライフサイクルイベントを受け取るためのリスナーインターフェース。
    /// </summary>
    /// <remarks>
    /// 参考: <see href="https://developer.apple.com/documentation/UIKit/UIViewController">UIViewController</see>
    /// </remarks>
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
        /// デバイスの画面方向が変更される直前に呼ばれます。
        /// </summary>
        void OnInterfaceWillChangeOrientationCallbacks();

        /// <summary>
        /// デバイスの画面方向が変更された直後に呼ばれます。
        /// </summary>
        void OnInterfaceDidChangeOrientationCallbacks();
    }
}
