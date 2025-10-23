namespace Tawtheef.Application.Common.Interfaces.Services;

public interface IEnvironmentNameProvider
{
    // Development / Staging / Production
    string EnvironmentName { get; } 
}