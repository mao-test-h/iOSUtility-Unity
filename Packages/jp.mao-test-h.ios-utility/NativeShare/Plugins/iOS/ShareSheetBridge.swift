import UIKit

@_cdecl("iOSUtility_NativeShare_ShareFile")
public func iOSUtility_NativeShare_ShareFile(filePath: UnsafePointer<CChar>, subject: UnsafePointer<CChar>, text: UnsafePointer<CChar>) {
    let filePathString = String(cString: filePath)
    let subjectString = String(cString: subject)
    let textString = String(cString: text)
    
    ShareSheetManager.shared.showShareSheet(filePath: filePathString, subject: subjectString, text: textString)
}

private class ShareSheetManager {
    fileprivate static let shared = ShareSheetManager()
    
    public func showShareSheet(filePath: String, subject: String, text: String) {
        DispatchQueue.main.async { [weak self] in
            self?.presentShareSheet(filePath: filePath, subject: subject, text: text)
        }
    }
    
    private func presentShareSheet(filePath: String, subject: String, text: String) {
        guard let rootViewController = UnityFramework.getInstance().appController().rootViewController else {
            print("ShareSheetManager: RootViewController not found")
            return
        }
        
        let fileURL = URL(fileURLWithPath: filePath)
        guard FileManager.default.fileExists(atPath: filePath) else {
            print("ShareSheetManager: File not found at path: \(filePath)")
            return
        }
        
        var activityItems: [Any] = []
        
        if !text.isEmpty {
            activityItems.append(text)
        }
        
        activityItems.append(fileURL)
        
        let activityViewController = UIActivityViewController(
            activityItems: activityItems,
            applicationActivities: nil
        )
        
        if !subject.isEmpty {
            activityViewController.setValue(subject, forKey: "subject")
        }
        
        // iPadでの表示対応（ポップオーバー）
        if let popoverController = activityViewController.popoverPresentationController {
            popoverController.sourceView = rootViewController.view
            popoverController.sourceRect = CGRect(
                x: rootViewController.view.bounds.midX,
                y: rootViewController.view.bounds.midY,
                width: 0,
                height: 0
            )
            popoverController.permittedArrowDirections = []
        }
        
        activityViewController.completionWithItemsHandler = { activityType, completed, returnedItems, error in
            if let error = error {
                print("ShareSheetManager: Share failed with error: \(error.localizedDescription)")
            } else if completed {
                print("ShareSheetManager: Share completed successfully")
            } else {
                print("ShareSheetManager: Share cancelled")
            }
        }
        
        rootViewController.present(activityViewController, animated: true, completion: nil)
    }
}
