namespace Vital;

/// <summary>
/// This interface serves as a marker for the API assembly. 
/// It does not contain any members, its sole purpose is to provide a means to easily identify the assembly that represents the API layer of the application.
/// Components can take a dependence on this marker interface to implicitly signal that they belong to or interact with the API layer.
/// </summary>
public interface IApiAssemblyMarker;
