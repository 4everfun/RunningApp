using System;

namespace RunningApp.Exceptions
{
    class NotOnMapException : Exception
    {
        public NotOnMapException()
        {

        }

        public NotOnMapException(string message) 
            : base(message)
        {

        }

        public NotOnMapException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}