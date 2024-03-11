# RecordAndReplayServices Library

The RecordAndReplayServices library is a .NET library that provides functionality to record and replay method invocations. It is useful for creating mock objects in unit tests, among other things.

## Features

- Record method invocations including the method name, arguments, and return value.
- Enables creating a mock of a service and replay the same results when calling the same methods.
- Compare method invocations based on method name and arguments and returns the recorded return/out values. 
- Support for multi-targeting .NET Framework 4.6.1 and .NET 6.0.

## Dependencies

- Castle.Core (5.1.1)
- Newtonsoft.Json (13.0.3)
- System.Text.Json (6.0.8)

## Usage

The RecordAndReplayServices library is used to record and replay method invocations. Here are some examples of how to use the library based on the tests in `RecordAndReplayBasicTests.cs`:

### Basic Record and Replay
```
        [TestMethod]
        public void TestRecordAndReplayBasic()
        {
            //arrange
            var calculatorService = new CalculatorService();

            var recordExecution = RecordExecutionBuilder.Create()
                .AddServiceToRecord<ICalculatorService>(calculatorService, out var calculatorRecordMock)
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
            var result3 = calculatorReplayMock.Add(3, 4);
            calculatorReplayMock.SaveMemory(4.33f);
            var memRestored = calculatorReplayMock.Memory;

            //assert
            Assert.AreEqual(result1, 5);
            Assert.AreEqual(result2, 7.2f);
            //the result is not 7 because the replay options are not set to compare the params (AddComparingParamsToService
            Assert.AreEqual(result3, 5);
            Assert.AreEqual(memRestored, 4.33f);
        }
```


### Record and Replay Considering Parameters Value
```
        [TestMethod]
        public void TestRecordAndReplay_ConsiderParamsValue()
        {
            //arrange
            var calculatorService = new CalculatorService();

            var recordExecution = RecordExecutionBuilder.Create()
                .AddServiceToRecord<ICalculatorService>(calculatorService, out var calculatorRecordMock)
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
            var calculatorReplayMock = new ReplayMockServiceFactory(replayContext, replayOptions).
                CreateReplayMock<ICalculatorService>();

            //replay service in the reserve to real execution
            var result1 = calculatorReplayMock.Add(new Real(4.4f), new Real(3.8f));
            var result2 = calculatorReplayMock.Add(new Real(3.4f), new Real(3.8f));

            //assert
            Assert.AreEqual(result1, 8.2f);
            Assert.AreEqual(result2, 7.2f);
        }
```


### Record and Replay with Out Parameters

```
        [TestMethod]
        public void TestRecordAndReplay_OutParamValue()
        {
            //arrange
            var calculatorService = new CalculatorService();

            var recordExecution = RecordExecutionBuilder.Create()
                .AddServiceToRecord<ICalculatorService>(calculatorService, out var calculatorRecordMock)
                .Build();

            //act
            // Intercept function calls and save arguments/return values
            calculatorRecordMock.Add(3.4f, 4.5f, out var result1Real);
            calculatorRecordMock.Add(4f, 4f, out var result2Real);

            var json = recordExecution.ToJson();

            var invocationContextFromJson = RecordInvocationContext.FromJson(json);

            var replayContext = new ReplayInvocationContext(invocationContextFromJson);
            var replayOptions = new ReplayOptions();
            replayOptions.AddComparingParamsToService<ICalculatorService>();
            var calculatorReplayMock = new ReplayMockServiceFactory(replayContext, replayOptions).
                CreateReplayMock<ICalculatorService>();

            //replay service in the reserve to real execution
            calculatorReplayMock.Add(3.4f, 4.5f, out var result1);
            calculatorReplayMock.Add(4f, 4f, out var result2);

            //assert
            Assert.AreEqual(result1, result1Real);
            Assert.AreEqual(result2, result2Real);
        }
```
These examples show how to record and replay method invocations with the RecordAndReplayServices library. The `RecordExecutionBuilder` class is used to record method invocations, and the `ReplayMockServiceFactory` class is used to replay them. The `ReplayOptions` class is used to specify options for the replay, such as whether to compare parameters values.

