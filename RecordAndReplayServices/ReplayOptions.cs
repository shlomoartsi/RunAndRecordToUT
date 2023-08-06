using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecordAndReplayServices
{
    public class ReplayOptions
    {
        internal readonly struct MethodInfo
        {
            public MethodInfo(TypeInfo serviceType, string methodName)
            {
                ServiceType = serviceType;
                MethodName = methodName;
            }

            public TypeInfo ServiceType { get; }
            public string MethodName { get;  }

            public static bool operator ==(MethodInfo a, MethodInfo b)
            {
                return a.MethodName.Equals(b.MethodName) && a.ServiceType.Equals(b.ServiceType);
            }

            public static bool operator !=(MethodInfo a, MethodInfo b)
            {
                return !(a == b);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is MethodInfo methodInfo)) return false;
                return methodInfo == this;

            }

            public bool Equals(MethodInfo other)
            {
                return this == other;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (ServiceType.GetHashCode() * 397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
                }
            }
        }

        private readonly List<TypeInfo> _servicesConsideringParamsValues;
        private readonly List<MethodInfo> _methodsConsideringParamsValues;

        public ReplayOptions()
        {
            _methodsConsideringParamsValues = new List<MethodInfo>();
            _servicesConsideringParamsValues = new List<TypeInfo>();
        }

        public void AddComparingParamsToService<TInterfaceService>() where TInterfaceService : class
        {
            var typeInfo = typeof(TInterfaceService).ToTypeInfo();

            if (_servicesConsideringParamsValues.Contains(typeInfo)) return;
            
            _servicesConsideringParamsValues.Add(typeInfo);
        }

        public void AddComparingParamsForMethod<TServiceInterface>(string methodName) where TServiceInterface : class
        {
            var methodInfo = new MethodInfo(typeof(TServiceInterface).ToTypeInfo(), methodName);
            if (_methodsConsideringParamsValues.Contains(methodInfo)) return;
            _methodsConsideringParamsValues.Add(methodInfo);
        }

        public bool ContainsService(TypeInfo instanceInterface)
        {
            return (_servicesConsideringParamsValues.Contains(instanceInterface)) ;
        }

        public bool ContainsMethod(TypeInfo typeInfo,string methodName) 
        {
            var methodInfo = new MethodInfo(typeInfo, methodName);
            return _methodsConsideringParamsValues.Contains(methodInfo);
        }
    }
}
