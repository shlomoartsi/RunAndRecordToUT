using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecordAndReplayServices;

namespace Test
{
    [TestClass]
    public class RecordAndReplayBasicTests
    {
        [TestMethod]
        public void TestRecordAndReplayBasic()
        {
            //arrange
            var calculatorService = new CalculatorService();

            var recordExecution = RecordExecutionBuilder.Create()
                .AddServiceToRecord<ICalculatorService>(calculatorService, out var calculatorRecordMock)
                .ValidateServiceReturnValue<ICalculatorService>()
                .Build();

            //act
            // Intercept function calls and save arguments/return values
            _ = calculatorRecordMock.Add(new Real(3.4f), new Real(3.8f));
            _ = calculatorRecordMock.Add(2, 3);
            calculatorRecordMock.SaveMemory(4.33f);
            _ = calculatorRecordMock.Memory;

            //use this to save to file
            //var filePath = "serialized_data.json";
            //recordExecution.SaveToJson(filePath);
            var json = recordExecution.ToJson();

            //Use this when json from file
            //var invocationContextFromJson = RecordInvocationContext.FromJsonFile(filePath);
            var invocationContextFromJson = RecordInvocationContext.FromJson(json);

            var replayContext = new ReplayInvocationContext(invocationContextFromJson);
            var calculatorReplayMock = new ReplayMockServiceFactory(replayContext).CreateReplayMock<ICalculatorService>();
            var result1 = calculatorReplayMock.Add(2, 3);
            var result2 = calculatorReplayMock.Add(new Real(3.4f), new Real(3.8f));
            calculatorReplayMock.SaveMemory(4.33f);
            var memRestored = calculatorReplayMock.Memory;

            //assert
            Assert.AreEqual(result1,5);
            Assert.AreEqual(result2,7.2f);
            Assert.AreEqual(memRestored,4.33f);
        }


        [TestMethod]
        public void TestRecordAndReplay_ConsiderParamsValue()
        {
            //arrange
            var calculatorService = new CalculatorService();

            var recordExecution = RecordExecutionBuilder.Create()
                .AddServiceToRecord<ICalculatorService>(calculatorService, out var calculatorRecordMock)
                .ValidateServiceReturnValue<ICalculatorService>()
                .Build();

            //act
            // Intercept function calls and save arguments/return values
            _ = calculatorRecordMock.Add(new Real(3.4f), new Real(3.8f));
            _ = calculatorRecordMock.Add(new Real(4.4f), new Real(3.8f));

            var json = recordExecution.ToJson();

            var invocationContextFromJson = RecordInvocationContext.FromJson(json);

            var replayContext = new ReplayInvocationContext(invocationContextFromJson);
            var replayOptions = new ReplayOptions();
            replayOptions.AddComparingParamsToService<ICalculatorService>();
            var calculatorReplayMock = new ReplayMockServiceFactory(replayContext,replayOptions).
                CreateReplayMock<ICalculatorService>();
            
            //replay service in the reserve to real execution
            var result1 = calculatorReplayMock.Add(new Real(4.4f), new Real(3.8f));
            var result2 = calculatorReplayMock.Add(new Real(3.4f), new Real(3.8f));

            //assert
            Assert.AreEqual(result1, 8.2f);
            Assert.AreEqual(result2, 7.2f);
        }
    }
}