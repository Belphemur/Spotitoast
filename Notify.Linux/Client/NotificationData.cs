using System;
using System.Drawing;
using JetBrains.Annotations;

namespace Notify.Linux.Client
{
    public class NotificationData
    {
        /// <summary>
        /// Name of the application the notification belongs to
        /// </summary>
        public string ApplicationName { get; set; } = String.Empty;
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
        [CanBeNull] public Image Image { get; set; }
        /// <summary>
        /// How long the notification stays up
        ///
        /// -1 for staying until manually closed
        /// </summary>
        public int Expiration { get; set; } = 1;
        
    }
}