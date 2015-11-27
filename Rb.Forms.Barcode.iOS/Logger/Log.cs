using System;
using Rb.Forms.Barcode.Pcl.Logger;
using IosDebug = System.Diagnostics.Debug;

namespace Rb.Forms.Barcode.iOS.Logger
{
    public static class Log
    {
        public static void Debug(this ILog log, string message) 
        {
            Debug(log, "{0}", message);
        }

        public static void Debug(this ILog log, string message, params object[] args) 
        {

            var prefixedMessage = String.Format("Rb.Forms.Barcode [{0}] {1}", log.GetType().Name, message);
            IosDebug.WriteLine(prefixedMessage, args);
        }
    }
}

