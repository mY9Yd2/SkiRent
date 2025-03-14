using System.Security.Cryptography;

using SkiRent.Shared.Validators.Common.Users;

namespace SkiRent.Desktop.Utils
{
    public static class PasswordGeneratorHelper
    {
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string AllChars = Uppercase + Lowercase + Digits;

        public static string GeneratePassword(int minLength = 8, int maxLength = 16)
        {
            int length = GetRandomNumber(minLength, maxLength);
            char[] password = new char[length];

            using var rng = RandomNumberGenerator.Create();

            password[0] = Uppercase[GetRandomNumber(rng, Uppercase.Length)];
            password[1] = Lowercase[GetRandomNumber(rng, Lowercase.Length)];
            password[2] = Digits[GetRandomNumber(rng, Digits.Length)];


            for (int i = 3; i < length; i++)
            {
                password[i] = AllChars[GetRandomNumber(rng, AllChars.Length)];
            }

            string shuffledPassword = ShufflePassword(password, rng);

            var validator = new PasswordValidator();

            if (!validator.Validate(shuffledPassword).IsValid)
            {
                return GeneratePassword();
            }

            return shuffledPassword;
        }

        private static int GetRandomNumber(int min, int max)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] randomNumber = new byte[4];
            rng.GetBytes(randomNumber);
            int value = BitConverter.ToInt32(randomNumber, 0) & int.MaxValue;
            return min + (value % (max - min + 1));
        }

        private static int GetRandomNumber(RandomNumberGenerator rng, int max)
        {
            byte[] randomNumber = new byte[1];

            do
            {
                rng.GetBytes(randomNumber);
            } while (randomNumber[0] >= 256 - (256 % max));

            return randomNumber[0] % max;
        }

        private static string ShufflePassword(char[] password, RandomNumberGenerator rng)
        {
            for (int i = password.Length - 1; i > 0; i--)
            {
                int j = GetRandomNumber(rng, i + 1);
                (password[i], password[j]) = (password[j], password[i]);
            }
            return new string(password);
        }
    }
}
