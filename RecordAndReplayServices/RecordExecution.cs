using System;
using System.Collections.Concurrent;
using System.IO;
using Castle.DynamicProxy;

namespace RecordAndReplayServices
{

    public class RecordExecution
    {
        internal class RecordedService<TServiceInterface>
        {
            public RecordedService(TServiceInterface recordedService)
            {
                Service = recordedService;
            }
            internal TServiceInterface Service { get; }
            internal bool ValidateArgs { get; set; }
            internal bool ValidateReturnValue { get; set; }
        }

        private readonly ConcurrentDictionary<Type, object> _servicesCollection;
        private readonly RecordInvocationContext _recordInvocations;

        public RecordExecution()
        {
            _servicesCollection = new ConcurrentDictionary<Type, object>();
            _recordInvocations = new RecordInvocationContext();
        }

        internal bool InterceptService<TServiceInterface>(TServiceInterface serviceImpl, out TServiceInterface serviceMock) where TServiceInterface : class
        {
            var interceptor = new ServiceInvocationRecordInterceptor(_recordInvocations,typeof(TServiceInterface));
            var proxyGenerator = new ProxyGenerator();

            // Create proxy for the service interface using the interceptor
            serviceMock =
                proxyGenerator.CreateInterfaceProxyWithTarget<TServiceInterface>(serviceImpl, interceptor);

            return _servicesCollection.TryAdd(typeof(TServiceInterface),
                new RecordedService<TServiceInterface>(serviceMock));
        }

        public void AddServiceToValidateReturnValue<TServiceInterface>()
        {
            if (!_servicesCollection.TryGetValue(typeof(TServiceInterface), out var recordService))
            {
                throw new ApplicationException($"Service {typeof(TServiceInterface)} is not added to record");
            }

            if (!(recordService is RecordedService<TServiceInterface> recordedServiceCast))
            {
                throw new ApplicationException("Internal error, added incorrect service to internal dictionary");
            }

            _recordInvocations.AddServiceToValidateReturnValue<TServiceInterface>();
            recordedServiceCast.ValidateReturnValue = true;
        }

        public void AddServiceToValidateCalledWithArgs<TServiceInterface>()
        {
            if (!_servicesCollection.TryGetValue(typeof(TServiceInterface), out var recordService))
            {
                throw new ApplicationException($"Service {typeof(TServiceInterface)} is not added to record");
            }

            if (!(recordService is RecordedService<TServiceInterface> recordedServiceCast))
            {
                throw new ApplicationException("Internal error, added incorrect service to internal dictionary");
            }

            _recordInvocations.AddServiceToValidateCalledWithArgs<TServiceInterface>();
            recordedServiceCast.ValidateArgs= true;
        }

        public string ToJson()
        {
            return _recordInvocations.ToJson();
        }

        public void SaveToJson(string filePath)
        {
            // Save intercepted data to a file
            var json = _recordInvocations.ToJson();
            File.WriteAllText(filePath, json);

        }
    }
}
