using System;
using Windows.ApplicationModel.DataTransfer;

namespace ShareEmergencyContacts.UWP
{
    public class WindowsShareProvider : IShareProvider
    {
        private DataTransferManager _dataTransferManager;

        public void ShareUrl(string url, string title, string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (_dataTransferManager == null)
            {
                _dataTransferManager = DataTransferManager.GetForCurrentView();
                _dataTransferManager.DataRequested += (sender, args) =>
                {
                    var request = args.Request;

                    // The Title is mandatory
                    request.Data.Properties.Title = title ?? Windows.ApplicationModel.Package.Current.DisplayName;

                    if (message != null)
                        request.Data.SetText(message);
                    if (url != null)
                        request.Data.SetWebLink(new Uri(url));
                };
            }
            DataTransferManager.ShowShareUI();
        }
    }
}