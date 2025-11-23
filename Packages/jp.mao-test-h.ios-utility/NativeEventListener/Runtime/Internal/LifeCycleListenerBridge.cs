#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Assertions;

namespace iOSUtility.NativeEventListener
{
    internal sealed class LifeCycleListenerBridge : IDisposable
    {
        private static readonly Dictionary<IntPtr, ILifeCycleListener> Listeners = new();
        private readonly IntPtr _ptr;
        private bool _disposed;

        public LifeCycleListenerBridge(ILifeCycleListener lifecycleListener)
        {
            var ptr = CreateLifeCycleListenerBridge(
                DidFinishLaunchingCallbackStatic,
                DidBecomeActiveCallbackStatic,
                WillResignActiveCallbackStatic,
                DidEnterBackgroundCallbackStatic,
                WillEnterForegroundCallbackStatic,
                WillTerminateCallbackStatic,
                UnityDidUnloadCallbackStatic,
                UnityDidQuitCallbackStatic);

            Assert.IsNotNull(lifecycleListener);
            Listeners[ptr] = lifecycleListener;
            UnityRegisterLifeCycleListener(ptr);
            _ptr = ptr;
        }

        ~LifeCycleListenerBridge()
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
                UnityUnregisterLifeCycleListener(_ptr);
                ReleaseLifeCycleListenerBridge(_ptr);
                _disposed = true;
            }
        }


        [DllImport("__Internal", EntryPoint = "UnityIOSPluginBaseBridge_CreateLifeCycleListenerBridge")]
        private static extern IntPtr CreateLifeCycleListenerBridge(
            DidFinishLaunchingCallback didFinishLaunchingCallback,
            DidBecomeActiveCallback didBecomeActiveCallback,
            WillResignActiveCallback willResignActiveCallback,
            DidEnterBackgroundCallback didEnterBackgroundCallback,
            WillEnterForegroundCallback willEnterForegroundCallback,
            WillTerminateCallback willTerminateCallback,
            UnityDidUnloadCallback unityDidUnloadCallback,
            UnityDidQuitCallback unityDidQuitCallback);

        [DllImport("__Internal", EntryPoint = "UnityIOSPluginBaseBridge_ReleaseLifeCycleListenerBridge")]
        private static extern void ReleaseLifeCycleListenerBridge(IntPtr ptr);

        [DllImport("__Internal", EntryPoint = "UnityIOSPluginBaseBridge_UnityRegisterLifeCycleListener")]
        private static extern void UnityRegisterLifeCycleListener(IntPtr ptr);

        [DllImport("__Internal", EntryPoint = "UnityIOSPluginBaseBridge_UnityUnregisterLifeCycleListener")]
        private static extern void UnityUnregisterLifeCycleListener(IntPtr ptr);


        private delegate void DidFinishLaunchingCallback(IntPtr context);

        private delegate void DidBecomeActiveCallback(IntPtr context);

        private delegate void WillResignActiveCallback(IntPtr context);

        private delegate void DidEnterBackgroundCallback(IntPtr context);

        private delegate void WillEnterForegroundCallback(IntPtr context);

        private delegate void WillTerminateCallback(IntPtr context);

        private delegate void UnityDidUnloadCallback(IntPtr context);

        private delegate void UnityDidQuitCallback(IntPtr context);

        [MonoPInvokeCallback(typeof(DidFinishLaunchingCallback))]
        private static void DidFinishLaunchingCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnDidFinishLaunchingCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(DidBecomeActiveCallback))]
        private static void DidBecomeActiveCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnDidBecomeActiveCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(WillResignActiveCallback))]
        private static void WillResignActiveCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnWillResignActiveCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(DidEnterBackgroundCallback))]
        private static void DidEnterBackgroundCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnDidEnterBackgroundCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(WillEnterForegroundCallback))]
        private static void WillEnterForegroundCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnWillEnterForegroundCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(WillTerminateCallback))]
        private static void WillTerminateCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnWillTerminateCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(UnityDidUnloadCallback))]
        private static void UnityDidUnloadCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnUnityDidUnloadCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(UnityDidQuitCallback))]
        private static void UnityDidQuitCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnUnityDidQuitCallbacks();
            }
        }
    }
}

#endif
