using Android.Content;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidClipboardProvider : IClipboardProvider
    {
        private readonly Context _context;

        public AndroidClipboardProvider(Context context)
        {
            _context = context;
        }

        public void Copy(string text)
        {
            var clipboardManager = (ClipboardManager)_context.GetSystemService(Context.ClipboardService);

            // Create a new Clip
            ClipData clip = ClipData.NewPlainText("tel", text);

            // Copy the text
            clipboardManager.PrimaryClip = clip;
        }
    }
}