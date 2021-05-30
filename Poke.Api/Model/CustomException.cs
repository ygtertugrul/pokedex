using System;

namespace Poke.Api.Model
{
    public class CustomException : Exception
    {
        public CustomException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        public int StatusCode { get; }
    }
}