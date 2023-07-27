using System.IO;
using System.Text.Json;
using Castle.DynamicProxy;

namespace RunAndRecordToUT
{

    public class Program
    {
        public static void Main()
        {
            var calculatorService = new CalculatorService();

            var recordExecution = RecordExecutionBuilder.Create()
                .AddServiceToRecord<ICalculatorService>(calculatorService, out var calculatorMock)
                .ValidateServiceReturnValue<ICalculatorService>()
                .Build();

                // Intercept function calls and save arguments/return values
            _ = calculatorMock.Add(new Real(3.4f) , new Real(3.8f) );
            _ = calculatorMock.Add( 2, 3);
            _ = calculatorMock.Add(1, 4);
            calculatorMock.SaveMemory(4.33f);
            _ = calculatorMock.Memory;

            var filePath = "serialized_data.json";

            recordExecution.SaveToJson(filePath);

            // Generate unit test code
            var invocationContextFromJson = RecordInvocationContext.FromJson(filePath);
            var utGenerator = new UnitTestGenerator();
            utGenerator.GenerateUnitTest(invocationContextFromJson);
        }
    }
}
