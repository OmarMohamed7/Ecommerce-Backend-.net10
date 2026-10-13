using E_Commerce.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Common;

namespace E_Commerce.Features.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    IValidator<RegisterDto> registerDtoValidator,
    IValidator<LoginDto> loginValidator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var validationResult = await loginValidator.ValidateAsync(loginDto);

        if (!validationResult.IsValid) return BadRequest(ApiResponse<User>.Fail(validationResult.ToApiErrors()));

        var res = await authService.LoginAsync(loginDto);

        if (res == null) return BadRequest(ApiResponse<User>.Fail("Invalid username or password"));

        return Ok(res);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var validationResult = await registerDtoValidator.ValidateAsync(registerDto);

        if (!validationResult.IsValid) return BadRequest(ApiResponse<User>.Fail(validationResult.ToApiErrors()));

        var res = await authService.RegisterAsync(registerDto);

        if (res == null) return BadRequest(ApiResponse<User>.Fail("Email already in use"));

        return Ok(res);
    }
}