using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poc_circuit_breaker.Models
{
    public enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }
}
