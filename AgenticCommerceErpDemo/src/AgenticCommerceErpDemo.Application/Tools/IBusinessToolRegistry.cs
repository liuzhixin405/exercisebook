namespace AgenticCommerceErpDemo.Application.Tools;

public interface IBusinessToolRegistry
{
    object Invoke(string toolName, Dictionary<string, object?> arguments);
}
