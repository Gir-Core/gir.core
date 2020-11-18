using System;

namespace GObject
{
    public partial class Object
    {
        /// <summary>
        /// Describes a wrapper type.
        /// </summary>
        public sealed class TypeDescriptor
        {
            #region Fields

            /// <summary>
            /// /// The function to retrieve the GType.
            /// </summary>
            private readonly Func<ulong> _getGType;

            /// <summary>
            /// Cached type.
            /// </summary>
            private Type? _gType;

            #endregion

            #region Properties

            /// <summary>
            /// The name of the wrapped type.
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// The c type of the wrapper.
            /// </summary>
            public Type GType
                => _gType ??= new Type(_getGType());

            #endregion

            #region Constructors

            private TypeDescriptor(string name, Func<ulong> getGType)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                _getGType = getGType ?? throw new ArgumentNullException(nameof(getGType));
            }

            #endregion

            #region Methods

            /// <summary>
            /// Creates a new GType descriptor for the given <paramref name="wrapperName"/>.
            /// </summary>
            /// <remarks>
            /// Use this only if you are creating a Wrapper for a native type. See Gir.Core
            /// documentation to know more about <see cref="TypeDescriptor"/>.
            /// </remarks>
            /// <param name="wrapperName">The name of the wrapper type to describe.</param>
            /// <param name="getType">A function which return the native GType pointer.</param>
            /// <returns>
            /// A new instance of <see cref="TypeDescriptor"/>.
            /// </returns>
            public static TypeDescriptor For(string wrapperName, Func<ulong> getType)
            {
                return new TypeDescriptor(wrapperName, getType);
            }

            #endregion
        }
    }
}
