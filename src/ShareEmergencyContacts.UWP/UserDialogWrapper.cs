using Acr.UserDialogs;
using Splat;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace ShareEmergencyContacts.UWP
{
    /// <summary>
    /// Wrapper around Acr dialogs that improves the UWP platform.
    /// Specifically it will substitute the hideous integrated toast with an actual UWP toast notificaton.
    /// </summary>
    public class UserDialogWrapper : IUserDialogs
    {
        private readonly IUserDialogs _dialogs;

        public UserDialogWrapper(IUserDialogs dialogs)
        {
            _dialogs = dialogs;
        }

        public IDisposable Alert(string message, string title = null, string okText = null)
        {
            return _dialogs.Alert(message, title, okText);
        }

        public IDisposable Alert(AlertConfig config)
        {
            return _dialogs.Alert(config);
        }

        public Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = null)
        {
            return _dialogs.AlertAsync(message, title, okText, cancelToken);
        }

        public Task AlertAsync(AlertConfig config, CancellationToken? cancelToken = null)
        {
            return _dialogs.AlertAsync(config, cancelToken);
        }

        public IDisposable ActionSheet(ActionSheetConfig config)
        {
            return _dialogs.ActionSheet(config);
        }

        public Task<string> ActionSheetAsync(string title, string cancel, string destructive, CancellationToken? cancelToken = null,
            params string[] buttons)
        {
            return _dialogs.ActionSheetAsync(title, cancel, destructive, cancelToken, buttons);
        }

        public IDisposable Confirm(ConfirmConfig config)
        {
            return _dialogs.Confirm(config);
        }

        public Task<bool> ConfirmAsync(string message, string title = null, string okText = null, string cancelText = null,
            CancellationToken? cancelToken = null)
        {
            return _dialogs.ConfirmAsync(message, title, okText, cancelText, cancelToken);
        }

        public Task<bool> ConfirmAsync(ConfirmConfig config, CancellationToken? cancelToken = null)
        {
            return _dialogs.ConfirmAsync(config, cancelToken);
        }

        public IDisposable DatePrompt(DatePromptConfig config)
        {
            return _dialogs.DatePrompt(config);
        }

        public Task<DatePromptResult> DatePromptAsync(DatePromptConfig config, CancellationToken? cancelToken = null)
        {
            return _dialogs.DatePromptAsync(config, cancelToken);
        }

        public Task<DatePromptResult> DatePromptAsync(string title = null, DateTime? selectedDate = null, CancellationToken? cancelToken = null)
        {
            return _dialogs.DatePromptAsync(title, selectedDate, cancelToken);
        }

        public IDisposable TimePrompt(TimePromptConfig config)
        {
            return _dialogs.TimePrompt(config);
        }

        public Task<TimePromptResult> TimePromptAsync(TimePromptConfig config, CancellationToken? cancelToken = null)
        {
            return _dialogs.TimePromptAsync(config, cancelToken);
        }

        public Task<TimePromptResult> TimePromptAsync(string title = null, TimeSpan? selectedTime = null, CancellationToken? cancelToken = null)
        {
            return _dialogs.TimePromptAsync(title, selectedTime, cancelToken);
        }

        public IDisposable Prompt(PromptConfig config)
        {
            return _dialogs.Prompt(config);
        }

        public Task<PromptResult> PromptAsync(string message, string title = null, string okText = null, string cancelText = null,
            string placeholder = "", InputType inputType = InputType.Default, CancellationToken? cancelToken = null)
        {
            return _dialogs.PromptAsync(message, title, okText, cancelText, placeholder, inputType, cancelToken);
        }

        public Task<PromptResult> PromptAsync(PromptConfig config, CancellationToken? cancelToken = null)
        {
            return _dialogs.PromptAsync(config, cancelToken);
        }

        public IDisposable Login(LoginConfig config)
        {
            return _dialogs.Login(config);
        }

        public Task<LoginResult> LoginAsync(string title = null, string message = null, CancellationToken? cancelToken = null)
        {
            return _dialogs.LoginAsync(title, message, cancelToken);
        }

        public Task<LoginResult> LoginAsync(LoginConfig config, CancellationToken? cancelToken = null)
        {
            return _dialogs.LoginAsync(config, cancelToken);
        }

        public IProgressDialog Progress(ProgressDialogConfig config)
        {
            return _dialogs.Progress(config);
        }

        public IProgressDialog Loading(string title = null, Action onCancel = null, string cancelText = null, bool show = true,
            MaskType? maskType = null)
        {
            return _dialogs.Loading(title, onCancel, cancelText, show, maskType);
        }

        public IProgressDialog Progress(string title = null, Action onCancel = null, string cancelText = null, bool show = true,
            MaskType? maskType = null)
        {
            return _dialogs.Progress(title, onCancel, cancelText, show, maskType);
        }

        public void ShowLoading(string title = null, MaskType? maskType = null)
        {
            _dialogs.ShowLoading(title, maskType);
        }

        public void HideLoading()
        {
            _dialogs.HideLoading();
        }

        public void ShowImage(IBitmap image, string message, int timeoutMillis = 2000)
        {
            _dialogs.ShowImage(image, message, timeoutMillis);
        }

        public void ShowSuccess(string message, int timeoutMillis = 2000)
        {
            _dialogs.ShowSuccess(message, timeoutMillis);
        }

        public void ShowError(string message, int timeoutMillis = 2000)
        {
            _dialogs.ShowError(message, timeoutMillis);
        }

        public IDisposable Toast(string title, TimeSpan? dismissTimer = null)
        {
            var c = new ToastConfig(title);
            if (dismissTimer.HasValue)
                c.Duration = dismissTimer.Value;

            return Toast(c);
        }

        public IDisposable Toast(ToastConfig cfg)
        {
            var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var textNodes = template.GetElementsByTagName("text");
            textNodes.First().AppendChild(template.CreateTextNode(cfg.Message));

            var toast = new ToastNotification(template);
            toast.Activated += (sender, args) =>
            {
                cfg.Action?.Action?.Invoke();
            };
            var notifier = ToastNotificationManager.CreateToastNotifier();
            notifier.Show(toast);

            // unused for now; was it intended for toast dismissal?
            return new DisposableAction(() => { });
        }
    }
}