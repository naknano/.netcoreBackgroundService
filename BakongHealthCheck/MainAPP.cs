using BakongHealthCheck.Configures;
using BakongHealthCheck.Repository;
using BakongHealthCheck.Services;

namespace BakongHealthCheck
{
    public class MainAPP : IHostedService  //, IDisposable
    {
        // private Timer? timer;
        private Timer _timer;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IConfigureBakong configure;
        public MainAPP(IServiceScopeFactory scopeFactory, IConfigureBakong configure)
        {
            this.scopeFactory = scopeFactory;
            this.configure = configure;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 300 = 5 mn

            int schduleTime = Convert.ToInt32(configure.BakongTimeService);
            _timer = new Timer(Process, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(schduleTime));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Process(object? state)
        {
            var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBCService>();
            service.BakongHealthCheck();
            
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
