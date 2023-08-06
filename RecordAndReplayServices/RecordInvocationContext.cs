using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RecordAndReplayServices
{
    public class RecordInvocationContext
    {
        public class InvocationValidation
        {
            public TypeInfo TypeInfo;
            public bool ValidateArgs { get; set; }
            public bool ValidateReturnValue { get; set; }
        }

        private readonly List<InvocationContextRecordForRecord> _invocationContexts;
        private readonly List<InvocationValidation> _invocationValidation;
        public List<InvocationContextRecordForRecord> InvocationContexts => _invocationContexts;
        public List<InvocationValidation> InvocationValidations => _invocationValidation;

        public RecordInvocationContext()
        {
            _invocationContexts = new List<InvocationContextRecordForRecord>();
            _invocationValidation = new List<InvocationValidation>();
        }

        public void AddInvocationContext(InvocationContextRecordForRecord invocationContext)
        {
            _invocationContexts.Add(invocationContext);
        }

        public void AddServiceToValidateReturnValue<TServiceInterface>()
        {
            var typeInfo = typeof(TServiceInterface).ToTypeInfo();
            var invocationContext = _invocationValidation.Find(ic => ic.TypeInfo.Equals(typeInfo));
            if (invocationContext != null)
            {
                invocationContext.ValidateReturnValue = true;
            }
            else
            {
                _invocationValidation.Add(new InvocationValidation()
                {
                    TypeInfo = typeInfo,
                    ValidateReturnValue = true,
                });
            }
        }

        public void AddServiceToValidateCalledWithArgs<TServiceInterface>()
        {
            var typeInfo = typeof(TServiceInterface).ToTypeInfo();
            var invocationContext = _invocationValidation.Find(ic => ic.TypeInfo.Equals(typeInfo));
            if (invocationContext != null)
            {
                invocationContext.ValidateArgs = true;
            }
            else
            {
                _invocationValidation.Add(new InvocationValidation()
                {
                    TypeInfo = typeInfo,
                    ValidateReturnValue = true,
                });
            }
        }
        public string ToJson()
        {
            //return JsonSerializer.Serialize(this);

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(this,settings);
        }

        public static RecordInvocationContext FromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            return JsonConvert.DeserializeObject<RecordInvocationContext>(json, settings);
        }

        public static RecordInvocationContext FromJsonFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return FromJson(json);
        }

    }

    public static class TypeExtensions
    {
        public static TypeInfo ToTypeInfo(this Type type)
        {
            return new TypeInfo(type.Assembly.FullName, type.Namespace, type.Name);
        }
    }
}
