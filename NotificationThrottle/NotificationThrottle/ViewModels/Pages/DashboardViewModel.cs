using Notification.Wpf;
using NotificationThrottle.Services;

namespace NotificationThrottle.ViewModels.Pages
{
	public partial class DashboardViewModel : ObservableObject
	{
		private readonly Services.NotificationThrottle _notificationThrottle;
        private readonly NotificationManager _notificationManager;
        public DashboardViewModel(Services.NotificationThrottle notificationThrottle, NotificationManager notificationManager)
		{
			_notificationThrottle = notificationThrottle;
			_notificationManager = notificationManager;
        }

		[ObservableProperty]
		private int _counter = 0;

		[RelayCommand]
		private void OnCounterIncrement()
		{
			Counter++;
		}

		[RelayCommand]
		private void OnShowInfoNotification()
		{
			_notificationThrottle.ShowThrottledNotification("Info", "hello", Notification.Wpf.NotificationType.Information);
        }

		[RelayCommand]
		private void OnShowWarningNotification()
		{
            _notificationThrottle.ShowThrottledNotification("warn", "hello", Notification.Wpf.NotificationType.Warning);
        }

		[RelayCommand]
		private void OnShowErrorNotification()
		{
			_notificationThrottle.ShowThrottledNotification("error", "hello", Notification.Wpf.NotificationType.Error);
        }

		[RelayCommand]
		private void OnShowSuccessNotification()
		{
			_notificationThrottle.ShowThrottledNotification("success", "hello", Notification.Wpf.NotificationType.Success);
        }

		[RelayCommand]
		private void OnShowNotificationExt()
		{
            _notificationManager.ShowThrottledNotification("ext", "hello from ext", Notification.Wpf.NotificationType.Information);

        }
    }
}
