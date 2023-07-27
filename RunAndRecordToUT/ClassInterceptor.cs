using System.Linq;
using Castle.DynamicProxy;

namespace RunAndRecordToUT
{

    public class ClassInterceptor : IInterceptor
    {
        private readonly RecordInvocationContext _invocationContext;

        public ClassInterceptor(RecordInvocationContext invocationContext)
        {
            _invocationContext = invocationContext;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            var context = new InvocationContext(invocation.TargetType.ToTypeInfo(),
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