using System;

namespace Rb.Forms.Barcode.Pcl.Extensions
{
    public static class EventExtension
    {
        public static void Raise(this EventHandler handler, object sender)
        {
            if (handler != null) {
                handler(sender, EventArgs.Empty);
            }
        }
            
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) where T : EventArgs
        {
            if (handler != null)  {
                handler(sender, args);
            }
        }
    }
}

