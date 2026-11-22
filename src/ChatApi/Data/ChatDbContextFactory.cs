using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChatApi.Data;

public sealed class ChatDbContextFactory : IDesignTimeDbContextFactory<ChatDbContext>
{
    public ChatDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=localhost;Database=ChatHoTro;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<ChatDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ChatDbContext(options);
    }
}
