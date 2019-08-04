using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Huiali.Blockchain.KeyStorage
{
    public sealed class RetryWithExponentialBackoff
    {
        private readonly int _maxRetries, _delayMilliseconds, _maxDelayMilliseconds;

        public RetryWithExponentialBackoff(int maxRetries = 50,
            int delayMilliseconds = 200,
            int maxDelayMilliseconds = 2000)
        {
            this._maxRetries = maxRetries;
            this._delayMilliseconds = delayMilliseconds;
            this._maxDelayMilliseconds = maxDelayMilliseconds;
        }

        public async Task RunAsync(Func<Task> func)
        {
            var backoff = new ExponentialBackoff(this._maxRetries,
                this._delayMilliseconds,
                this._maxDelayMilliseconds);
            retry:
            try
            {
                await func();
            }
            catch (Exception ex) when (ex is TimeoutException ||
                ex is System.Net.Http.HttpRequestException)
            {
                Debug.WriteLine("Exception raised is: " +
                    ex.GetType().ToString() +
                    " –Message: " + ex.Message +
                    " -- Inner Message: " +
                    ex.InnerException.Message);
                await backoff.Delay();
                goto retry;
            }
        }
    }
}
