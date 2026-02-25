using FluentValidation.TestHelper;
using LifeTrackerPro.Application.Features.Auth.Commands;

namespace LifeTrackerPro.UnitTests.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var cmd = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "StrongP@ss1",
            FullName = "Test User"
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("", "Password is required.")]
    [InlineData("short", "Password must be at least 8 characters.")]
    public void Should_Fail_When_Password_Invalid(string password, string expectedMessage)
    {
        var cmd = new RegisterCommand
        {
            Email = "user@example.com",
            Password = password,
            FullName = "Test User"
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage(expectedMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Should_Fail_When_Email_Invalid(string email)
    {
        var cmd = new RegisterCommand
        {
            Email = email,
            Password = "StrongP@ss1",
            FullName = "Test"
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Fail_When_FullName_Empty()
    {
        var cmd = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "StrongP@ss1",
            FullName = ""
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
              .WithErrorMessage("Full name is required.");
    }

    [Fact]
    public void Should_Fail_When_FullName_Too_Long()
    {
        var cmd = new RegisterCommand
        {
            Email = "user@example.com",
            Password = "StrongP@ss1",
            FullName = new string('A', 201)
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.FullName)
              .WithErrorMessage("Full name must not exceed 200 characters.");
    }
}

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var cmd = new LoginCommand { Email = "user@example.com", Password = "secret123" };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_Email_Empty()
    {
        var cmd = new LoginCommand { Email = "", Password = "secret123" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Fail_When_Password_Empty()
    {
        var cmd = new LoginCommand { Email = "user@example.com", Password = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
