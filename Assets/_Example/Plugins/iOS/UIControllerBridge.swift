import UIKit

@_cdecl("Example_NativeUIController_CreateView")
public func Example_NativeUIController_CreateView() -> UnsafeMutableRawPointer {
    let screenBounds = UIScreen.main.bounds
    let viewWidth: CGFloat = 200.0
    let viewHeight: CGFloat = 200.0
    let centerX = screenBounds.midX - (viewWidth / 2.0)
    let centerY = screenBounds.midY - (viewHeight / 2.0)
    
    let view = UIView()
    view.backgroundColor = UIColor.cyan.withAlphaComponent(0.5)
    view.frame = CGRect(x: centerX, y: centerY, width: viewWidth, height: viewHeight)
    
    return Unmanaged.passRetained(view).toOpaque()
}

@_cdecl("Example_NativeUIController_DeleteView")
public func Example_NativeUIController_DeleteView(_ viewPtr: UnsafeMutableRawPointer) {
    Unmanaged<UIView>.fromOpaque(viewPtr).release()
}

@_cdecl("Example_NativeUIController_AddSubview")
public func Example_NativeUIController_AddSubview(_ viewPtr: UnsafeMutableRawPointer) {
    guard let rootViewController = UnityFramework.getInstance().appController().rootViewController else {
        print("Example_NativeUIController_AddSubview: RootViewController not found")
        return
    }
    
    let view = Unmanaged<UIView>.fromOpaque(viewPtr).takeUnretainedValue()
    if view.superview == rootViewController.view {
        print("Example_NativeUIController_AddSubview: View is already a subview")
        return
    }
    
    rootViewController.view.addSubview(view)
}

@_cdecl("Example_NativeUIController_RemoveSubview")
public func Example_NativeUIController_RemoveSubview(_ viewPtr: UnsafeMutableRawPointer) {
    let view = Unmanaged<UIView>.fromOpaque(viewPtr).takeUnretainedValue()
    guard view.superview != nil else {
        print("Example_NativeUIController_RemoveSubview: View is not in any hierarchy")
        return
    }
    
    view.removeFromSuperview()
}
