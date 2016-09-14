using System;

namespace Dropcraft.Deployment.Exceptions
{
    public class FileConflictException : Exception
    {
        public FileConflictException(string msg)
            : base(msg)
        {
            
        }
    }
}
