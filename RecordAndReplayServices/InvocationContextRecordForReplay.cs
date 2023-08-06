using Newtonsoft.Json;

namespace RecordAndReplayServices
{
    public class InvocationContextRecordForReplay : InvocationContextRecordForRecord
    {
        private string _actualCallArgumentsAsJson;
        private string _actualReturnValueAsJson;
        private string _returnValueAsJson;
        private object[] _actualCallArguments;
        private object _actualReturnValue;
        private bool? _actualArgumentDifferent;
        private bool? _actualReturnValueDifferent;
        private string _callArgumentsAsJson;

        public InvocationContextRecordForReplay(InvocationContextRecordForRecord invocationContext):base(
            invocationContext.InstanceInterface,invocationContext.FunctionName,invocationContext.CallArguments,
            invocationContext.CallArgumentTypes,invocationContext.ReturnValue,invocationContext.ReturnValueType)
        {
            
        }

        public object[] ActualCallArguments
        {
            get => _actualCallArguments;
            set => _actualCallArguments = value;
        }

        public object ActualReturnValue
        {
            get => _actualReturnValue;
            set => _actualReturnValue = value;
        }

        public bool? ActualArgumentDifferent
        {
            get => _actualArgumentDifferent;
            set => _actualArgumentDifferent = value;
        }

        public bool? ActualReturnValueDifferent
        {
            get => _actualReturnValueDifferent;
            set => _actualReturnValueDifferent = value;
        }


        public string ReturnValueAsJson
        {
            get
            {
                if (ReturnValue != null && _returnValueAsJson == null)
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                    };
                    _returnValueAsJson = JsonConvert.SerializeObject(ReturnValue,settings);
                }
                return _returnValueAsJson;
            }
        }

        public string CallArgumentsAsJson
        {
            get
            {
                if (CallArguments != null && _callArgumentsAsJson == null)
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                    };
                    _callArgumentsAsJson = JsonConvert.SerializeObject(CallArguments,settings);
                }
                return _callArgumentsAsJson;
            }
        }

        public string ActualCallArgumentsAsJson
        {
            get
            {
                if (CallArguments != null && _actualCallArgumentsAsJson == null)
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                    };
                    _actualCallArgumentsAsJson = JsonConvert.SerializeObject(CallArguments,settings);
                }

                return _actualCallArgumentsAsJson;
            }
        }

        public string ActualReturnValueAsJson
        {
            get
            {
                if (ActualReturnValue != null && _actualReturnValueAsJson == null)
                {
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                    };
                    _actualReturnValueAsJson = JsonConvert.SerializeObject(ActualReturnValue,settings);
                }
                return _actualReturnValueAsJson;
            }
        }

        public bool AlreadyUsedReturnValue { get; set; }
    }
}
