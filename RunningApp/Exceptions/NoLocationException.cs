using System;

namespace RunningApp.Exceptions
{
    class NoLocationException : Exception
    {
        public NoLocationException()
        {

        }

        public NoLocationException(string message) 
            : base(message)
        {

        }

        public NoLocationException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}