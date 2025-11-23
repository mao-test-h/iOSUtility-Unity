namespace iOSUtility.NativeShare
{
    internal interface INativeShare
    {
        void ShareFile(string filePath, string subject, string text);
    }
}
