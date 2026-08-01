using Airport.Features.Auth.Application.Login;

namespace Airport.UnitTests.Auth;

public sealed class LoginValidatorTests
{
    private readonly LoginValidator validator = new();

    [Fact]
    public void ValidateAcceptsUsernameWithSixtyCharacters()
    {
        var errors = validator.Validate(new LoginCommand(new string('a', 60), "password"));

        Assert.DoesNotContain(nameof(LoginCommand.Username), errors.Keys);
    }

    [Fact]
    public void ValidateRejectsUsernameWithMoreThanSixtyCharacters()
    {
        var errors = validator.Validate(new LoginCommand(new string('a', 61), "password"));

        Assert.Equal(
            ["El nombre de usuario no puede superar 60 caracteres."],
            errors[nameof(LoginCommand.Username)]);
    }
}
