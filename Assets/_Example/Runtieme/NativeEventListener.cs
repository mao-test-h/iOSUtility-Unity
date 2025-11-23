using System;
using iOSUtility.NativeEventListener;
using UnityEngine;

namespace _Example
{
    public sealed class NativeEventListener : MonoBehaviour
    {
        private static string Tag => $"[{nameof(NativeEventListener)}]";
        private IDisposable _unityViewControllerListenerBridge;
        private IDisposable _lifeCycleListenerBridge;

        private void Start()
        {
            _unityViewControllerListenerBridge = UnityViewControllerListenerBuilder
                .Build(new UnityViewControllerListener());

            _lifeCycleListenerBridge = LifeCycleListenerBuilder
                .Build(new LifeCycleListener());
        }

        private void OnDestroy()
        {
            _unityViewControllerListenerBridge?.Dispose();
            _lifeCycleListenerBridge?.Dispose();
        }

        private sealed class UnityViewControllerListener : IUnityViewControllerListener
        {
            public void OnViewWillLayoutSubviewsCallbacks()
            {
                Debug.Log($"{Tag} [IUnityViewControllerListener] OnViewWillLayoutSubviews");
            }

            public void OnViewDidLayoutSubviewsCallbacks()
            {
                Debug.Log($"{Tag} [IUnityViewControllerListener] OnViewDidLayoutSubviews");
            }

            public void OnViewWillDisappearCallbacks(bool animated)
            {
                Debug.Log($"{Tag} [IUnityViewControllerListener] OnViewWillDisappear (animated: {animated})");
            }

            public void OnViewDidDisappearCallbacks(bool animated)
            {
                Debug.Log($"{Tag} [IUnityViewControllerListener] OnViewDidDisappear (animated: {animated})");
            }

            public void OnViewWillAppearCallbacks(bool animated)
            {
                Debug.Log($"{Tag} [IUnityViewControllerListener] OnViewWillAppear (animated: {animated})");
            }

            public void OnViewDidAppearCallbacks(bool animated)
            {
                Debug.Log($"{Tag} [IUnityViewControllerListener] OnViewDidAppear (animated: {animated})");
            }

            public void OnInterfaceWillChangeOrientationCallbacks()
            {
                Debug.Log($"{Tag} [IUnityViewControllerListener] OnInterfaceWillChangeOrientation");
            }

            public void OnInterfaceDidChangeOrientationCallbacks()
            {
                Debug.Log($"{Tag} [IUnityViewControllerListener] OnInterfaceDidChangeOrientation");
            }
        }

        private sealed class LifeCycleListener : ILifeCycleListener
        {
            public void OnDidFinishLaunchingCallbacks()
            {
                Debug.Log($"{Tag} [ILifeCycleListener] OnDidFinishLaunching");
            }

            public void OnDidBecomeActiveCallbacks()
            {
                Debug.Log($"{Tag} [ILifeCycleListener] OnDidBecomeActive");
            }

            public void OnWillResignActiveCallbacks()
            {
                Debug.Log($"{Tag} [ILifeCycleListener] OnWillResignActive");
            }

            public void OnDidEnterBackgroundCallbacks()
            {
                Debug.Log($"{Tag} [ILifeCycleListener] OnDidEnterBackground");
            }

            public void OnWillEnterForegroundCallbacks()
            {
                Debug.Log($"{Tag} [ILifeCycleListener] OnWillEnterForeground");
            }

            public void OnWillTerminateCallbacks()
            {
                Debug.Log($"{Tag} [ILifeCycleListener] OnWillTerminate");
            }

            public void OnUnityDidUnloadCallbacks()
            {
                Debug.Log($"{Tag} [ILifeCycleListener] OnUnityDidUnload");
            }

            public void OnUnityDidQuitCallbacks()
            {
                Debug.Log($"{Tag} [ILifeCycleListener] OnUnityDidQuit");
            }
        }
    }
}
