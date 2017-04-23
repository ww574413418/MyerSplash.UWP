using System;

namespace MyerSplashShared.API
{
    public class APIException : Exception
    {
        public string ErrorMessage { get; set; } = "";

        public APIException(string message)
        {
            ErrorMessage = message;
        }

        public APIException()
        {

        }
    }
}
