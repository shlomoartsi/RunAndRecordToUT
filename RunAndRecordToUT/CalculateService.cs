namespace RunAndRecordToUT
{
    public interface INumber<T> 
    {
        T Value { get;  }
        T AddNumber(INumber<T> n);
    }

    public class Natural : INumber<int>
    {
        public Natural(int value)
        {
            Value = value;
        }
        //public Natural(int value)
        //{
        //    _value = value;
        //}
        public int Value { get; }
        public int AddNumber(INumber<int> n)
        {
            return Value + n.Value;
        }
    }

    public class Real : INumber<float>
    {
        public Real(float value)
        {
            Value = value;
        }
        public float Value { get; }
        public float AddNumber(INumber<float> n)
        {
            return n.Value + Value;
        }
    }

    public interface ICalculatorService
    {
        float Add(float a, float b);

        T Add<T>(INumber<T> a, INumber<T> b);

        void SaveMemory(float a);
        float GetMemory();
        float Memory { get; }
    }

    public class CalculatorService : ICalculatorService
    {
        public float Add(float a, float b) => a + b;
        public T Add<T>(INumber<T> a, INumber<T> b)
        {
            return a.AddNumber(b);
        }

        public void SaveMemory(float a) => Memory = a;
        public float GetMemory() => Memory;

        public float Memory { get; private set; }
    }

}

