using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace RemoteMangerAPI.Middleware
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly TimeSpan _tokenLifetime;

        public RefreshTokenMiddleware(RequestDelegate next, string jwtKey, string jwtIssuer, string jwtAudience, TimeSpan tokenLifetime)
        {
            _next = next;
            _jwtKey = jwtKey;
            _jwtIssuer = jwtIssuer;
            _jwtAudience = jwtAudience;
            _tokenLifetime = tokenLifetime;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 获取请求头中的 JWT Token
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    // 验证 JWT Token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                    // 获取原始 Token 的过期时间
                    var expiration = ((JwtSecurityToken)validatedToken).ValidTo;

                    // 如果 Token 即将过期（例如，剩余时间少于 1 小时），则刷新 Token
                    if (expiration < DateTime.UtcNow.AddHours(1))
                    {
                        // 创建新的 Token
                        var claims = principal.Claims.ToList();
                        var newToken = new JwtSecurityToken(
                            issuer: _jwtIssuer,
                            audience: _jwtAudience,
                            claims: claims,
                            expires: DateTime.UtcNow.Add(_tokenLifetime),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)), SecurityAlgorithms.HmacSha256)
                        );

                        // 将新的 Token 添加到响应头中
                        context.Response.Headers.Add("Authorization", $"Bearer {new JwtSecurityTokenHandler().WriteToken(newToken)}");
                    }
                }
                catch (Exception ex)
                {
                    // 如果 Token 验证失败，记录日志或返回 401 Unauthorized
                    context.Response.StatusCode = 401;
                    return;
                }
            }

            // 继续执行下一个中间件
            await _next(context);
        }
    }
}
