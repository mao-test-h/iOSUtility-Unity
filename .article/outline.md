
# Native Event Listener に関する技術記事執筆内容

- こちらのパッケージ内にある `Native Event Listener` についての解説記事を執筆します

## アウトライン (話したいこと)

- 1. はじめに
  - ライフサイクルイベントやビューのイベントを受け取る術は無いかと iOS ビルドで出力されるプロジェクトを探っていたところ、`Classes/PluginBase` と言うのを見つけた
  - こちらを使えば以下のイベントを取得できることが分かったので、簡単に機能について解説していく
    - UnityViewControllerListener
    - LifeCycleListenr
    - AppDelegateListener
- 2. 各種イベントについて
  - 2.1. UnityViewControllerListener について
    - 取得可能なイベントの概要説明
  - 2.2. LifeCycleListenr について
    - 取得可能なイベントの概要説明
  - 2.3 AppDelegateListener について
    - 取得可能なイベントの概要説明
- 3. Unity で活用する際のプラグインの実装例
  - このパッケージの実装内容を元に `UnityViewControllerListener` を例に解説
  - 備考: Swift では無く ObjC++ で実装している理由について
    - Swift で実装することも出来なくは無いが、 `UnityViewControllerListener` にアクセスするには Umbrella Header に登録する必要があり、それが手間なので ObjC++ で実装している
  - `./Assets/_Example` 以下には `NativeUIController` を実装しており、こちらでネイティブ側で View の追加や削除を行うことでイベントが発火さてているのが確認できる
- 4. 各種 Listener がどう呼び出されているのか？
  - ビルド後の iOS プロジェクトの中身を見てどうイベントが発火されているのか？について補足
- 5. おわりに
  - 必ずしもこちらのイベントを使う必要はなく、Unity の場合には一部のイベントに関してはプラグインを作らずとも取得することが可能
    - [`MonoBehaviour.OnApplicationPause`](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.OnApplicationPause.html)
    - [`Application.lowMemory`](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Application-lowMemory.html)
    - など
  - これらのイベントだけだと完結できないケースや、ネイティブで `UIViewController` を切り替えた際の破棄処理みたいなケースでは使えるかもしれない


## 備考

- パッケージにある他の機能 `NativeShare` などの解説は不要
- Unity の iOS ビルドで出力されるプロジェクトは `./Builds/iOS/reference` を参照
