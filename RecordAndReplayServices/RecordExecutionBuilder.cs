using System;

namespace RecordAndReplayServices
{
    public class RecordExecutionBuilder
    {
        private readonly RecordExecution _recordExecution;

        private RecordExecutionBuilder()
        {
            _recordExecution = new RecordExecution();
        }
        public static RecordExecutionBuilder Create()
        {
            return new RecordExecutionBuilder();
        }

        public RecordExecutionBuilder AddServiceToRecord<TServiceInterface>(TServiceInterface service,
            out TServiceInterface serviceMock) where TServiceInterface : class
        {
            if (!_recordExecution.InterceptService(service,out serviceMock))
            {
                throw new ArgumentException($"Service to record already added: {typeof(TServiceInterface)}");
            }

            return this;
        }

        public RecordExecutionBuilder ValidateServiceReturnValue<TServiceInterface>()
        {
            _recordExecution.AddServiceToValidateReturnValue<TServiceInterface>();
            return this;
        }

        public RecordExecutionBuilder ValidateServiceCalledWithArgs<TServiceInterface>()
        {
            _recordExecution.AddServiceToValidateCalledWithArgs<TServiceInterface>();
            return this;
        }

        public RecordExecution Build()
        {
            return _recordExecution;
        }
    }
}
