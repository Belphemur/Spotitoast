using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tmds.DBus;

[assembly: InternalsVisibleTo(Tmds.DBus.Connection.DynamicAssemblyName)]
namespace Notify.Linux.DBus
{
    [DBusInterface("io.elementary.desktop.AppLauncherService")]
    interface IAppLauncherService : IDBusObject
    {
        Task<IDisposable> WatchVisibilityChangedAsync(Action<bool> handler, Action<Exception> onError = null);
    }

    [DBusInterface("org.freedesktop.Notifications")]
    interface INotifications : IDBusObject
    {
        Task<string[]> GetCapabilitiesAsync();
        Task CloseNotificationAsync(uint Id);
        Task<uint> NotifyAsync(string AppName, uint ReplacesId, string AppIcon, string Summary, string Body, string[] Actions, IDictionary<string, object> Hints, int ExpireTimeout);
        Task<(string name, string vendor, string version, string specVersion)> GetServerInformationAsync();
        Task<IDisposable> WatchNotificationClosedAsync(Action<(uint id, uint reason)> handler, Action<Exception> onError = null);
        Task<IDisposable> WatchActionInvokedAsync(Action<(uint id, string actionKey)> handler, Action<Exception> onError = null);
    }

    [DBusInterface("org.budgie_desktop.Panel")]
    interface IPanel : IDBusObject
    {
        Task<string> GetVersionAsync();
        Task ActivateActionAsync(int Action);
        Task OpenSettingsAsync();
    }

    [DBusInterface("org.budgie_desktop.Raven")]
    interface IRaven : IDBusObject
    {
        Task ClearNotificationsAsync();
        Task<bool> GetExpandedAsync();
        Task<bool> GetLeftAnchoredAsync();
        Task SetExpandedAsync(bool B);
        Task ToggleAsync();
        Task ToggleAppletViewAsync();
        Task ToggleNotificationsViewAsync();
        Task DismissAsync();
        Task<uint> GetNotificationCountAsync();
        Task<string> GetVersionAsync();
        Task<bool> GetDoNotDisturbStateAsync();
        Task SetDoNotDisturbAsync(bool Enable);
        Task<IDisposable> WatchExpansionChangedAsync(Action<bool> handler, Action<Exception> onError = null);
        Task<IDisposable> WatchAnchorChangedAsync(Action<bool> handler, Action<Exception> onError = null);
        Task<IDisposable> WatchNotificationsChangedAsync(Action handler, Action<Exception> onError = null);
        Task<IDisposable> WatchClearAllNotificationsAsync(Action handler, Action<Exception> onError = null);
        Task<IDisposable> WatchUnreadNotificationsAsync(Action handler, Action<Exception> onError = null);
        Task<IDisposable> WatchReadNotificationsAsync(Action handler, Action<Exception> onError = null);
        Task<IDisposable> WatchDoNotDisturbChangedAsync(Action<bool> handler, Action<Exception> onError = null);
        Task<T> GetAsync<T>(string prop);
        Task<RavenProperties> GetAllAsync();
        Task SetAsync(string prop, object val);
        Task<IDisposable> WatchPropertiesAsync(Action<PropertyChanges> handler);
    }

    [Dictionary]
    class RavenProperties
    {
        private bool _IsExpanded = default (bool);
        public bool IsExpanded
        {
            get
            {
                return _IsExpanded;
            }

            set
            {
                _IsExpanded = (value);
            }
        }
    }

    static class RavenExtensions
    {
        public static Task<bool> GetIsExpandedAsync(this IRaven o) => o.GetAsync<bool>("IsExpanded");
        public static Task SetIsExpandedAsync(this IRaven o, bool val) => o.SetAsync("IsExpanded", val);
    }
}