using Android.Content;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Droid
{
    public class AndroidClipboardProvider : IClipboardProvider
    {
        public void Copy(string text)
        {
            var clipboardManager = (ClipboardManager)Forms.Context.GetSystemService(Context.ClipboardService);

            // Create a new Clip
            ClipData clip = ClipData.NewPlainText("tel", text);

            // Copy the text
            clipboardManager.PrimaryClip = clip;
        }
    }
}