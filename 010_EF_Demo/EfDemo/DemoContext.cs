using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

public sealed class DemoContext(DbContextOptions<DemoContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTodo(modelBuilder);
        ConfigureStudent(modelBuilder);

        /* 
        Alternative fluent API configuration:
        
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();
        });
        */
    }

    private static void ConfigureStudent(ModelBuilder modelBuilder)
    {
        var student = modelBuilder.Entity<Student>();
        student.HasKey(st => st.Id);
        student.Property(st => st.Id).ValueGeneratedOnAdd();
        student.ComplexCollection<Hobby>(st => st.Hobbies).ToJson();
        student.HasMany(st => st.TodoItems)
            .WithOne(td => td.Student)
            .HasForeignKey(td => td.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureTodo(ModelBuilder modelBuilder)
    {
        var todoItem = modelBuilder.Entity<TodoItem>();
        todoItem.HasKey(td => td.Id);
        todoItem.Property(td => td.Id).ValueGeneratedOnAdd();
        todoItem.Property(td => td.Text).HasMaxLength(200);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
    }
}

public class Student
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public List<string> EarlyWarnings { get; set; } = [];
    public List<Hobby> Hobbies { get; set; } = [];
    public List<TodoItem> TodoItems { get; set; } = [];
}

public class Hobby
{
    public required string Title { get; set; }
    public decimal MonthlyCost { get; set; }
}

public class TodoItem
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public bool IsDone { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
}
