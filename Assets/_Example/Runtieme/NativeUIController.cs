using System;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

namespace _Example
{
    internal interface INativeUIController : IDisposable
    {
        void AddSubview();
        void RemoveSubview();
    }

    internal static class NativeUIControllerFactory
    {
        internal static INativeUIController CreateNativeUIController()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new NativeUIControllerIOS();
#else
            return new NativeUIControllerDummy();
#endif
        }
    }

#if UNITY_IOS
    internal sealed class NativeUIControllerIOS : INativeUIController
    {
        private IntPtr _viewPtr = Example_NativeUIController_CreateView();

        public void Dispose()
        {
            if (_viewPtr != IntPtr.Zero)
            {
                Example_NativeUIController_DeleteView(_viewPtr);
                _viewPtr = IntPtr.Zero;
            }
        }

        public void AddSubview()
        {
            Assert.IsTrue(_viewPtr != IntPtr.Zero, "View pointer is null");
            Example_NativeUIController_AddSubview(_viewPtr);
        }

        public void RemoveSubview()
        {
            Assert.IsTrue(_viewPtr != IntPtr.Zero, "View pointer is null");
            Example_NativeUIController_RemoveSubview(_viewPtr);
        }

        [DllImport("__Internal", EntryPoint = "Example_NativeUIController_CreateView")]
        private static extern IntPtr Example_NativeUIController_CreateView();

        [DllImport("__Internal", EntryPoint = "Example_NativeUIController_DeleteView")]
        private static extern void Example_NativeUIController_DeleteView(IntPtr viewPtr);

        [DllImport("__Internal", EntryPoint = "Example_NativeUIController_AddSubview")]
        private static extern void Example_NativeUIController_AddSubview(IntPtr viewPtr);

        [DllImport("__Internal", EntryPoint = "Example_NativeUIController_RemoveSubview")]
        private static extern void Example_NativeUIController_RemoveSubview(IntPtr viewPtr);
    }
#endif

    internal sealed class NativeUIControllerDummy : INativeUIController
    {
        public void AddSubview()
        {
            // do nothing
        }

        public void RemoveSubview()
        {
            // do nothing
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}
