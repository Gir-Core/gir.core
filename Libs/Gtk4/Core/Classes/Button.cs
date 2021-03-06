using System;
using GObject;

namespace Gtk
{
    public partial class Button
    {
        public Button() : this(Sys.Button.@new()) { }
        public Button(string text) : this(Sys.Button.new_with_label(text)) { }
    }
}
