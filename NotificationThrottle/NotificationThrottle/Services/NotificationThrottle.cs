using Notification.Wpf;

namespace NotificationThrottle.Services
{
	public class NotificationThrottle
	{
        /// <summary>
        /// 存储每个警告标识的最后弹窗时间和次数
        /// </summary>
        private readonly Dictionary<string, TrackInfo> _trackInfo = new Dictionary<string, TrackInfo>();
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
            if(!_trackInfo.TryGetValue(id, out TrackInfo trackInfoValue))
			{
                trackInfoValue = new TrackInfo()
                {
                    LastTime = DateTime.MinValue,
                    Count = 1
                };
                _trackInfo[id] = trackInfoValue;
            }

            TimeSpan elapsed = DateTime.Now - trackInfoValue.LastTime;
			if (elapsed < _cooldown)
			{
                trackInfoValue.Count++;
				return;
			}

			_notificationManager.Show(Enum.GetName(typeof(NotificationType), notificationType), $"{message}, 期间尝试弹出{trackInfoValue.Count}次", notificationType);

            trackInfoValue.LastTime = DateTime.Now;
            _trackInfo[id] = trackInfoValue;
		}

		private class TrackInfo
		{
			public DateTime LastTime {  get; set; }
			public int Count {  get; set; }
        }
	}


    public static class NotificationManagerExtensions
    {
        /// <summary>
        /// 存储每个警告标识的最后弹窗时间和次数
        /// </summary>
        private static readonly Dictionary<string, TrackInfo> _trackInfo = new Dictionary<string, TrackInfo>();
        /// <summary>
        /// 冷却时间
        /// </summary>
        private static readonly TimeSpan _cooldown = TimeSpan.FromSeconds(10);
        public static void ShowThrottledNotification(this NotificationManager notificationManager, string id, string message, NotificationType notificationType = NotificationType.Information)
        {
            if (!_trackInfo.TryGetValue(id, out TrackInfo trackInfoValue))
            {
                trackInfoValue = new TrackInfo()
                {
                    LastTime = DateTime.MinValue,
                    Count = 1
                };
                _trackInfo[id] = trackInfoValue;
            }

            TimeSpan elapsed = DateTime.Now - trackInfoValue.LastTime;
            if (elapsed < _cooldown)
            {
                trackInfoValue.Count++;
                return;
            }

            notificationManager.Show(Enum.GetName(typeof(NotificationType), notificationType), $"{message}, 期间尝试弹出{trackInfoValue.Count}次", notificationType);

            trackInfoValue.LastTime = DateTime.Now;
            _trackInfo[id] = trackInfoValue;
        }

        private class TrackInfo
        {
            public DateTime LastTime { get; set; }
            public int Count { get; set; }
        }
    }
}
