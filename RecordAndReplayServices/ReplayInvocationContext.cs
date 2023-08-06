using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Newtonsoft.Json;

namespace RecordAndReplayServices
{
    public class ReplayInvocationContext
    {
        public List<InvocationContextRecordForReplay> ValidateInvocationContext { get; set; }
        public RecordInvocationContext RecordInvocationContext { get; }
        public int CurrentInvocationCounter { get; set; }
        public ReplayOptions ReplayOptions { get;internal set; }

        public ReplayInvocationContext(RecordInvocationContext invocationContext)
        {
            RecordInvocationContext = invocationContext;
            ValidateInvocationContext = new List<InvocationContextRecordForReplay>();

            foreach (var invocation in RecordInvocationContext.InvocationContexts)
            {
                ValidateInvocationContext.Add(new InvocationContextRecordForReplay(invocation));
            }
        }

        public bool SearchCall(IInvocation invocation,Type serviceType,ReplayOptions replayOptions,out object returnValue)
        {
            returnValue = null;
            var found = false;
            foreach (var invocationRecord in ValidateInvocationContext)
            {
                if (invocationRecord.FunctionName == invocation.Method.Name &&
                    invocationRecord.InstanceInterface.Equals(serviceType.ToTypeInfo()) &&
                    CompareArguments(invocation, invocationRecord, replayOptions))
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
            InvocationContextRecordForReplay invocationContextRecord,ReplayOptions replayOptions)
        {
            if (invocation.Method.GetParameters().Length == 0 &&
                invocationContextRecord.CallArgumentTypes == null)
            {
                return true;
            }

            if (invocation.Method.GetParameters().Length != invocationContextRecord.CallArgumentTypes?.Length)
                return false;
            
            if (replayOptions.ContainsService(invocationContextRecord.InstanceInterface) ||
                replayOptions.ContainsMethod(invocationContextRecord.InstanceInterface,
                    invocationContextRecord.FunctionName))
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                };

                //Compare arguments values
                var paramsInRecord = invocationContextRecord.ActualCallArgumentsAsJson;
                var paramsInInvocation = JsonConvert.SerializeObject(invocation.Arguments, settings);
                return paramsInRecord == paramsInInvocation;
            }

            //compare only arguments types
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