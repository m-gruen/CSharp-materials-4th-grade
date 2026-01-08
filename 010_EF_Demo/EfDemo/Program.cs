using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("*** EfDemo ***");

var context = new DemoContextFactory().CreateDbContext([]);

try
{
    List<TodoItem> allTodoItems = await context.TodoItems.ToListAsync();
    Console.WriteLine($"Todo Count: {allTodoItems.Count}");
}
catch (PostgresException ex) when (ex.SqlState == "42P01")
{
    Console.WriteLine("Fails initially, because tables have not yet been created");
}

public sealed class DemoContextFactory : IDesignTimeDbContextFactory<DemoContext>
{
    public DemoContext CreateDbContext(string[] _)
    {
        DbContextOptionsBuilder<DemoContext> optionsBuilder = new DbContextOptionsBuilder<DemoContext>()
            .UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=postgres",
            options => options.UseNodaTime())
            .ConfigureWarnings(warnings => 
                warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning))
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .LogTo(Console.WriteLine, LogLevel.Information);

        return new DemoContext(optionsBuilder.Options);
    }
}
