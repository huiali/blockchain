using System;
using System.Threading.Tasks;

namespace Huiali.Blockchain.KeyStorage
{
    public struct ExponentialBackoff
    {
        private readonly int _mMaxRetries, _mDelayMilliseconds, _mMaxDelayMilliseconds;
        private int _mRetries, _mPow;

        public ExponentialBackoff(int maxRetries, int delayMilliseconds,
            int maxDelayMilliseconds)
        {
            _mMaxRetries = maxRetries;
            _mDelayMilliseconds = delayMilliseconds;
            _mMaxDelayMilliseconds = maxDelayMilliseconds;
            _mRetries = 0;
            _mPow = 1;
        }

        public Task Delay()
        {
            if (_mRetries == _mMaxRetries)
            {
                throw new TimeoutException("Max retry attempts exceeded.");
            }
            ++_mRetries;
            if (_mRetries < 31)
            {
                _mPow = _mPow << 1; // m_pow = Pow(2, m_retries - 1)
            }
            int delay = Math.Min(_mDelayMilliseconds * (_mPow - 1) / 2,
                _mMaxDelayMilliseconds);
            return Task.Delay(delay);
        }
    }
}
