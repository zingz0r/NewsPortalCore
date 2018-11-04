using System;
using System.Security;

namespace NewsPortal.Interfaces
{
    public interface INewsPortalModel
    {
        string SecureStringToString(SecureString value);
    }
}
