using System;

namespace RunningApp.Exceptions
{
    class NotExtendableException : Exception
    {
        public NotExtendableException()
        {

        }

        public NotExtendableException(string message)
            : base(message)
        {

        }

        public NotExtendableException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}