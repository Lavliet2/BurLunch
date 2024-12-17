using BurLunch.AuthAPI.Utils; 

namespace BurLunch.AuthAPI.Services
{
    public class AuthenticationService
    {
        public bool VerifyPassword(string inputPassword, string storedHash)
        {
            var inputHash = PasswordHasher.HashPassword(inputPassword);
            return inputHash == storedHash;
        }
    }
}
