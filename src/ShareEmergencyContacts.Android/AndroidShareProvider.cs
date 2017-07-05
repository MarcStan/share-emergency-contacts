using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidShareProvider : IShareProvider
    {
        public void ShareUrl(string url, string title, string message)
        {
            var items = new List<string>();
            if (message != null)
                items.Add(message);
            if (url != null)
                items.Add(url);

            var intent = new Intent(Intent.ActionSend);
            intent.SetType("text/plain");
            intent.PutExtra(Intent.ExtraText, string.Join(Environment.NewLine, items));
            if (title != null)
                intent.PutExtra(Intent.ExtraSubject, title);

            string nullTitle = null;
            var chooserIntent = Intent.CreateChooser(intent, nullTitle);
            chooserIntent.SetFlags(ActivityFlags.ClearTop);
            chooserIntent.SetFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(chooserIntent);
        }
    }
}