using System;
using System.Linq;
using Castle.DynamicProxy;

namespace RecordAndReplayServices
{

    public class ServiceInvocationRecordInterceptor : IInterceptor
    {
        private readonly RecordInvocationContext _invocationContext;
        private readonly Type _interfaceType;

        public ServiceInvocationRecordInterceptor(RecordInvocationContext invocationContext,Type interfaceType)
        {
            _invocationContext = invocationContext;
            _interfaceType = interfaceType;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            var context = new InvocationContextRecordForRecord(_interfaceType.ToTypeInfo(),
                invocation.Method.Name, invocation.Arguments,
                invocation.Arguments.Select(arg => arg?.GetType().ToTypeInfo()).ToArray(),
                null,null);

            _invocationContext.AddInvocationContext(context);
            var returnValue = invocation.ReturnValue;
            context.ReturnValue = returnValue;
            context.ReturnValueType = returnValue?.GetType().ToTypeInfo();
        }


    }
}