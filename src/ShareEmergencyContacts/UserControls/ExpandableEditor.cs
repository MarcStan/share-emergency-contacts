using Xamarin.Forms;

namespace ShareEmergencyContacts.UserControls
{
    /// <summary>
    /// Helper that auto expands the editor for each new line entered.
    /// </summary>
    public class ExpandableEditor : Editor
    {
        public ExpandableEditor()
        {
            TextChanged += (sender, e) => InvalidateMeasure();
        }
    }
}