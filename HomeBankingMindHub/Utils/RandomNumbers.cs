namespace HomeBankingMindHub.Utils
{
    public static class RandomNumbers
    {
        private static readonly Random _random = new Random();
        
        public static int GenerateRandomInt(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public static long GenerateRandomLong(long minValue, long maxValue)
        {
            return (long)(_random.NextDouble() * (maxValue - minValue) + minValue);
        }
    }
}
