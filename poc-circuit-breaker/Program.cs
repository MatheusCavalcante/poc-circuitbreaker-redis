using poc_circuit_breaker;
using poc_circuit_breaker.Exceptions;
using poc_circuit_breaker.Utils;
using System.Diagnostics;
using System.Text.Json;

var circuitBreaker = new CircuitBreaker(3, 5000);

while (true)
{
    try
    {
        Console.WriteLine("Informe o valor do parâmetro: ");
        var value = Console.ReadLine();

        if (value != null)
        {
            circuitBreaker.Execute(() =>
            {                                                
                var id = new Random().Next(1, 10000);

                var key = $"test_key_{id}";

                var param = new { Id = id, Description = "Dias de vencimento", Value = value };

                string paramSerialized = JsonSerializer.Serialize(param);


                RedisConnectionHelper.Set(key, paramSerialized);

            });
        }
        else
        {
            Console.WriteLine("Circuito Fechado");
            var num = Console.ReadLine();

            Console.WriteLine($"O parâmetro tem o valor: {num}");
        }

    }
    catch (CircuitBreakerOperationException ex)
    {
        Trace.Write(ex);
    }
    catch (OpenCircuitException ex)
    {
        Console.WriteLine(circuitBreaker.IsOpen);
    }
}