using System;

namespace Data.Services;

public static class PasswordValidator
{
    public static void Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        if (password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters long.");

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("Password must contain at least one digit.");
    }
}
