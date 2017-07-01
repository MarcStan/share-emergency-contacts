using Caliburn.Micro.Xamarin.Forms;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Extensions
{
    /// <summary>
    /// Wrapper for caliburns navigator. Adds extended method
    /// </summary>
    public class ExtendedNavPageAdapter : NavigationPageAdapter
    {
        private readonly NavigationPage _navigationPage;

        public ExtendedNavPageAdapter(NavigationPage navigationPage) : base(navigationPage)
        {
            _navigationPage = navigationPage;
        }


        /// <summary>
        /// Helper that allows to navigate to generated viewmodel.
        /// Caliburn.Micro can only inject items from IoC or set via SetParam which late-binds the values OnNavigated to properties.
        /// It is however not possible to provide params to the ctor directly. This fixes that.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vm"></param>
        /// <param name="animated"></param>
        public Task NavigateToInstanceAsync<T>(T vm, bool animated = true)
        {
            var view = ViewLocator.LocateForModelType(typeof(T), null, null);
            return PushInstanceAsync(view, vm, animated);
        }


        private Task PushInstanceAsync<T>(Element view, T viewModel, bool animated)
        {
            var page = view as Page;

            if (page == null && !(view is ContentView))
                throw new NotSupportedException(String.Format("{0} does not inherit from either {1} or {2}.", view.GetType(), typeof(Page), typeof(ContentView)));

            ViewModelBinder.Bind(viewModel, view, null);

            var contentView = view as ContentView;
            if (contentView != null)
            {
                page = CreateContentPage(contentView, viewModel);
            }

            return _navigationPage.PushAsync(page, animated);
        }
    }
}