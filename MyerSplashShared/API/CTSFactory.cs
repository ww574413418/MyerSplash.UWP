using System.Threading;

namespace MyerSplashShared.API
{
    public static class CTSFactory
    {
        public static CancellationTokenSource MakeCTS(int? timeout)
        {
            return new CancellationTokenSource(timeout == null ? 10000 : timeout.Value);
        }

        public static CancellationTokenSource MakeCTS()
        {
            return new CancellationTokenSource();
        }
    }
}