using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Infrastructure.Data
{
    public class TaskFlowDbContext : DbContext
    {
        public TaskFlowDbContext(DbContextOptions options) : base(options)
        {
            
        }
        
    }
}