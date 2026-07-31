using Airport.Features.Auth.Infrastructure.Security;

namespace Airport.UnitTests.Auth;

public sealed class LegacyMd5PasswordVerifierTests
{
    private readonly LegacyMd5PasswordVerifier verifier = new();

    [Fact]
    public void VerifyAcceptsMatchingPassword()
    {
        Assert.True(verifier.Verify("Lauren", "6c16362f64973441684cfb1ce82ec7b9"));
    }

    [Theory]
    [InlineData("incorrect", "6c16362f64973441684cfb1ce82ec7b9")]
    [InlineData("Lauren", "not-a-valid-hash")]
    public void VerifyRejectsInvalidCredentials(string password, string storedHash)
    {
        Assert.False(verifier.Verify(password, storedHash));
    }
}
