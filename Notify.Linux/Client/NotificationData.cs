using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Notify.Linux.Client
{
    public class NotificationData
    {
        /// <summary>
        /// Action available on the notification
        /// </summary>
        public struct Action
        {
            /// <summary>
            /// Unique Key for the action in a notification
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// Label seen by the user for this action
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// Function that will be called when the user click on the action
            /// </summary>
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
        /// null to respect system settings
        /// </summary>
        public TimeSpan? Expiration { get; set; }

        /// <summary>
        /// Used when you want to replace the same notification
        /// </summary>
        public uint NotificationId = 0;

        /// <summary>
        /// Set of action and what to do when it's triggered
        /// </summary>
        public Action[] Actions { get; set; }

        /// <summary>
        /// Specific hints for this notification
        /// </summary>
        public Dictionary<string, object> Hints { get; } = new Dictionary<string, object>();
    }
}