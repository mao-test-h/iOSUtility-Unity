namespace iOSUtility.NativeEventListener
{
    public interface IAppDelegateListener
    {
        void OnOpenURL(string url, string sourceApplication);
        void OnApplicationWillFinishLaunchingWithOptions();
        void OnHandleEventsForBackgroundURLSession(string identifier);
        void OnApplicationDidReceiveMemoryWarning();
        void OnApplicationSignificantTimeChange();
    }
}
