namespace ClassLibrary1
{
    public class Calculator
    {
        public int Add(int x, int y)
        {
            return x + y;
        }

        public decimal Divide(decimal x, decimal y)
        {
            if (y == 0)
            {
                throw new ArgumentException("Divider can not be zero", nameof(y));
            }
            return x / y;
        }
    }
}