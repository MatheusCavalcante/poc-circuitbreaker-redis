using poc_circuit_breaker;
using poc_circuit_breaker.Exceptions;
using poc_circuit_breaker.Utils;
using System.Diagnostics;
using System.Text.Json;

var circuitBreaker = new CircuitBreaker(3, 5000);

while (true)
{
    Console.WriteLine("Type your name: ");
    var name = Console.ReadLine();

    Console.WriteLine("Type your age: ");
    var age = Console.ReadLine();

    try
    {
        circuitBreaker.Execute(() =>
        {
            var key = Guid.NewGuid().ToString();
            var parameters = new { Id = key, Name = name, Age = age };

            var person = JsonSerializer.Serialize(parameters);

            RedisConnectionHelper.Set(key, person);

        });

    }
    catch (CircuitBreakerOperationException ex)
    {
        Trace.Write(ex);
    }
    catch (OpenCircuitException ex)
    {
        Console.WriteLine("Resource unavailable, a contingency method will be used.");
    }
}