using XLabs.Forms.Controls;

namespace ShareEmergencyContacts.Helpers.XLabs
{
    /// <summary>
    /// iOS doesn't like generic types in xaml (instant crash on page load).
    /// Thus derive it manually for each type.
    /// </summary>
    /// <remarks>
    /// Originally found solution at: https://stackoverflow.com/questions/26536000/repeaterviewt-in-xamarin-forms-labs
    /// </remarks>
    public class StringRepeater : RepeaterView<string>
    {

    }
}