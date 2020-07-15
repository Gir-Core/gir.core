using System;

namespace Gtk.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConnectAttribute : Attribute
    {
        public string? WidgetName { get; }

        public ConnectAttribute(string? widgetName = null)
        {
            WidgetName = widgetName;
        }
    }
}