using System;
using Castle.DynamicProxy;

namespace RecordAndReplayServices
{
    public class ServiceInvocationReplayReturnValueInterceptor : IInterceptor
    {
        private readonly ReplayInvocationContext _invocationContext;
        private readonly Type _serviceType;

        public ServiceInvocationReplayReturnValueInterceptor(ReplayInvocationContext invocationContext,Type serviceType)
        {
            _invocationContext = invocationContext;
            _serviceType = serviceType;
        }

        public void Intercept(IInvocation invocation)
        {
            _invocationContext.CurrentInvocationCounter++;
            
            //in here set the return value as it was replayed.
            //get the return value by searching replayed call with the same arguments
            var wasCallFound = _invocationContext.SearchCall(invocation,_serviceType,
                _invocationContext.ReplayOptions,out var returnValue);

            if (wasCallFound)
            {
                if (returnValue == null)
                {
                    return;
                }

                if (invocation.Method.ReturnType == returnValue.GetType())
                {
                    invocation.ReturnValue = returnValue;
                }
                else
                {
                    invocation.ReturnValue = Convert.ChangeType(returnValue, invocation.Method.ReturnType);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Can not execute method, no recorded data corresponds to arguments");
            }
        }
    }
}