using System;

namespace MyerSplashShared.API
{
    public class APIException : Exception
    {
        public APIException(string message) : base(message)
        {
        }
    }
}