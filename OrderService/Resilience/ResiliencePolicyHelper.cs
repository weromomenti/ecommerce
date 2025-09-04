using Polly;
using Polly.CircuitBreaker;

namespace OrderService.Resilience
{
    public static class ResiliencePolicyHelper
    {
        private static readonly ILogger logger = 
            LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(nameof(ResiliencePolicyHelper));
        public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(msg => !msg.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, breakDelay) =>
                {
                    logger.LogCritical("Circuit broken! Breaking for {breakDelay.TotalSeconds}s", breakDelay.Seconds);
                },
                onReset: () =>
                {
                    logger.LogInformation("Circuit reset! Resuming normal operations.");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation("Circuit half-open! Testing...");
                });
        }
    }
}
