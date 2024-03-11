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
                _invocationContext.ReplayOptions,out var invocationContextRecord);

            if (wasCallFound)
            {

                //fill out variable with the return value
                var methodParameters = invocation.Method.GetParameters();
                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (!methodParameters[i].IsOut && !methodParameters[i].ParameterType.IsByRef) continue;
                    if (invocationContextRecord.CallArguments[i].GetType() == methodParameters[i].ParameterType)
                    {
                        invocation.Arguments[i] = invocationContextRecord.CallArguments[i];
                    }
                    else
                    {
                        invocation.Arguments[i] = Convert.ChangeType(invocationContextRecord.CallArguments[i],
                            methodParameters[i].ParameterType.GetElementType());
                    }
                }

                if (invocationContextRecord.ReturnValue == null)
                {
                    return;
                }

                if (invocation.Method.ReturnType == invocationContextRecord.ReturnValue.GetType())
                {
                    invocation.ReturnValue = invocationContextRecord.ReturnValue;
                }
                else
                {
                    invocation.ReturnValue = Convert.ChangeType(invocationContextRecord.ReturnValue,
                        invocation.Method.ReturnType);
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