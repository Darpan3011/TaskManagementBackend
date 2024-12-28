using System.Security.Cryptography;

namespace finalSubmissionDotNet.Helper
{
    public static class GenerateRefresh
    {
        public static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[64];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);

        }
    }
}
