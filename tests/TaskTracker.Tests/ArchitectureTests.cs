using TaskTracker.Application.Validation;
using TaskTracker.Domain;

namespace TaskTracker.Tests;

/// <summary>Правило залежностей: стрілки спрямовані всередину, до домену.</summary>
public class ArchitectureTests
{
    [Fact]
    public void Domain_DependsOnNoOtherProject()
    {
        IEnumerable<string> links = typeof(BoardTask).Assembly.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(n => n.StartsWith("TaskTracker", StringComparison.Ordinal)
                || n.Contains("Logging", StringComparison.Ordinal));

        Assert.Empty(links);
    }

    [Fact]
    public void Application_DependsOnlyOnDomain()
    {
        IEnumerable<string> links = typeof(InputParser).Assembly.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .Where(n => n.StartsWith("TaskTracker", StringComparison.Ordinal));

        Assert.Equal(["TaskTracker.Domain"], links);
    }
}
