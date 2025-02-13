using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RemoteMangerAPI.Entitys;
using System;
using System.Linq;
namespace RemoteMangerAPI.DataBase;
public class AuditingInterceptor : SaveChangesInterceptor
{
    private readonly string _currentUserId;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditingInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        _currentUserId = GetCurrentUserId();
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var entries = eventData.Context.ChangeTracker.Entries<Baseentity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = _currentUserId;
                entry.Entity.CreatedTime = DateTime.UtcNow;
                entry.Entity.ModifiedById = _currentUserId;
                entry.Entity.ModifiedTime = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedById = _currentUserId;
                entry.Entity.ModifiedTime = DateTime.UtcNow;
            }
        }

        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        var entries = eventData.Context.ChangeTracker.Entries<Baseentity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedById = _currentUserId;
                entry.Entity.CreatedTime = DateTime.UtcNow;
                entry.Entity.ModifiedById = _currentUserId;
                entry.Entity.ModifiedTime = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedById = _currentUserId;
                entry.Entity.ModifiedTime = DateTime.UtcNow;
            }
        }
        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }



    private string GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            // 假设用户 ID 存储在 Claims 中，可以根据实际情况调整
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            return userIdClaim?.Value;
        }
        return null; // 或者返回一个默认值，例如 "Anonymous"
    }
}