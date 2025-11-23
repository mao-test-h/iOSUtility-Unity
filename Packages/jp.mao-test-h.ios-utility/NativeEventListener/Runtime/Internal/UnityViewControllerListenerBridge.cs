#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Assertions;

namespace iOSUtility.NativeEventListener
{
    internal sealed class UnityViewControllerListenerBridge : IDisposable
    {
        private static readonly Dictionary<IntPtr, IUnityViewControllerListener> Listeners = new();
        private readonly IntPtr _ptr;
        private bool _disposed;

        public UnityViewControllerListenerBridge(IUnityViewControllerListener lifecycleListener)
        {
            var ptr = CreateUnityViewControllerListenerBridge(
                ViewWillLayoutSubviewsCallbackStatic,
                ViewDidLayoutSubviewsCallbackStatic,
                ViewWillDisappearCallbackStatic,
                ViewDidDisappearCallbackStatic,
                ViewWillAppearCallbackStatic,
                ViewDidAppearCallbackStatic,
                InterfaceWillChangeOrientationCallbackStatic,
                InterfaceDidChangeOrientationCallbackStatic);

            Assert.IsNotNull(lifecycleListener);
            Listeners[ptr] = lifecycleListener;
            UnityRegisterViewControllerListener(ptr);
            _ptr = ptr;
        }

        ~UnityViewControllerListenerBridge()
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
                UnityUnregisterViewControllerListener(_ptr);
                ReleaseUnityViewControllerListenerBridge(_ptr);
                _disposed = true;
            }
        }


        [DllImport("__Internal", EntryPoint = "UnityIOSPluginBaseBridge_CreateUnityViewControllerListenerBridge")]
        private static extern IntPtr CreateUnityViewControllerListenerBridge(
            ViewWillLayoutSubviewsCallback viewWillLayoutSubviewsCallback,
            ViewDidLayoutSubviewsCallback viewDidLayoutSubviewsCallback,
            ViewWillDisappearCallback viewWillDisappearCallback,
            ViewDidDisappearCallback viewDidDisappearCallback,
            ViewWillAppearCallback viewWillAppearCallback,
            ViewDidAppearCallback viewDidAppearCallback,
            InterfaceWillChangeOrientationCallback interfaceWillChangeOrientationCallback,
            InterfaceDidChangeOrientationCallback interfaceDidChangeOrientationCallback);

        [DllImport("__Internal", EntryPoint = "UnityIOSPluginBaseBridge_ReleaseUnityViewControllerListenerBridge")]
        private static extern void ReleaseUnityViewControllerListenerBridge(IntPtr ptr);

        [DllImport("__Internal", EntryPoint = "UnityIOSPluginBaseBridge_UnityRegisterViewControllerListener")]
        private static extern void UnityRegisterViewControllerListener(IntPtr ptr);

        [DllImport("__Internal", EntryPoint = "UnityIOSPluginBaseBridge_UnityUnregisterViewControllerListener")]
        private static extern void UnityUnregisterViewControllerListener(IntPtr ptr);


        private delegate void ViewWillLayoutSubviewsCallback(IntPtr context);

        private delegate void ViewDidLayoutSubviewsCallback(IntPtr context);

        private delegate void ViewWillDisappearCallback(IntPtr context, byte animated);

        private delegate void ViewDidDisappearCallback(IntPtr context, byte animated);

        private delegate void ViewWillAppearCallback(IntPtr context, byte animated);

        private delegate void ViewDidAppearCallback(IntPtr context, byte animated);

        private delegate void InterfaceWillChangeOrientationCallback(IntPtr context);

        private delegate void InterfaceDidChangeOrientationCallback(IntPtr context);

        [MonoPInvokeCallback(typeof(ViewWillLayoutSubviewsCallback))]
        private static void ViewWillLayoutSubviewsCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnViewWillLayoutSubviewsCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(ViewDidLayoutSubviewsCallback))]
        private static void ViewDidLayoutSubviewsCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnViewDidLayoutSubviewsCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(ViewWillDisappearCallback))]
        private static void ViewWillDisappearCallbackStatic(IntPtr context, byte animated)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnViewWillDisappearCallbacks(animated != 0);
            }
        }

        [MonoPInvokeCallback(typeof(ViewDidDisappearCallback))]
        private static void ViewDidDisappearCallbackStatic(IntPtr context, byte animated)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnViewDidDisappearCallbacks(animated != 0);
            }
        }

        [MonoPInvokeCallback(typeof(ViewWillAppearCallback))]
        private static void ViewWillAppearCallbackStatic(IntPtr context, byte animated)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnViewWillAppearCallbacks(animated != 0);
            }
        }

        [MonoPInvokeCallback(typeof(ViewDidAppearCallback))]
        private static void ViewDidAppearCallbackStatic(IntPtr context, byte animated)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnViewDidAppearCallbacks(animated != 0);
            }
        }

        [MonoPInvokeCallback(typeof(InterfaceWillChangeOrientationCallback))]
        private static void InterfaceWillChangeOrientationCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnInterfaceWillChangeOrientationCallbacks();
            }
        }

        [MonoPInvokeCallback(typeof(InterfaceDidChangeOrientationCallback))]
        private static void InterfaceDidChangeOrientationCallbackStatic(IntPtr context)
        {
            if (Listeners.TryGetValue(context, out var listenerInstance))
            {
                listenerInstance.OnInterfaceDidChangeOrientationCallbacks();
            }
        }
    }
}

#endif
