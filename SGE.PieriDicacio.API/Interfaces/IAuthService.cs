using SGE.PieriDicacio.API.DTOs.Auth;

namespace SGE.PieriDicacio.API.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
