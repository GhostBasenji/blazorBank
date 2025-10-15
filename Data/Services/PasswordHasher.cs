using System.Security.Cryptography;
using System.Text;

namespace Data.Services
{
    public static class PasswordHasher
    {
        // Хэширование пароля с солью
        public static string HashPassword(string password)
        {
            // Генерируем соль
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Хэшируем пароль с солью
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            // Объединяем соль и хэш
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            // Конвертируем в Base64 строку
            return Convert.ToBase64String(hashBytes);
        }

        // Проверка пароля
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Конвертируем обратно в байты
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                // Извлекаем соль
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                // Хэшируем введенный пароль с той же солью
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] hash = pbkdf2.GetBytes(32);

                // Сравниваем хэши
                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}