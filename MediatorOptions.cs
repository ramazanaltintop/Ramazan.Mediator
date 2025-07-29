using System.Reflection;

namespace Ramazan.Mediator;

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
