using Android.Content;
using Android.Net;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidPhoneDialProvider : IPhoneDialProvider
    {
        private readonly Context _context;

        public AndroidPhoneDialProvider(Context context)
        {
            _context = context;
        }

        public void Dial(string number, string name)
        {
            var uri = Uri.Parse($"tel:{number}");
            var intent = new Intent(Intent.ActionView, uri);
            _context.StartActivity(intent);
        }
    }
}