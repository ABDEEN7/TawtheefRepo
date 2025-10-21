using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Data;

public class TawtheefDbContext(DbContextOptions<TawtheefDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options), ITawtheefDbContext
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
