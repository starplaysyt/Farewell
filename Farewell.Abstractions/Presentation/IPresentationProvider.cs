namespace Farewell.Abstractions.Presentation;

public interface IPresentationProvider
{
    string Present(object target, PresentationContext context = default);
    bool CanPresent(Type type, PresentationContext context = default);
}