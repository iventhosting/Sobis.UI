namespace Sobis.UI;

/// <summary>
/// Authorization abstraction used by Sobis.UI components (such as SbsButton and SbsGrid)
/// to check claim-based visibility and execution permissions without coupling to host auth implementations.
/// </summary>
public interface ISbsAuthorizationProvider
{
    Task<bool> HasClaimAsync(string claimCode);
    Task<bool> HasAnyClaimAsync(params string[] claimCodes);
}
