namespace RunAndRecordToUT
{
    public class InvocationContext
    {
        public TypeInfo? InstanceInterface { get; }
        public string FunctionName { get; }
        public object[] CallArguments { get; }
        public TypeInfo?[] CallArgumentTypes { get; }
        public object ReturnValue { get; set; }
        public TypeInfo? ReturnValueType { get; set; }

        public InvocationContext(TypeInfo? instanceInterface,string functionName, object[] callArguments,
            TypeInfo?[] callArgumentTypes, object returnValue,TypeInfo? returnValueType)
        {
            InstanceInterface = instanceInterface;
            FunctionName = functionName;
            CallArguments = callArguments;
            CallArgumentTypes = callArgumentTypes;
            ReturnValue = returnValue;
            ReturnValueType = returnValueType;
        }

    }
}