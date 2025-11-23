#include "PluginBase/LifeCycleListener.h"
#include <stdint.h>

// application lifecycle events
typedef void (*DidFinishLaunchingCallback)(void* context);
typedef void (*DidBecomeActiveCallback)(void* context);
typedef void (*WillResignActiveCallback)(void* context);
typedef void (*DidEnterBackgroundCallback)(void* context);
typedef void (*WillEnterForegroundCallback)(void* context);
typedef void (*WillTerminateCallback)(void* context);
typedef void (*UnityDidUnloadCallback)(void* context);
typedef void (*UnityDidQuitCallback)(void* context);

@interface LifeCycleListenerBridge : NSObject<LifeCycleListener>
@property (nonatomic, assign) DidFinishLaunchingCallback didFinishLaunchingCallback;
@property (nonatomic, assign) DidBecomeActiveCallback didBecomeActiveCallback;
@property (nonatomic, assign) WillResignActiveCallback willResignActiveCallback;
@property (nonatomic, assign) DidEnterBackgroundCallback didEnterBackgroundCallback;
@property (nonatomic, assign) WillEnterForegroundCallback willEnterForegroundCallback;
@property (nonatomic, assign) WillTerminateCallback willTerminateCallback;
@property (nonatomic, assign) UnityDidUnloadCallback unityDidUnloadCallback;
@property (nonatomic, assign) UnityDidQuitCallback unityDidQuitCallback;
@end

@implementation LifeCycleListenerBridge

- (void)didFinishLaunching:(NSNotification*)notification
{
    if (self.didFinishLaunchingCallback) {
        self.didFinishLaunchingCallback((__bridge void*)self);
    }
}

- (void)didBecomeActive:(NSNotification*)notification
{
    if (self.didBecomeActiveCallback) {
        self.didBecomeActiveCallback((__bridge void*)self);
    }
}

- (void)willResignActive:(NSNotification*)notification
{
    if (self.willResignActiveCallback) {
        self.willResignActiveCallback((__bridge void*)self);
    }
}

- (void)didEnterBackground:(NSNotification*)notification
{
    if (self.didEnterBackgroundCallback) {
        self.didEnterBackgroundCallback((__bridge void*)self);
    }
}

- (void)willEnterForeground:(NSNotification*)notification
{
    if (self.willEnterForegroundCallback) {
        self.willEnterForegroundCallback((__bridge void*)self);
    }
}

- (void)willTerminate:(NSNotification*)notification
{
    if (self.willTerminateCallback) {
        self.willTerminateCallback((__bridge void*)self);
    }
}

- (void)unityDidUnload:(NSNotification*)notification
{
    if (self.unityDidUnloadCallback) {
        self.unityDidUnloadCallback((__bridge void*)self);
    }
}

- (void)unityDidQuit:(NSNotification*)notification
{
    if (self.unityDidQuitCallback) {
        self.unityDidQuitCallback((__bridge void*)self);
    }
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void* UnityIOSPluginBaseBridge_CreateLifeCycleListenerBridge(
                                                               DidFinishLaunchingCallback didFinishLaunchingCallback,
                                                               DidBecomeActiveCallback didBecomeActiveCallback,
                                                               WillResignActiveCallback willResignActiveCallback,
                                                               DidEnterBackgroundCallback didEnterBackgroundCallback,
                                                               WillEnterForegroundCallback willEnterForegroundCallback,
                                                               WillTerminateCallback willTerminateCallback,
                                                               UnityDidUnloadCallback unityDidUnloadCallback,
                                                               UnityDidQuitCallback unityDidQuitCallback)
{
    LifeCycleListenerBridge* bridge = [[LifeCycleListenerBridge alloc] init];
    bridge.didFinishLaunchingCallback = didFinishLaunchingCallback;
    bridge.didBecomeActiveCallback = didBecomeActiveCallback;
    bridge.willResignActiveCallback = willResignActiveCallback;
    bridge.didEnterBackgroundCallback = didEnterBackgroundCallback;
    bridge.willEnterForegroundCallback = willEnterForegroundCallback;
    bridge.willTerminateCallback = willTerminateCallback;
    bridge.unityDidUnloadCallback = unityDidUnloadCallback;
    bridge.unityDidQuitCallback = unityDidQuitCallback;
    return (__bridge_retained void*)bridge;
}

void UnityIOSPluginBaseBridge_ReleaseLifeCycleListenerBridge(void* ptr)
{
    LifeCycleListenerBridge* bridge = (__bridge_transfer LifeCycleListenerBridge*)ptr;
    bridge.didFinishLaunchingCallback = nil;
    bridge.didBecomeActiveCallback = nil;
    bridge.willResignActiveCallback = nil;
    bridge.didEnterBackgroundCallback = nil;
    bridge.willEnterForegroundCallback = nil;
    bridge.willTerminateCallback = nil;
    bridge.unityDidUnloadCallback = nil;
    bridge.unityDidQuitCallback = nil;
}

void UnityIOSPluginBaseBridge_UnityRegisterLifeCycleListener(void* ptr)
{
    LifeCycleListenerBridge* bridge = (__bridge LifeCycleListenerBridge*)ptr;
    UnityRegisterLifeCycleListener(bridge);
}

void UnityIOSPluginBaseBridge_UnityUnregisterLifeCycleListener(void* ptr)
{
    LifeCycleListenerBridge* bridge = (__bridge LifeCycleListenerBridge*)ptr;
    UnityUnregisterLifeCycleListener(bridge);
}

#ifdef __cplusplus
}
#endif
