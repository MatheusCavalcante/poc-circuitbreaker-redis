using poc_circuit_breaker.Exceptions;
using poc_circuit_breaker.Interfaces;
using poc_circuit_breaker.Models;
using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace poc_circuit_breaker
{
    public class CircuitBreaker : ICircuitBreaker
    {
        public event EventHandler StateChanged;
        private object monitor = new object();
        private DateTime? circuitClosedDateTime;
        public int Timeout { get; private set; }
        public int Threshold { get; private set; }
        public CircuitBreakerState State { get; private set; }
        private Timer Timer { get; set; }
        private int FailureCount { get; set; }
        private Action Action { get; set; }
        public bool IsClosed { get { return State == CircuitBreakerState.Closed; } }
        public bool IsOpen { get { return State == CircuitBreakerState.Open; } }

        public CircuitBreaker(int threshold = 5, int timeout = 10000)
        {
            if (threshold <= 0)
                throw new ArgumentOutOfRangeException($"{threshold} must be greater than zero");
            if (timeout <= 0)
                throw new ArgumentOutOfRangeException($"{timeout} must be greater than zero");
            this.Threshold = threshold;
            this.Timeout = timeout;
            this.State = CircuitBreakerState.Closed;
            this.Timer = new Timer(timeout);
            this.Timer.Enabled = false;
            this.Timer.Elapsed += Timer_Elapsed;
        }

        public void Execute(Action action)
        {
            if (circuitClosedDateTime?.AddMinutes(2) < DateTime.Now)
                this.State = CircuitBreakerState.HalfOpen;

            if (this.State == CircuitBreakerState.Open)
                throw new OpenCircuitException("Circuit breaker is currently open");

            lock (monitor)
            {
                try
                {
                    this.Action = action;
                    this.Action();
                }
                catch (Exception ex)
                {
                    if (this.State == CircuitBreakerState.HalfOpen)
                    {
                        Trip();
                    }
                    else if (this.FailureCount <= this.Threshold)
                    {
                        this.FailureCount++;                        
                        if (this.Timer.Enabled == false)
                            this.Timer.Enabled = true;
                    }
                    else if (this.FailureCount > this.Threshold)
                    {
                        Trip();
                    }
                    throw new CircuitBreakerOperationException("Operation failed", ex);
                }
                if (this.State == CircuitBreakerState.HalfOpen)
                {
                    Reset();
                }
                if (this.FailureCount > 0)
                {
                    this.FailureCount--;
                }
            }
        }
        public void Reset()
        {
            if (this.State != CircuitBreakerState.Closed)
            {
                Trace.WriteLine($"Circuit Closed");
                ChangeState(CircuitBreakerState.Closed);
                this.Timer.Stop();
            }
        }

        private void Trip()
        {
            if (this.State != CircuitBreakerState.Open)
            {
                Trace.WriteLine($"Circuit Open");
                ChangeState(CircuitBreakerState.Open);
            }
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (monitor)
            {
                try
                {
                    Trace.WriteLine($"Retry, Execution nº {this.FailureCount}");
                    Execute(this.Action);
                    Reset();
                }
                catch
                {
                    if (this.FailureCount > this.Threshold)
                    {
                        Trip();
                        this.Timer.Elapsed -= Timer_Elapsed;
                        this.Timer.Enabled = false;
                        this.Timer.Stop();
                        circuitClosedDateTime = DateTime.Now;

                    }
                }
            }
        }
        private void ChangeState(CircuitBreakerState state)
        {
            this.State = state;
            this.OnCircuitBreakerStateChanged(new EventArgs() { });
        }
        private void OnCircuitBreakerStateChanged(EventArgs e)
        {
            if (this.StateChanged != null)
            {
                StateChanged(this, e);
            }
        }
    }
}
