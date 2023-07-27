using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RunAndRecordToUT
{
    public class RecordInvocationContext
    {
        public class InvocationValidation
        {
            public TypeInfo TypeInfo;
            public bool ValidateArgs { get; set; }
            public bool ValidateReturnValue { get; set; }
        }

        private readonly List<InvocationContext> _invocationContexts;
        private readonly List<InvocationValidation> _invocationValidation;
        public List<InvocationContext> InvocationContexts => _invocationContexts;
        public List<InvocationValidation> InvocationValidations => _invocationValidation;

        public RecordInvocationContext()
        {
            _invocationContexts = new List<InvocationContext>();
            _invocationValidation = new List<InvocationValidation>();
        }

        public void AddInvocationContext(InvocationContext invocationContext)
        {
            _invocationContexts.Add(invocationContext);
        }

        public void AddServiceToValidateReturnValue<TServiceInterface>()
        {
            var typeInfo = typeof(TServiceInterface).ToTypeInfo();
            var invocationContext = _invocationValidation.Find(ic => ic.TypeInfo.TypeGuid == typeInfo.TypeGuid);
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
            var invocationContext = _invocationValidation.Find(ic => ic.TypeInfo.TypeGuid == typeInfo.TypeGuid);
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

        public static RecordInvocationContext FromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented

            };
            //return JsonSerializer.Deserialize<RecordInvocationContext>(json);
            return JsonConvert.DeserializeObject<RecordInvocationContext>(json,settings);
        }

    }

    public readonly struct TypeInfo
    {

        public TypeInfo(string assemblyName,string @namespace,string name,Guid typeGuid)
        {
            AssemblyName = assemblyName;
            Namespace = @namespace;
            Name = name;
            TypeGuid = typeGuid;
        }

        public string AssemblyName { get; }
        public string Namespace { get; }
        public string Name { get; }
        public Guid TypeGuid { get; }

        public string FullName => Namespace + "." + Name;

        public bool TryGetType(out Type type)
        {
            type = null;

            //try get type of 
            //"System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e",
            var typeWithNs = Namespace + "." + Name;

            //first try specific version type and later non version dependent
            try
            {
                var typeResult = Type.GetType(typeWithNs + ", " + AssemblyName);
                if (typeResult != null)
                {
                    type = typeResult;
                    return true;
                }
            }
            catch
            {
                //Ignore and try another way to load type
            }

            try
            {
                var firstIndexOfComma = AssemblyName.IndexOf(',');
                if (firstIndexOfComma == -1) return false;
                var fullTypeNoVersion = AssemblyName.Substring(0, firstIndexOfComma + 1);
                var typeResult = Type.GetType(typeWithNs + ", " + fullTypeNoVersion);
                if (typeResult == null) return false;
                type = typeResult;
                return true;
            }
            catch 
            {
                //ignore
            }

            return false;
        }
    }

    public static class TypeExtensions
    {
        public static TypeInfo ToTypeInfo(this Type type)
        {
            return new TypeInfo(type.Assembly.FullName, type.Namespace, type.Name,type.GUID);
        }
    }
}
