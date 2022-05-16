using poc_circuit_breaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poc_circuit_breaker.Interfaces
{
    public interface ICircuitBreaker
    {
        CircuitBreakerState State { get; }
        void Reset();
        void Execute(Action action);
        bool IsClosed { get; }
        bool IsOpen { get; }
    }
}
