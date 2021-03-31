﻿using System;
using System.Runtime.InteropServices;

namespace GLib
{
    public partial record Variant : IDisposable
    {
        #region Fields

        private Variant[] _children;
        private readonly Native.VariantSafeHandle _handle;

        #endregion

        #region Properties

        public Native.VariantSafeHandle Handle => _handle;

        #endregion

        #region Constructors

        public Variant(params Variant[] children)
        {
            _children = children;
            Init(out _handle, children);
        }

        /*public Variant(IDictionary<string, Variant> dictionary)
        {
            var data = new Variant[dictionary.Count];
            var counter = 0;
            foreach(var entry in dictionary)
            {
                var e = new Variant(Variant.new_dict_entry(new Variant(entry.Key).Handle, entry.Value.handle));
                data[counter] = e;
                counter++;
            }
            this.children = data;
            Init(out this.handle, data);
        }*/

        public Variant(Native.VariantSafeHandle handle)
        {
            _children = new Variant[0];
            _handle = handle;
            Native.Methods.RefSink(handle);
        }

        #endregion

        #region Methods

        public static Variant Create(int i) => new Variant(Native.Methods.NewInt32(i));
        public static Variant Create(uint ui) => new Variant(Native.Methods.NewUint32(ui));
        public static Variant Create(string str) => new Variant(Native.Methods.NewString(str));
        public static Variant Create(params string[] strs) => new Variant(Native.Methods.NewStrv(strs, strs.Length));

        public static Variant CreateEmptyDictionary(VariantType key, VariantType value)
        {
            var childType = VariantType.Native.Methods.NewDictEntry(key.Handle, value.Handle);
            return new Variant(Native.Methods.NewArray(childType, new IntPtr[0], 0));
        }

        private void Init(out Native.VariantSafeHandle handle, params Variant[] children)
        {
            _children = children;

            var count = children.Length;
            var ptrs = new IntPtr[count];

            for (var i = 0; i < count; i++)
                ptrs[i] = children[i].Handle.DangerousGetHandle();

            handle = Native.Methods.NewTuple(ptrs, (ulong) count);
            Native.Methods.RefSink(handle);
        }

        public string GetString()
            => Native.Methods.GetString(_handle, out _);

        public int GetInt()
            => Native.Methods.GetInt32(_handle);
        
        public uint GetUInt()
            => Native.Methods.GetUint32(_handle);
        
        public string Print(bool typeAnnotate)
            => Native.Methods.Print(_handle, typeAnnotate);

        #endregion

        public void Dispose()
        {
            foreach(var child in _children)
                child.Dispose();
            
            Handle.Dispose();
        }
    }
}
