using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Runtime.Utilities
{
    public static class UnitaskUtilities
    {
        public static UniTask WaitForSecondsAsync(float time)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(time));
        }
        public static UniTask WaitForSecondsAsync(float time, CancellationTokenSource cts)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: cts.Token);
        }
        public static UniTask WaitUntilAsync(Func<bool> value)
        {
            return UniTask.WaitUntil(value);
        }
        public static UniTask WaitUntilAsync(Func<bool> value, CancellationTokenSource cts)
        {
            return UniTask.WaitUntil(value, cancellationToken: cts.Token);
        }
    }
}