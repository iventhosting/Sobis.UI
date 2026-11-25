namespace Sobis.UI;

/// <summary>
/// Default permissive authorization provider that allows all claims when the host application
/// does not register a custom <see cref="ISbsAuthorizationProvider"/>.
/// </summary>
public sealed class AllowAllSbsAuthorizationProvider : ISbsAuthorizationProvider
{
    public Task<bool> HasClaimAsync(string claimCode) => Task.FromResult(true);

    public Task<bool> HasAnyClaimAsync(params string[] claimCodes) => Task.FromResult(true);
}
