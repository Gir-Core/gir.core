using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GLib.Core;

namespace GObject.Core
{
    public partial class GObject : Object
    {
        private static Dictionary<IntPtr, GObject> objects = new Dictionary<IntPtr, GObject>();

        private IntPtr handle;
        private HashSet<GClosure> closures;

        protected GObject(IntPtr handle, bool isInitiallyUnowned = false)
        {
            objects.Add(handle, this);
            
            if(isInitiallyUnowned)
                this.handle = global::GObject.Object.ref_sink(handle);
            else
                this.handle = handle;

            closures = new HashSet<GClosure>();
            RegisterOnFinalized();
        }

        private void OnFinalized(IntPtr data, IntPtr where_the_object_was) => Dispose();
        private void RegisterOnFinalized() => global::GObject.Object.weak_ref(this, this.OnFinalized, IntPtr.Zero);

        internal protected void RegisterNotifyPropertyChangedEvent(string propertyName, Action callback) 
            => RegisterEvent($"notify::{propertyName}", callback);

        internal protected void RegisterEvent(string eventName, ActionRefValues callback)
        {
            ThrowIfDisposed();
            RegisterEvent(eventName, new GClosure(this, callback));
        }

        internal protected void RegisterEvent(string eventName, Action callback)
        {
            ThrowIfDisposed();
            RegisterEvent(eventName, new GClosure(this, callback));
        }

        private void RegisterEvent(string eventName, GClosure closure)
        {
            var ret = global::GObject.Methods.signal_connect_closure(handle, eventName, closure, false);

            if(ret == 0)
                throw new Exception($"Could not connect to event {eventName}");

            closures.Add(closure);
        }

        public static T Convert<T>(IntPtr handle, Func<IntPtr, T> factory) where T : GObject
        {
            if(TryGetObject(handle, out T obj))
                return obj;
            else
                return factory(handle);
        }

        private void ThrowIfDisposed()
        {
            if(Disposed)
                throw new Exception("Object is disposed");
        }

        protected static void HandleError(IntPtr error)
        {
            if(error != IntPtr.Zero)
                throw new GException(error);
        }

        public static implicit operator IntPtr (GObject? val) => val?.handle ?? IntPtr.Zero;

        //TODO: Remove implicit cast
        public static implicit operator GObject? (IntPtr val)
        {
            objects.TryGetValue(val, out var ret);
            return ret;
        }

        public static bool TryGetObject<T>(IntPtr handle, out T obj) where T: GObject
        { 
            var result = objects.TryGetValue(handle, out var ret);
            obj = (T) ret;
            
            return result;
        }
    }
}