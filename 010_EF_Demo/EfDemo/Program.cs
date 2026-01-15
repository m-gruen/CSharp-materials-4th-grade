using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("*** EfDemo ***");

var context = new DemoContextFactory().CreateDbContext([]);

// insert two students with 3-5 todos each
context.Students.AddRange(
    new Student
    {
        FirstName = "Max",
        LastName = "Mustermann",
        EarlyWarnings = ["Late submission of homework", "Low attendance"],
        Hobbies =
        [
            new Hobby { Title = "Football", MonthlyCost = 30.0m },
            new Hobby { Title = "Chess", MonthlyCost = 10.0m }
        ],
        TodoItems =
        [
            new TodoItem { Text = "Learn EF Core", IsDone = false },
            new TodoItem { Text = "Write demo app", IsDone = false },
            new TodoItem { Text = "Test app", IsDone = false }
        ]
    },
    new Student
    {
        FirstName = "Erika",
        LastName = "Musterfrau",
        EarlyWarnings = ["Missed project deadline", "Incomplete assignments"],
        Hobbies =
        [
            new Hobby { Title = "Painting", MonthlyCost = 25.0m },
            new Hobby { Title = "Cycling", MonthlyCost = 15.0m },
            new Hobby { Title = "Very very expensive hobby", MonthlyCost = 9999.99m }
        ],
        TodoItems =
        [
            new TodoItem { Text = "Read EF Core docs", IsDone = true },
            new TodoItem { Text = "Create sample data", IsDone = false }
        ]
    },
    new Student
    {
        FirstName = "John",
        LastName = "Doe",
        EarlyWarnings = [],
        Hobbies = [],
        TodoItems =
        [
            new TodoItem { Text = "Set up database", IsDone = true },
            new TodoItem { Text = "Implement features", IsDone = false },
            new TodoItem { Text = "Fix bugs", IsDone = false },
            new TodoItem { Text = "Deploy application", IsDone = false }
        ]
    }
);

await context.SaveChangesAsync();

var everything = await context.Students
    .Include(st => st.TodoItems)
    .ToListAsync();

/*
var s = everything.First();
s.EarlyWarnings = [];
s.TodoItems[0].IsDone = true;
s.TodoItems.Remove(s.TodoItems[1]);
s.TodoItems.Add(new TodoItem { Text = "Review code", IsDone = false });
*/

var avgCost = await context.Students
    .Where(s => s.FirstName == "Erika")
    .SelectMany(s => s.Hobbies.Select(h => h.MonthlyCost))
    .AverageAsync();

var groupBy = await context.Students
    .Select(st => new
    {
        Student = st,
        CountNotDone = st.TodoItems.Count(td => !td.IsDone)
    })
    .GroupBy(x => x.CountNotDone)
    .Select(g => new
    {
        OpenTodos = g.Key,
        TotalEarlyWarnings = g.Sum(x => x.Student.EarlyWarnings.Count)
    })
    .ToListAsync();

Console.WriteLine($"Average monthly cost of Erika's hobbies: {avgCost:C}");
Console.WriteLine($"Total number of early warnings for students with at least one pending todo: {groupBy}");

foreach (var student in everything)
{
    Console.WriteLine($"Student: {student.FirstName} {student.LastName}");
    Console.WriteLine($"  Early Warnings: {string.Join(", ", student.EarlyWarnings)}");
    Console.WriteLine("  Hobbies:");
    foreach (var hobby in student.Hobbies)
    {
        Console.WriteLine($"    - {hobby.Title} (Monthly Cost: {hobby.MonthlyCost:C})");
    }
    Console.WriteLine("  Todo Items:");
    foreach (var todo in student.TodoItems)
    {
        Console.WriteLine($"    - [{(todo.IsDone ? "X" : " ")}] {todo.Text}");
    }
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
