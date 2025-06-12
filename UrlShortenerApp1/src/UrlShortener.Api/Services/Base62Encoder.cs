namespace UrlShortenerApp1.src.UrlShortener.Api.Services
{
    public class Base62Encoder
    {
        private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const int Base = 62;

        public static string Encode(long number)
        {
            if (number == 0) return Alphabet[0].ToString();

            var result = string.Empty;
            while (number > 0)
            {
                result = Alphabet[(int)(number % Base)] + result;
                number /= Base;
            }

            return result;
        }

        public static long Decode(string str)
        {
            long result = 0;
            for (int i = 0; i < str.Length; i++)
            {
                result = result * Base + Alphabet.IndexOf(str[i]);
            }
            return result;
        }

        public static string GenerateShortCode()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var random = new Random().Next(1000, 9999);
            return Encode(timestamp + random);
        }
    }
}
