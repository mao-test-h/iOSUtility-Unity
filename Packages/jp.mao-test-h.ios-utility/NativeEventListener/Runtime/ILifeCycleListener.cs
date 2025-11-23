namespace iOSUtility.NativeEventListener
{
    public interface ILifeCycleListener
    {
        void OnDidFinishLaunchingCallbacks();
        void OnDidBecomeActiveCallbacks();
        void OnWillResignActiveCallbacks();
        void OnDidEnterBackgroundCallbacks();
        void OnWillEnterForegroundCallbacks();
        void OnWillTerminateCallbacks();
        void OnUnityDidUnloadCallbacks();
        void OnUnityDidQuitCallbacks();
    }
}
