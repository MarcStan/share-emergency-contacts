using Caliburn.Micro.Xamarin.Forms;
using System;
using System.Threading.Tasks;

namespace ShareEmergencyContacts.Extensions
{
    public static class CaliburnNavServiceExtensions
    {
        /// <summary>
        /// Helper method that allows manual viewmodel creation to pass ctor arguments and then resolves the view + binds on the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nav"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public static Task NavigateToInstanceAsync<T>(this INavigationService nav, T vm)
        {
            if (!(nav is ExtendedNavPageAdapter))
                throw new NotSupportedException("Extension method only works on the " + typeof(ExtendedNavPageAdapter));

            var extended = (ExtendedNavPageAdapter)nav;
            return extended.NavigateToInstanceAsync(vm);
        }
    }
}