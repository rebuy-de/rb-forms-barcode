using System;
using Rb.Forms.Barcode.Pcl;

namespace Rb.Forms.Barcode.Pcl
{
    public class OutOfBoundsException : System.Exception
    {
        public OutOfBoundsException() : base()
        {}

        public OutOfBoundsException(string message) : base(message)
        {}
    }
}

