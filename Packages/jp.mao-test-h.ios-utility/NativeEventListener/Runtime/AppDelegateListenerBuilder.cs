using System;

namespace iOSUtility.NativeEventListener
{
    public static class AppDelegateListenerBuilder
    {
        public static IDisposable Build(IAppDelegateListener listener)
        {
#if UNITY_IOS && !UNITY_EDITOR
            var instance = new AppDelegateListenerBridge(listener);
#else
            var instance = new DummyBridge();
#endif
            return instance;
        }
    }
}
