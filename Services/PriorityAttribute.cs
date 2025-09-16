namespace Mohr.Jonas.Spaceshot.Services;

[AttributeUsage(AttributeTargets.Class)]
public class PriorityAttribute(Priority priority) : Attribute
{
    public Priority Priority { get; } = priority;
}