using Foundation;
using System.Collections.Generic;
using UIKit;

namespace ShareEmergencyContacts.iOS
{
    public class IOSShareProvider : IShareProvider
    {
        public void ShareUrl(string url, string title, string message)
        {

            // create activity items
            var items = new List<NSObject>();
            if (message != null)
                items.Add(new ShareActivityItemSource(new NSString(message), title));
            if (url != null)
                items.Add(new ShareActivityItemSource(NSUrl.FromString(url), title));

            // create activity controller
            var activityController = new UIActivityViewController(items.ToArray(), null);

            // show activity controller
            var vc = GetVisibleViewController();

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                if (activityController.PopoverPresentationController != null)
                {
                    activityController.PopoverPresentationController.SourceView = vc.View;
                }
            }

            vc.PresentViewControllerAsync(activityController, true);
        }

        /// <summary>
        /// Gets the visible view controller.
        /// </summary>
        /// <returns>The visible view controller.</returns>
        UIViewController GetVisibleViewController(UIViewController controller = null)
        {
            controller = controller ?? UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (controller.PresentedViewController == null)
                return controller;

            if (controller.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)controller.PresentedViewController).VisibleViewController;
            }

            if (controller.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)controller.PresentedViewController).SelectedViewController;
            }

            return GetVisibleViewController(controller.PresentedViewController);
        }
    }

    class ShareActivityItemSource : UIActivityItemSource
    {
        private NSObject item;
        private string subject;

        public ShareActivityItemSource(NSObject item, string subject)
        {
            this.item = item;
            this.subject = subject;
        }

        public override NSObject GetItemForActivity(UIActivityViewController activityViewController, NSString activityType)
        {
            return item;
        }

        public override NSObject GetPlaceholderData(UIActivityViewController activityViewController)
        {
            return item;
        }

        public override string GetSubjectForActivity(UIActivityViewController activityViewController, NSString activityType)
        {
            return subject;
        }
    }
}