
using System;
using Confluent.Kafka;
using apichat.Service;

namespace kafka_consumer
{
    public class Worker : BackgroundService
    {
        IConfiguration _Configuration;
        public Worker(IConfiguration Configuration)
        {
            _Configuration = Configuration;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using var kafka = new Kafka(_Configuration);
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(10);
                        ConsumeResult<Null, string> consumedData = kafka.Consume("chat", stoppingToken);
                        //if EnablePartitionEof is set to true. This value can be used to check whether there is no more data to read or the data on that offset is null. 
                        if (consumedData == null)
                        {
                            continue;
                        }
                        if (consumedData.IsPartitionEOF)
                        {
                            //put delay here
                            Console.WriteLine("No data left in kafka to read!");
                        }
                        //Get the consumed message value using consumedData.Message.Value.
                        Console.WriteLine($"Message consumed: {consumedData.Message.Value}");
                    }
                    catch (ConsumeException ex)
                    {
                        Console.WriteLine($"Consumer Exception occurred {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                        Console.WriteLine($"Exception occurred {ex.Message}");
                    }
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("종료됨");
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }
            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public virtual void Dispose()
        {
            Console.WriteLine("Dispose 됨");
            _stoppingCts.Cancel();
        }
    }
}
