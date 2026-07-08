using System;

namespace Delivery.Modelos.Excepciones
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; set; } = 400; // Por defecto Bad Request

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
