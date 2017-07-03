using Windows.ApplicationModel.DataTransfer;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsClipboardProvider : IClipboardProvider
    {
        public void Copy(string text)
        {
            var p = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            p.SetText(text);
            Clipboard.SetContent(p);
        }
    }
}