#include "PluginBase/UnityViewControllerListener.h"
#include <stdint.h>

// view changes on the main view controller
typedef void (*ViewWillLayoutSubviewsCallback)(void* context);
typedef void (*ViewDidLayoutSubviewsCallback)(void* context);
typedef void (*ViewWillDisappearCallback)(void* context, uint8_t animated);
typedef void (*ViewDidDisappearCallback)(void* context, uint8_t animated);
typedef void (*ViewWillAppearCallback)(void* context, uint8_t animated);
typedef void (*ViewDidAppearCallback)(void* context, uint8_t animated);
typedef void (*InterfaceWillChangeOrientationCallback)(void* context);
typedef void (*InterfaceDidChangeOrientationCallback)(void* context);

@interface UnityViewControllerListenerBridge : NSObject<UnityViewControllerListener>
@property (nonatomic, assign) ViewWillLayoutSubviewsCallback viewWillLayoutSubviewsCallback;
@property (nonatomic, assign) ViewDidLayoutSubviewsCallback viewDidLayoutSubviewsCallback;
@property (nonatomic, assign) ViewWillDisappearCallback viewWillDisappearCallback;
@property (nonatomic, assign) ViewDidDisappearCallback viewDidDisappearCallback;
@property (nonatomic, assign) ViewWillAppearCallback viewWillAppearCallback;
@property (nonatomic, assign) ViewDidAppearCallback viewDidAppearCallback;
@property (nonatomic, assign) InterfaceWillChangeOrientationCallback interfaceWillChangeOrientationCallback;
@property (nonatomic, assign) InterfaceDidChangeOrientationCallback interfaceDidChangeOrientationCallback;
@end

@implementation UnityViewControllerListenerBridge

- (void)viewWillLayoutSubviews:(NSNotification*)notification
{
    if (self.viewWillLayoutSubviewsCallback) {
        self.viewWillLayoutSubviewsCallback((__bridge void*)self);
    }
}

- (void)viewDidLayoutSubviews:(NSNotification*)notification
{
    if (self.viewDidLayoutSubviewsCallback) {
        self.viewDidLayoutSubviewsCallback((__bridge void*)self);
    }
}

- (void)viewWillDisappear:(NSNotification*)notification
{
    if (self.viewWillDisappearCallback) {
        NSNumber* animatedNumber = notification.userInfo[@"animated"];
        uint8_t animated = animatedNumber ? [animatedNumber boolValue] : 0;
        self.viewWillDisappearCallback((__bridge void*)self, animated);
    }
}

- (void)viewDidDisappear:(NSNotification*)notification
{
    if (self.viewDidDisappearCallback) {
        NSNumber* animatedNumber = notification.userInfo[@"animated"];
        uint8_t animated = animatedNumber ? [animatedNumber boolValue] : 0;
        self.viewDidDisappearCallback((__bridge void*)self, animated);
    }
}

- (void)viewWillAppear:(NSNotification*)notification
{
    if (self.viewWillAppearCallback) {
        NSNumber* animatedNumber = notification.userInfo[@"animated"];
        uint8_t animated = animatedNumber ? [animatedNumber boolValue] : 0;
        self.viewWillAppearCallback((__bridge void*)self, animated);
    }
}

- (void)viewDidAppear:(NSNotification*)notification
{
    if (self.viewDidAppearCallback) {
        NSNumber* animatedNumber = notification.userInfo[@"animated"];
        uint8_t animated = animatedNumber ? [animatedNumber boolValue] : 0;
        self.viewDidAppearCallback((__bridge void*)self, animated);
    }
}

- (void)interfaceWillChangeOrientation:(NSNotification*)notification
{
    if (self.interfaceWillChangeOrientationCallback) {
        self.interfaceWillChangeOrientationCallback((__bridge void*)self);
    }
}

- (void)interfaceDidChangeOrientation:(NSNotification*)notification
{
    if (self.interfaceDidChangeOrientationCallback) {
        self.interfaceDidChangeOrientationCallback((__bridge void*)self);
    }
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void* iOSUtility_NativeEventListener_CreateUnityViewControllerListenerBridge(
                                                                 ViewWillLayoutSubviewsCallback viewWillLayoutSubviewsCallback,
                                                                 ViewDidLayoutSubviewsCallback viewDidLayoutSubviewsCallback,
                                                                 ViewWillDisappearCallback viewWillDisappearCallback,
                                                                 ViewDidDisappearCallback viewDidDisappearCallback,
                                                                 ViewWillAppearCallback viewWillAppearCallback,
                                                                 ViewDidAppearCallback viewDidAppearCallback,
                                                                 InterfaceWillChangeOrientationCallback interfaceWillChangeOrientationCallback,
                                                                 InterfaceDidChangeOrientationCallback interfaceDidChangeOrientationCallback)
{
    UnityViewControllerListenerBridge* bridge = [[UnityViewControllerListenerBridge alloc] init];
    bridge.viewWillLayoutSubviewsCallback = viewWillLayoutSubviewsCallback;
    bridge.viewDidLayoutSubviewsCallback = viewDidLayoutSubviewsCallback;
    bridge.viewWillDisappearCallback = viewWillDisappearCallback;
    bridge.viewDidDisappearCallback = viewDidDisappearCallback;
    bridge.viewWillAppearCallback = viewWillAppearCallback;
    bridge.viewDidAppearCallback = viewDidAppearCallback;
    bridge.interfaceWillChangeOrientationCallback = interfaceWillChangeOrientationCallback;
    bridge.interfaceDidChangeOrientationCallback = interfaceDidChangeOrientationCallback;
    return (__bridge_retained void*)bridge;
}

void iOSUtility_NativeEventListener_ReleaseUnityViewControllerListenerBridge(void* ptr)
{
    UnityViewControllerListenerBridge* bridge = (__bridge_transfer UnityViewControllerListenerBridge*)ptr;
    bridge.viewWillLayoutSubviewsCallback = nil;
    bridge.viewDidLayoutSubviewsCallback = nil;
    bridge.viewWillDisappearCallback = nil;
    bridge.viewDidDisappearCallback = nil;
    bridge.viewWillAppearCallback = nil;
    bridge.viewDidAppearCallback = nil;
    bridge.interfaceWillChangeOrientationCallback = nil;
    bridge.interfaceDidChangeOrientationCallback = nil;
}

void iOSUtility_NativeEventListener_UnityRegisterViewControllerListener(void* ptr)
{
    UnityViewControllerListenerBridge* bridge = (__bridge UnityViewControllerListenerBridge*)ptr;
    UnityRegisterViewControllerListener(bridge);
}

void iOSUtility_NativeEventListener_UnityUnregisterViewControllerListener(void* ptr)
{
    UnityViewControllerListenerBridge* bridge = (__bridge UnityViewControllerListenerBridge*)ptr;
    UnityUnregisterViewControllerListener(bridge);
}

#ifdef __cplusplus
}
#endif
