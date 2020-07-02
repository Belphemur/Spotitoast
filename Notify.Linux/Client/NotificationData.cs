using System;
using System.Drawing;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Notify.Linux.Client
{
    public class NotificationData
    {
        public struct Action
        {
            public string Key { get; set; }
            public string Label { get; set; }

            public Func<Task> OnActionCalled { get; set; }
        }

        /// <summary>
        /// Name of the application the notification belongs to
        /// </summary>
        public string ApplicationName { get; set; } = String.Empty;

        /// <summary>
        /// Path to the icon of the application
        /// </summary>
        public string ApplicationIconPath { get; set; } = String.Empty;

        /// <summary>
        /// Title of the notification
        /// </summary>
        public string Summary { get; set; } = String.Empty;

        /// <summary>
        /// Body of the notification
        /// </summary>
        public string Body { get; set; } = String.Empty;

        /// <summary>
        /// Image of the notification
        /// </summary>
        [CanBeNull]
        public Image Image { get; set; }

        /// <summary>
        /// How long the notification stays up
        ///
        /// 0 for needing manual stopping
        /// -1 to respect system settings
        /// </summary>
        public int Expiration { get; set; } = -1;

        /// <summary>
        /// Used when you want to replace the same notification
        /// </summary>
        public uint NotificationId = 0;

        /// <summary>
        /// Set of action and what to do when it's triggered
        /// </summary>
        public Action[] Actions { get; set; }
    }
}