using System;

namespace GLib.Core
{
    public partial class GVariantType : IDisposable
    {
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                VariantType.free(handle);
                disposedValue = true;
            }
        }

         ~GVariantType() => Dispose(false);

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}