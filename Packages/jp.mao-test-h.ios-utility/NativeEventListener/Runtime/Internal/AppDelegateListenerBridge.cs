#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Assertions;

namespace iOSUtility.NativeEventListener
{
    internal sealed class AppDelegateListenerBridge : IDisposable
    {
        private static readonly Dictionary<IntPtr, IAppDelegateListener> Listeners = new();
        private readonly IntPtr _ptr;
        private bool _disposed;

        public AppDelegateListenerBridge(IAppDelegateListener appDelegateListener)
        {
            var ptr = CreateAppDelegateListenerBridge(
                OnOpenURLCallbackStatic,
                ApplicationWillFinishLaunchingCallbackStatic,
                OnHandleEventsForBackgroundURLSessionCallbackStatic,
                ApplicationDidReceiveMemoryWarningCallbackStatic,
                ApplicationSignificantTimeChangeCallbackStatic);

            Assert.IsNotNull(appDelegateListener);
            Listeners[ptr] = appDelegateListener;
            UnityRegisterAppDelegateListener(ptr);
            _ptr = ptr;
        }

        ~AppDelegateListenerBridge()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Assert.IsTrue(_ptr != IntPtr.Zero);

                if (disposing)
                {
                    // release managed resources
                }

                Listeners.Remove(_ptr);
                UnityUnregisterAppDelegateListener(_ptr);
                ReleaseAppDelegateListenerBridge(_ptr);
                _disposed = true;
            }
        }


        [DllImport("__Internal", EntryPoint = "iOSUtility_NativeEventListener_CreateAppDelegateListenerBridge")]
        private static extern IntPtr CreateAppDelegateListenerBridge(
            OnOpenURLCallback onOpenURLCallback,
            ApplicationWillFinishLaunchingCallback applicationWillFinishLaunchingCallback,
            OnHandleEventsForBackgroundURLSessionCallback onHandleEventsForBackgroundURLSessionCallback,
            ApplicationDidReceiveMemoryWarningCallback applicationDidReceiveMemoryWarningCallback,
            ApplicationSignificantTimeChangeCallback applicationSignificantTimeChangeCallback);

        [DllImport("__Internal", EntryPoint = "iOSUtility_NativeEventListener_ReleaseAppDelegateListenerBridge")]
        private static extern void ReleaseAppDelegateListenerBridge(IntPtr ptr);

        [DllImport("__Internal", EntryPoint = "iOSUtility_NativeEventListener_UnityRegisterAppDelegateListener")]
        private static extern void UnityRegisterAppDelegateListener(IntPtr ptr);

        [DllImport("__Internal", EntryPoint = "iOSUtility_NativeEventListener_UnityUnregisterAppDelegateListener")]
        private static extern void UnityUnregisterAppDelegateListener(IntPtr ptr);


        private delegate void OnOpenURLCallback(IntPtr context, string url, string sourceApplication);

        private delegate void ApplicationWillFinishLaunchingCallback(IntPtr context);

        private delegate void OnHandleEventsForBackgroundURLSessionCallback(IntPtr context, string identifier);

        private delegate void ApplicationDidReceiveMemoryWarningCallback(IntPtr context);

        private delegate void ApplicationSignificantTimeChangeCallback(IntPtr context);

        [MonoPInvokeCallback(typeof(OnOpenURLCallback))]
        private static void OnOpenURLCallbackStatic(IntPtr context, string url, string sourceApplication)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnOpenURL(url, sourceApplication);
            }
        }

        [MonoPInvokeCallback(typeof(ApplicationWillFinishLaunchingCallback))]
        private static void ApplicationWillFinishLaunchingCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnApplicationWillFinishLaunchingWithOptions();
            }
        }

        [MonoPInvokeCallback(typeof(OnHandleEventsForBackgroundURLSessionCallback))]
        private static void OnHandleEventsForBackgroundURLSessionCallbackStatic(IntPtr context, string identifier)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnHandleEventsForBackgroundURLSession(identifier);
            }
        }

        [MonoPInvokeCallback(typeof(ApplicationDidReceiveMemoryWarningCallback))]
        private static void ApplicationDidReceiveMemoryWarningCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnApplicationDidReceiveMemoryWarning();
            }
        }

        [MonoPInvokeCallback(typeof(ApplicationSignificantTimeChangeCallback))]
        private static void ApplicationSignificantTimeChangeCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnApplicationSignificantTimeChange();
            }
        }
    }
}

#endif
