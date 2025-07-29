using System.Reflection;

namespace CleanArchitecture_2025.Application.Abstractions.Messaging;

public sealed class MediatorOptions
{
    internal List<Assembly> Assemblies { get; set; } = new();
    internal List<Type> PipelineBehaviors { get; set; } = new();

    public void AddRegisterAssemblies(params Assembly[] assemblies)
    {
        Assemblies.AddRange(assemblies);
    }

    public void AddOpenBehavior(Type behaviorType)
    {
        PipelineBehaviors.Add(behaviorType);
    }
}
