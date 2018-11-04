using System;
using System.Runtime.InteropServices;
using System.Security;
using NewsPortal.Interfaces;

namespace NewsPortal.Models
{
    public class NewsPortalModel : INewsPortalModel
    {
        public string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
