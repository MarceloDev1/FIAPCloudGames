using FIAPCloudGames.Models.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractToken(context.Request);

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var (isValid, principal) = ValidateToken(token);
                if (isValid && principal != null)
                {
                    context.User = principal;
                    AttachAdditionalUserInfo(context, principal);
                }
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning($"Token expirado: {ex.Message}");
                context.Response.Headers.Add("Token-Expired", "true");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao validar token: {ex.Message}");
            }
        }

        await _next(context);
    }

    private string ExtractToken(HttpRequest request)
    {
        return request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()
               ?? request.Cookies["access_token"];
    }

    private (bool isValid, ClaimsPrincipal principal) ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out _);

        return (true, principal);
    }

    private void AttachAdditionalUserInfo(HttpContext context, ClaimsPrincipal principal)
    {
        var userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var userRoles = principal.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        context.Items["UserId"] = userId;
        context.Items["UserRoles"] = userRoles;
    }
}