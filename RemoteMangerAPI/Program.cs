using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RemoteMangerAPI;
using RemoteMangerAPI.DataBase;
using RemoteMangerAPI.Middleware;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// ���JWT��֤����
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CM E-Commerce API", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Auth Bearer Scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement { { securitySchema, new[] { "Bearer" } } };
    c.AddSecurityRequirement(securityRequirement);
});
//ע�������ģ�AOP������Ի�ȡIOC����������ֳɿ�ܱ���Furion���Բ�д��һ��
builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<IUserContext, HttpUserContext>();
builder.Services.AddDbContextPool<RemoteMangerDBContext>((options) =>
{
    options.UseNpgsql("Host =localhost;Port=5432;Database=RemoteManger;Username=postgres;Password=1340896123");
    options.AddInterceptors();
    var buildServices = builder.Services.BuildServiceProvider();

    options.AddInterceptors(new AuditingInterceptor(buildServices.GetService<IHttpContextAccessor>())); // ��ѡ����ѯ������ 
});

builder.Services.AddMemoryCache();

builder.Services.AddMvc();

var app = builder.Build();
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<RemoteMangerDBContext>();
    ///await context.Database.EnsureCreatedAsync();
    //await context.Database.MigrateAsync();

}
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "Grapefruit.VuCore API V1.0");
    });
}

app.UseHttpsRedirection();

// ע���Զ����м��
app.UseMiddleware<RefreshTokenMiddleware>(builder.Configuration["Jwt:Issuer"], builder.Configuration["Jwt:Issuer"], builder.Configuration["Jwt:Audience"], TimeSpan.FromHours(24));


app.UseAuthorization();



app.MapControllers();


app.Run();

