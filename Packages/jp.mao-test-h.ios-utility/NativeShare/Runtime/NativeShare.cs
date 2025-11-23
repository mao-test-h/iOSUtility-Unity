namespace iOSUtility.NativeShare
{
    /// <summary>
    /// ネイティブのシェア機能の呼び出し
    /// </summary>
    public static class NativeShare
    {
        private static INativeShare _instance;

        private static INativeShare Instance
        {
            get
            {
                _instance ??=
#if UNITY_IOS && !UNITY_EDITOR
                    new NativeShareIOS();
#elif UNITY_EDITOR
                    new NativeShareEditor();
#else
                    new NativeShareDummy();
#endif

                return _instance;
            }
        }

        /// <summary>
        /// ファイルの共有
        /// </summary>
        /// <param name="filePath">共有するファイルのパス</param>
        /// <param name="subject">件名</param>
        /// <param name="text">テキスト</param>
        public static void ShareFile(string filePath, string subject = "", string text = "")
        {
            Instance.ShareFile(filePath, subject, text);
        }
    }
}
