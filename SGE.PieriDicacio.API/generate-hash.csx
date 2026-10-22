using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        string password = "Admin123*";
        string salt = BCrypt.GenerateSalt(12);
        string hash = BCrypt.HashPassword(password, salt);
        
        Console.WriteLine($"===== HASH BCrypt GENERADO =====");
        Console.WriteLine(hash);
        Console.WriteLine("===============================");
        Console.WriteLine($"Verificación: {BCrypt.Verify(password, hash)}");
    }
}
