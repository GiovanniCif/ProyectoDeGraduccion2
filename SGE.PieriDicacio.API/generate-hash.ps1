# Script para generar hash BCrypt
# Requiere: dotnet-script o ejecutar dentro del proyecto

Write-Host "Generando hash para 'Admin123*'..." -ForegroundColor Cyan

# Crear un pequeño programa temporal
$code = @'
using System;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        string password = "Admin123*";
        string hash = BCrypt.HashPassword(password, BCrypt.GenerateSalt(12));
        Console.WriteLine($"Hash generado: {hash}");
        Console.WriteLine($"Verificación: {BCrypt.Verify(password, hash)}");
    }
}
'@

# Guardar y compilar temporalmente
$tempFile = "temp_hash.cs"
$code | Out-File -FilePath $tempFile -Encoding UTF8

Write-Host "`nPara generar el hash, ejecuta este comando en el proyecto:" -ForegroundColor Yellow
Write-Host "dotnet run --project . -- --generate-hash" -ForegroundColor Green
Write-Host "`nO borra el usuario admin de la BD y deja que el sistema lo recree automáticamente." -ForegroundColor Cyan

# Limpiar
Remove-Item $tempFile -ErrorAction SilentlyContinue
