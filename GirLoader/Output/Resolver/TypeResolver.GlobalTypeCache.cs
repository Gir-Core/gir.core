namespace GirLoader.Output.Resolver
{
    internal partial class TypeResolver
    {
        private class GlobalTypeCache : TypeCache
        {
            public GlobalTypeCache()
            {
                Add(new Model.Void("none"));
                Add(new Model.Void("void"));

                Add(new Model.Boolean("gboolean"));

                Add(new Model.Float("gfloat"));
                Add(new Model.Float("float"));

                Add(new Model.Pointer("any"));
                Add(new Model.Pointer("gconstpointer"));
                Add(new Model.Pointer("va_list"));
                Add(new Model.GPointer());
                Add(new Model.Pointer("tm"));
                Add(new Model.Pointer("FILE")); //TODO: Automatic convert to some stream?

                // TODO: Should we use UIntPtr here? Non-CLR compliant
                Add(new Model.UnsignedPointer("guintptr"));

                Add(new Model.UnsignedShort("guint16"));
                Add(new Model.UnsignedShort("gunichar2")); //TOOO: UTF16 char?
                Add(new Model.UnsignedShort("gushort"));

                Add(new Model.Short("gint16"));
                Add(new Model.Short("gshort"));

                Add(new Model.Double("double"));
                Add(new Model.Double("gdouble"));
                Add(new Model.Double("long double"));

                Add(new Model.Integer("int"));
                Add(new Model.Integer("gint"));
                Add(new Model.Integer("gatomicrefcount"));
                Add(new Model.Integer("gint32"));
                Add(new Model.Integer("pid_t"));
                Add(new Model.Integer("grefcount"));

                Add(new Model.UnsignedInteger("unsigned int"));
                Add(new Model.UnsignedInteger("unsigned"));
                Add(new Model.UnsignedInteger("guint"));
                Add(new Model.UnsignedInteger("guint32"));
                Add(new Model.UnsignedInteger("gunichar"));
                Add(new Model.UnsignedInteger("uid_t"));

                Add(new Model.Byte("guchar"));
                Add(new Model.Byte("guint8"));

                Add(new Model.UnpointedSignedByte("gchar"));
                Add(new Model.UnpointedSignedByte("char"));
                Add(new Model.SignedByte("gint8"));

                Add(new Model.Long("glong"));
                Add(new Model.Long("gssize"));
                Add(new Model.Long("gint64"));
                Add(new Model.Long("goffset"));
                Add(new Model.Long("time_t"));

                Add(new Model.NativeUnsignedInteger("gsize"));
                Add(new Model.UnsignedLong("guint64"));
                Add(new Model.UnsignedLong("gulong"));

                Add(new Model.Utf8String());
                Add(new Model.PlatformString());
            }
        }
    }
}
