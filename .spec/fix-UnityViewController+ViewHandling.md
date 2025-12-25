# 概要

- Unity が iOS ビルドで自動生成する Xcode プロジェクトにある `UnityAppController+ViewHandling.mm` に処理を書き足すエディタ拡張の実装案です
  - フォルダパスは `(ビルド後の出力先)/Classes/UI/UnityAppController+ViewHandling.mm` にあります
- こちらを `PostProcessBuildAttribute` を用いて Unity のビルド完了後に直接ソースファイルを書き換えます

## 書き換える内容について

- 以下の `interfaceWillChangeOrientationTo`, `interfaceDidChangeOrientationFrom` を after のように書き換えてください。
- また、Replace, Append どちらも対応できるように都度既に書き込み済みかをチェックし、書き込み済みであれば無視してください

**Before**

```objc
- (void)interfaceWillChangeOrientationTo:(UIInterfaceOrientation)toInterfaceOrientation
{
    UIInterfaceOrientation fromInterfaceOrientation = _curOrientation;

    _curOrientation = toInterfaceOrientation;
    [_unityView willRotateToOrientation: toInterfaceOrientation fromOrientation: fromInterfaceOrientation];
}

- (void)interfaceDidChangeOrientationFrom:(UIInterfaceOrientation)fromInterfaceOrientation
{
    [_unityView didRotate];
}
```

**after**

```objc
- (void)interfaceWillChangeOrientationTo:(UIInterfaceOrientation)toInterfaceOrientation
{
    // 追記
    AppController_SendUnityViewControllerNotification(@"kUnityInterfaceWillChangeOrientation");

    UIInterfaceOrientation fromInterfaceOrientation = _curOrientation;

    _curOrientation = toInterfaceOrientation;
    [_unityView willRotateToOrientation: toInterfaceOrientation fromOrientation: fromInterfaceOrientation];
}

- (void)interfaceDidChangeOrientationFrom:(UIInterfaceOrientation)fromInterfaceOrientation
{
    // 追記
    AppController_SendUnityViewControllerNotification(@"kUnityInterfaceDidChangeOrientation");

    [_unityView didRotate];
}
```
