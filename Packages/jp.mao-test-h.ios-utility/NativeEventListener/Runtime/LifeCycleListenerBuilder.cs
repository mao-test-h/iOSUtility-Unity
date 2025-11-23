using System;

namespace iOSUtility.NativeEventListener
{
    public static class LifeCycleListenerBuilder
    {
        public static IDisposable Build(ILifeCycleListener listener)
        {
#if UNITY_IOS && !UNITY_EDITOR
            var instance = new LifeCycleListenerBridge(listener);
#else
            var instance = new DummyBridge();
#endif
            return instance;
        }
    }
}
