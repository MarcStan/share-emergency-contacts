using ShareEmergencyContacts.Droid.Express.CustomRenderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer))]
namespace ShareEmergencyContacts.Droid
{
    namespace Express.CustomRenderers
    {
        /// <summary>
        /// Workaround for android button styling not working ala https://stackoverflow.com/a/38455653
        /// </summary>
        public class CustomButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
        {
        }
    }
}