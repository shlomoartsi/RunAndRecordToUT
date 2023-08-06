using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace RecordAndReplayServices
{
    public class ReplayInvocationContext
    {
        public List<InvocationContextRecordForReplay> ValidateInvocationContext { get; set; }
        public RecordInvocationContext RecordInvocationContext { get; }
        public int CurrentInvocationCounter { get; set; }

        public ReplayInvocationContext(RecordInvocationContext invocationContext)
        {
            RecordInvocationContext = invocationContext;
            ValidateInvocationContext = new List<InvocationContextRecordForReplay>();

            foreach (var invocation in RecordInvocationContext.InvocationContexts)
            {
                ValidateInvocationContext.Add(new InvocationContextRecordForReplay(invocation));
            }
        }

        public bool SearchCall(IInvocation invocation,Type serviceType,out object returnValue)
        {
            returnValue = null;
            var found = false;
            foreach (var invocationRecord in ValidateInvocationContext)
            {
                if (invocationRecord.FunctionName == invocation.Method.Name &&
                    invocationRecord.InstanceInterface.Equals(serviceType.ToTypeInfo()) &&
                    CompareArguments(invocation, invocationRecord))
                {
                    returnValue = invocationRecord.ReturnValue;
                    if (!invocationRecord.AlreadyUsedReturnValue)
                    {
                        invocationRecord.AlreadyUsedReturnValue = true;
                        return true;
                    }
                    found = true;
                }
            }

            return found;
        }

        private bool CompareArguments(IInvocation invocation, 
            InvocationContextRecordForReplay invocationContextRecord)
        {
            if (invocation.Method.GetParameters().Length == 0 &&
                invocationContextRecord.CallArgumentTypes == null)
            {
                return true;
            }

            if (invocation.Method.GetParameters().Length != invocationContextRecord.CallArgumentTypes?.Length)
                return false;
            
            for (var i = 0; i < invocation.Method.GetParameters().Length; i++)
            {
                var parameter = invocation.Arguments[i];
                if (!invocationContextRecord.CallArgumentTypes[i].HasValue) return false;
                if (!parameter.GetType().ToTypeInfo().Equals(invocationContextRecord.CallArgumentTypes[i].Value))
                    return false;
            }
            
            return true;
        }
    }
}