using Castle.DynamicProxy;

namespace RecordAndReplayServices
{
    public class ReplayMockServiceFactory
    {
        private readonly ReplayInvocationContext _context;

        public ReplayMockServiceFactory(ReplayInvocationContext context)
        {
            _context = context;
        }

        public TServiceInterface CreateReplayMock<TServiceInterface>() where TServiceInterface : class
        {
            var interceptor = new ServiceInvocationReplayReturnValueInterceptor(_context,typeof(TServiceInterface));
            var proxyGenerator = new ProxyGenerator();

            // Create proxy for the service interface using the interceptor
            var serviceMock =
                proxyGenerator.CreateInterfaceProxyWithoutTarget<TServiceInterface>(interceptor);

            return serviceMock;
        }
    }
}
