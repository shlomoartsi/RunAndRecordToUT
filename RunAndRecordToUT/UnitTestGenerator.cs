using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using Newtonsoft.Json;

namespace RunAndRecordToUT
{
    public class MocksAndInvocations
    {
        //Dictionary of services and invocations
        private readonly Dictionary<string, object> _servicesMockByType;
        private readonly Dictionary<string, InvocationContext> _invocationContextsByType;

        public IReadOnlyDictionary<string, object> ServicesMockByType => _servicesMockByType;
        public IReadOnlyDictionary<string, InvocationContext> InvocationContextsByType => _invocationContextsByType;


        public MocksAndInvocations(RecordInvocationContext recordInvocationContext)
        {
            if (recordInvocationContext == null) throw new ArgumentNullException(nameof(recordInvocationContext));
            if (recordInvocationContext.InvocationContexts == null)
                throw new ArgumentException("InvocationContext is null");
            if (recordInvocationContext.InvocationContexts.Count <= 1) return;

            _servicesMockByType = new Dictionary<string, object>();
            _invocationContextsByType = new Dictionary<string, InvocationContext>();

            var mainServiceInvocation = recordInvocationContext.InvocationContexts.Last();
            if (!mainServiceInvocation.InstanceInterface.HasValue)
            {
                throw new ArgumentException("main service invocation type is missing");
            }

            for (var i = 0; i < recordInvocationContext.InvocationContexts.Count; i++)
            {
                var invocationContext = recordInvocationContext.InvocationContexts[i];

                if (!invocationContext.InstanceInterface.HasValue)
                {
                    throw new ArgumentException($"invocation context number {i} does not have value");
                }

                if (invocationContext.InstanceInterface.Value.FullName ==
                    mainServiceInvocation.InstanceInterface.Value.FullName)
                {
                    //we do not create mocks for the main service
                    continue;
                }

                if (!_servicesMockByType.ContainsKey(invocationContext.InstanceInterface.Value.FullName))
                {
                }

            }
        }

    }

    internal class UnitTestGenerator
    {
        public string GenerateUnitTest(RecordInvocationContext context)
        {
            if (!(context?.InvocationContexts?.Count > 0)) return "no context or empty invocation context";
            var unitTestCode = new List<string>();
            var numberOfInvocations = 1;

            //using code
            unitTestCode.Add("using System;");
            unitTestCode.Add("using System.Collections.Generic;");
            unitTestCode.Add("using System.Text.Json;");
            var instanceInterface = context.InvocationContexts.Last().InstanceInterface;
            if (instanceInterface == null) return "no instance in that begins invocation sequence";

            //class and ns declaration
            unitTestCode.Add($"namespace Tests.{instanceInterface.Value.Name}");
            var className = $"{instanceInterface.Value.Name}Tests_{DateTime.Now}";
            unitTestCode.Add($"public class {className}");
            unitTestCode.Add($"{{");
            unitTestCode.Add($"   {instanceInterface.Value.Namespace}.{instanceInterface.Value.Name} GetService(IService1 service1,IService2 service2)");
            unitTestCode.Add($"   {{");
            unitTestCode.Add($"     //TODO write code that creates the Service");
            unitTestCode.Add($"   }}");

            //for each function call
            for (var invocationNumber = 0;invocationNumber< context.InvocationContexts.Count;invocationNumber++)
            {
                var errorParsing = false; 
                var invocation = context.InvocationContexts[invocationNumber];
                if (invocation.InstanceInterface == null) continue; //log error
                if (!invocation.InstanceInterface.Value.TryGetType(out var interfaceType))
                {
                    //log error
                    break;
                

                }
                if(errorParsing) break;

                unitTestCode.Add($"    [TestMethod]");
                unitTestCode.Add($"    public void Test_{numberOfInvocations}()");
                unitTestCode.Add($"    {{");
                unitTestCode.Add("// UT Body");
                unitTestCode.Add($"    }}");
            }

            unitTestCode.Add($"    }}");
            return string.Join("\n", unitTestCode);
        }
    }
}
