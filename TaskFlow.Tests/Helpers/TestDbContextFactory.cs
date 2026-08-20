using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Tests.Helpers;

public static class TestDbContextFactory
{
    public static TaskFlowDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TaskFlowDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        var context = new TaskFlowDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }
}
