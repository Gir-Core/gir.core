using System;
using System.Runtime.InteropServices;

namespace GObject.Sys
{
    [StructLayout(LayoutKind.Sequential)]
    public partial struct Value : IDisposable
    {
        #region Fields

        private IntPtr type;

        private long data1;
        private long data2;

        #endregion

        #region Constructors

        public Value(Type type)
        {
            this.type = IntPtr.Zero;
            data1 = 0;
            data2 = 0;

            Value.init(ref this, type.Value);
        }

        public Value(IntPtr value) : this(Type.Object) => Value.set_object(ref this, value);
        public Value(bool value) : this(Type.Boolean) => Value.set_boolean(ref this, value);
        public Value(int value) : this(Type.Int) => Value.set_int(ref this, value);
        public Value(uint value) : this(Type.UInt) => Value.set_uint(ref this, value);
        public Value(long value) : this(Type.Long) => Value.set_long(ref this, value);
        public Value(double value) : this(Type.Double) => Value.set_double(ref this, value);
        public Value(string value) : this(Type.String) => Value.set_string(ref this, value);

        #endregion

        #region Methods

        /// <summary>
        /// Gets an instance of <see cref="Value"/> from the given <paramref name="value"/>.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="Value"/> if the cast is successful.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The given <paramref name="value"/> has a type which cannot be parsed as a <see cref="Value"/>.
        /// </exception>
        public static Value From(object? value) => value switch
        {
            null => new Value(IntPtr.Zero),
            bool v1 => new Value(v1),
            uint v2 => new Value(v2),
            int v3 => new Value(v3),
            long v4 => new Value(v4),
            double v5 => new Value(v5),
            string v6 => new Value(v6),
            IntPtr v7 => new Value(v7),
            Enum _ => new Value((long) value),
            _ => throw new NotSupportedException("Unable to create the value from the given type.")
        };

        /// <summary>
        /// Casts this <see cref="Value"/> to the type <typeparamref name="T"/>.
        ///
        /// In case of an IntPtr a GObject pointer is returned. This method does not support GPointer.
        /// </summary>
        /// <typeparam name="T">The type in which this value is casted.</typeparam>
        /// <returns>
        /// A value of type <typeparamref name="T"/> if the cast is successful.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// The value cannot be casted to the given type.
        /// </exception>
        public T To<T>()
        {
            System.Type t = typeof(T);
            if (t == typeof(bool)) return (T) (object) Value.get_boolean(ref this);
            if (t == typeof(uint)) return (T) (object) Value.get_uint(ref this);
            if (t == typeof(int)) return (T) (object) Value.get_int(ref this);
            if (t.IsEnum || t == typeof(long)) return (T) (object) Value.get_long(ref this);
            if (t == typeof(double)) return (T) (object) Value.get_double(ref this);
            if (t == typeof(string)) return (T) (object) Marshal.PtrToStringAnsi(Value.get_string(ref this));

            //Warning: This could be GetPointer() or GetObject()!
            if (t == typeof(IntPtr)) return (T) (object) Value.get_object(ref this);

            throw new NotSupportedException("Unable to cast the value to the given type.");
        }

        public void Dispose() => Value.unset(ref this);

        #endregion
    }
}