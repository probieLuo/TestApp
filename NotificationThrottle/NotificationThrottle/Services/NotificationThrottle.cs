using Notification.Wpf;

namespace NotificationThrottle.Services
{
	public class NotificationThrottle
	{
        /// <summary>
        /// 存储每个警告标识的最后弹窗时间
        /// </summary>
        private readonly Dictionary<string, DateTime> _lastNotifyTimes = new Dictionary<string, DateTime>();
		/// <summary>
		/// 冷却时间
		/// </summary>
		private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(10);

		private readonly NotificationManager _notificationManager;

		public NotificationThrottle(NotificationManager notificationManager)
		{
			_notificationManager = notificationManager;
		}

        /// <summary>
        /// 过滤重复通知或重复类型的通知
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <param name="notificationType"></param>
        public void ShowThrottledNotification(string id, string message,NotificationType notificationType=NotificationType.Information)
		{
			if (!_lastNotifyTimes.TryGetValue(id, out DateTime lastTime))
			{
                _lastNotifyTimes[id] = DateTime.Now;
            }

			TimeSpan elapsed = DateTime.Now - lastTime;
			if (elapsed < _cooldown)
			{
				return;
			}

			_notificationManager.Show(message);
			_lastNotifyTimes[id] = DateTime.Now;
		}
	}
}
