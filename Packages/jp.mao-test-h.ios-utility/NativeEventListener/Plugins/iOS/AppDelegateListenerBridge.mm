#include "PluginBase/AppDelegateListener.h"
#include <stdint.h>

// AppDelegate events
typedef void (*OnOpenURLCallback)(void* context, const char* url, const char* sourceApplication);
typedef void (*ApplicationWillFinishLaunchingCallback)(void* context);
typedef void (*OnHandleEventsForBackgroundURLSessionCallback)(void* context, const char* identifier);
typedef void (*ApplicationDidReceiveMemoryWarningCallback)(void* context);
typedef void (*ApplicationSignificantTimeChangeCallback)(void* context);
typedef void (*ApplicationWillChangeStatusBarFrameCallback)(void* context, float frameX, float frameY, float frameWidth, float frameHeight);
typedef void (*ApplicationWillChangeStatusBarOrientationCallback)(void* context, int orientation);

@interface AppDelegateListenerBridge : NSObject<AppDelegateListener>
@property (nonatomic, assign) OnOpenURLCallback onOpenURLCallback;
@property (nonatomic, assign) ApplicationWillFinishLaunchingCallback applicationWillFinishLaunchingCallback;
@property (nonatomic, assign) OnHandleEventsForBackgroundURLSessionCallback onHandleEventsForBackgroundURLSessionCallback;
@property (nonatomic, assign) ApplicationDidReceiveMemoryWarningCallback applicationDidReceiveMemoryWarningCallback;
@property (nonatomic, assign) ApplicationSignificantTimeChangeCallback applicationSignificantTimeChangeCallback;
@property (nonatomic, assign) ApplicationWillChangeStatusBarFrameCallback applicationWillChangeStatusBarFrameCallback;
@property (nonatomic, assign) ApplicationWillChangeStatusBarOrientationCallback applicationWillChangeStatusBarOrientationCallback;
@end

@implementation AppDelegateListenerBridge

- (void)onOpenURL:(NSNotification*)notification
{
    if (self.onOpenURLCallback) {
        NSDictionary* userInfo = notification.userInfo;
        NSURL* url = userInfo[@"url"];
        NSString* sourceApplication = userInfo[@"sourceApplication"];
        
        const char* urlStr = url ? [[url absoluteString] UTF8String] : "";
        const char* sourceAppStr = sourceApplication ? [sourceApplication UTF8String] : "";
        
        self.onOpenURLCallback((__bridge void*)self, urlStr, sourceAppStr);
    }
}

- (void)applicationWillFinishLaunchingWithOptions:(NSNotification*)notification
{
    if (self.applicationWillFinishLaunchingCallback) {
        self.applicationWillFinishLaunchingCallback((__bridge void*)self);
    }
}

- (void)onHandleEventsForBackgroundURLSession:(NSNotification*)notification
{
    if (self.onHandleEventsForBackgroundURLSessionCallback) {
        NSDictionary* userInfo = notification.userInfo;
        NSString* identifier = [[userInfo allKeys] firstObject];
        
        const char* identifierStr = identifier ? [identifier UTF8String] : "";
        
        self.onHandleEventsForBackgroundURLSessionCallback((__bridge void*)self, identifierStr);
    }
}

- (void)applicationDidReceiveMemoryWarning:(NSNotification*)notification
{
    if (self.applicationDidReceiveMemoryWarningCallback) {
        self.applicationDidReceiveMemoryWarningCallback((__bridge void*)self);
    }
}

- (void)applicationSignificantTimeChange:(NSNotification*)notification
{
    if (self.applicationSignificantTimeChangeCallback) {
        self.applicationSignificantTimeChangeCallback((__bridge void*)self);
    }
}

