namespace iOSUtility.NativeEventListener
{
    /// <summary>
    /// UIApplicationDelegate のイベントを受け取るためのリスナーインターフェース
    /// </summary>
    /// <remarks>
    /// 参考: <see href="https://developer.apple.com/documentation/uikit/uiapplicationdelegate">UIApplicationDelegate</see>
    /// TODO: ILifeCycleListener を継承するようにする
    /// </remarks>
    public interface IAppDelegateListener
    {
        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplicationdelegate/application(_:open:options:)?language=objc">`UIApplicationDelegate.application:openURL:options:`</see>
        /// </summary>
        void OnOpenURL(string url, string sourceApplication);

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplicationdelegate/application(_:willfinishlaunchingwithoptions:)?language=objc">`UIApplicationDelegate.application:willFinishLaunchingWithOptions:`</see>
        /// </summary>
        void OnApplicationWillFinishLaunchingWithOptions();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplicationdelegate/application(_:handleeventsforbackgroundurlsession:completionhandler:)?language=objc">`UIApplicationDelegate.application:handleEventsForBackgroundURLSession:completionHandler:`</see>
        /// </summary>
        void OnHandleEventsForBackgroundURLSession(string identifier);

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/didreceivememorywarningnotification?language=objc">`UIApplicationDidReceiveMemoryWarningNotification`</see>
        /// </summary>
        void OnApplicationDidReceiveMemoryWarning();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/significanttimechangenotification?language=objc">`UIApplicationSignificantTimeChangeNotification`</see>
        /// </summary>
        void OnApplicationSignificantTimeChange();

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/willchangestatusbarframenotification?language=objc">`UIApplicationWillChangeStatusBarFrameNotification`</see>
        /// </summary>
        void OnApplicationWillChangeStatusBarFrame(float x, float y, float width, float height);

        /// <summary>
        /// <see href="https://developer.apple.com/documentation/uikit/uiapplication/willchangestatusbarorientationnotification?language=objc">`UIApplicationWillChangeStatusBarOrientationNotification`</see>
        /// </summary>
        void OnApplicationWillChangeStatusBarOrientation(UIInterfaceOrientation orientation);
    }

    /// <summary>
    /// <see href="https://developer.apple.com/documentation/uikit/uiinterfaceorientation">UIInterfaceOrientation</see>
    /// </summary>
    public enum UIInterfaceOrientation
    {
        Unknown = 0,
        Portrait = 1,
        PortraitUpsideDown = 2,
        LandscapeLeft = 3,
        LandscapeRight = 4
    }
}
