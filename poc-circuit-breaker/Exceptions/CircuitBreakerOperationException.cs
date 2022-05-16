using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poc_circuit_breaker.Exceptions
{
    public class CircuitBreakerOperationException : Exception
    {
        public CircuitBreakerOperationException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