- (void)applicationWillChangeStatusBarFrame:(NSNotification*)notification
{
    if (self.applicationWillChangeStatusBarFrameCallback) {
        NSDictionary* userInfo = notification.userInfo;
        NSValue* frameValue = userInfo[UIApplicationStatusBarFrameUserInfoKey];
        CGRect frame = [frameValue CGRectValue];
        
        self.applicationWillChangeStatusBarFrameCallback((__bridge void*)self,
                                                         (float)frame.origin.x,
                                                         (float)frame.origin.y,
                                                         (float)frame.size.width,
                                                         (float)frame.size.height);
    }
}

- (void)applicationWillChangeStatusBarOrientation:(NSNotification*)notification
{
    if (self.applicationWillChangeStatusBarOrientationCallback) {
        NSDictionary* userInfo = notification.userInfo;
        NSNumber* orientationValue = userInfo[UIApplicationStatusBarOrientationUserInfoKey];
        int orientation = [orientationValue intValue];
        
        self.applicationWillChangeStatusBarOrientationCallback((__bridge void*)self, orientation);
    }
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void* iOSUtility_NativeEventListener_CreateAppDelegateListenerBridge(
                                                                     OnOpenURLCallback onOpenURLCallback,
                                                                     ApplicationWillFinishLaunchingCallback applicationWillFinishLaunchingCallback,
                                                                     OnHandleEventsForBackgroundURLSessionCallback onHandleEventsForBackgroundURLSessionCallback,
                                                                     ApplicationDidReceiveMemoryWarningCallback applicationDidReceiveMemoryWarningCallback,
                                                                     ApplicationSignificantTimeChangeCallback applicationSignificantTimeChangeCallback,
                                                                     ApplicationWillChangeStatusBarFrameCallback applicationWillChangeStatusBarFrameCallback,
                                                                     ApplicationWillChangeStatusBarOrientationCallback applicationWillChangeStatusBarOrientationCallback)
{
    AppDelegateListenerBridge* bridge = [[AppDelegateListenerBridge alloc] init];
    bridge.onOpenURLCallback = onOpenURLCallback;
    bridge.applicationWillFinishLaunchingCallback = applicationWillFinishLaunchingCallback;
    bridge.onHandleEventsForBackgroundURLSessionCallback = onHandleEventsForBackgroundURLSessionCallback;
    bridge.applicationDidReceiveMemoryWarningCallback = applicationDidReceiveMemoryWarningCallback;
    bridge.applicationSignificantTimeChangeCallback = applicationSignificantTimeChangeCallback;
    bridge.applicationWillChangeStatusBarFrameCallback = applicationWillChangeStatusBarFrameCallback;
    bridge.applicationWillChangeStatusBarOrientationCallback = applicationWillChangeStatusBarOrientationCallback;
    return (__bridge_retained void*)bridge;
}

void iOSUtility_NativeEventListener_ReleaseAppDelegateListenerBridge(void* ptr)
{
    AppDelegateListenerBridge* bridge = (__bridge_transfer AppDelegateListenerBridge*)ptr;
    bridge.onOpenURLCallback = nil;
    bridge.applicationWillFinishLaunchingCallback = nil;
    bridge.onHandleEventsForBackgroundURLSessionCallback = nil;
    bridge.applicationDidReceiveMemoryWarningCallback = nil;
    bridge.applicationSignificantTimeChangeCallback = nil;
    bridge.applicationWillChangeStatusBarFrameCallback = nil;
    bridge.applicationWillChangeStatusBarOrientationCallback = nil;
}

void iOSUtility_NativeEventListener_UnityRegisterAppDelegateListener(void* ptr)
{
    AppDelegateListenerBridge* bridge = (__bridge AppDelegateListenerBridge*)ptr;
    UnityRegisterAppDelegateListener(bridge);
}

void iOSUtility_NativeEventListener_UnityUnregisterAppDelegateListener(void* ptr)
{
    AppDelegateListenerBridge* bridge = (__bridge AppDelegateListenerBridge*)ptr;
    UnityUnregisterAppDelegateListener(bridge);
}

#ifdef __cplusplus
}
#endif
