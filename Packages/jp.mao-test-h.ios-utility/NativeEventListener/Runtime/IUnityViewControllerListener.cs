namespace iOSUtility.NativeEventListener
{
    /// <summary>
    /// Unity の UIViewController のライフサイクルイベントを受け取るためのリスナーインターフェース
    /// </summary>
    /// <remarks>
    /// 参考: <see href="https://developer.apple.com/documentation/UIKit/UIViewController">UIViewController</see>
    /// </remarks>
    public interface IUnityViewControllerListener
    {
        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwilllayoutsubviews()">`UIViewController.viewWillLayoutSubviews()`</see>
        /// </summary>
        void OnViewWillLayoutSubviewsCallbacks();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdidlayoutsubviews()">`UIViewController.viewDidLayoutSubviews()`</see>
        /// </summary>
        void OnViewDidLayoutSubviewsCallbacks();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwilldisappear(_:)">`UIViewController.viewWillDisappear(_:)`</see>
        /// </summary>
        void OnViewWillDisappearCallbacks(bool animated);

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdiddisappear(_:)">`UIViewController.viewDidDisappear(_:)`</see>
        /// </summary>
        void OnViewDidDisappearCallbacks(bool animated);

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiviewcontroller/viewwillappear(_:)">`UIViewController.viewWillAppear(_:)`</see>
        /// </summary>
        void OnViewWillAppearCallbacks(bool animated);

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiviewcontroller/viewdidappear(_:)">`UIViewController.viewDidAppear(_:)`</see>
        /// </summary>
        void OnViewDidAppearCallbacks(bool animated);
    }
}
