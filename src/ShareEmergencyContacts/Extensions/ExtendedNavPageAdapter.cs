using Caliburn.Micro;
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
            if (!CanClose())
                return Task.FromResult(false);

            var view = ViewLocator.LocateForModelType(typeof(T), null, null);

            return PushInstanceAsync(view, vm, animated);
        }

        /// <summary>
        /// Sample implementation as base, but base is priavate.
        /// </summary>
        /// <returns></returns>
        protected bool CanClose()
        {
            var view = _navigationPage.CurrentPage;

            var guard = view?.BindingContext as IGuardClose;

            if (guard != null)
            {
                var shouldCancel = false;
                var runningAsync = true;
                guard.CanClose(result => { runningAsync = false; shouldCancel = !result; });
                if (runningAsync)
                    throw new NotSupportedException("Async CanClose is not supported.");

                if (shouldCancel)
                {
                    return false;
                }
            }

            return true;
        }

        private Task PushInstanceAsync<T>(Element view, T viewModel, bool animated)
        {
            var page = view as Page;

            if (page == null && !(view is ContentView))
                throw new NotSupportedException($"{view.GetType()} does not inherit from either {typeof(Page)} or {typeof(ContentView)}.");

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