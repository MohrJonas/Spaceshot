namespace Mohr.Jonas.Spaceshot.Spaceshot.Services;

[AttributeUsage(AttributeTargets.Class)]
public class PriorityAttribute(Priority priority) : Attribute
{
    public Priority Priority { get; } = priority;
}