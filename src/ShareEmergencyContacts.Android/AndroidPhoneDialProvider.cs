using Android.Content;
using Android.Net;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidPhoneDialProvider : IPhoneDialProvider
    {
        public void Dial(string number, string name)
        {
            var uri = Uri.Parse($"tel:{number}");
            var intent = new Intent(Intent.ActionView, uri);
            Forms.Context.StartActivity(intent);
        }
    }
}