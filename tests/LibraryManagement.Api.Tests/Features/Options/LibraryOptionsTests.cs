using LibraryManagement.Api.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace LibraryManagement.Api.Tests.Features.Options;

public class LibraryOptionsTests(ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    [Fact]
    public void Valid_Options_Pass_Validation()
    {
        var options = factory.Services
            .GetRequiredService<IOptions<LibraryOptions>>()
            .Value;

        Assert.Equal(14, options.LoanDays);
        Assert.Equal(5, options.MaxActiveLoans);
        Assert.Equal(2, options.MaxRenewals);
        Assert.Equal(5m, options.FinePerDay);
        Assert.Equal(48, options.ReservationHoldHours);
    }

    [Fact]
    public void Options_Can_Be_Override()
    {
        using var customFactory = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("Library:MaxActiveLoans", "22"));

        var options = customFactory.Services
            .GetRequiredService<IOptions<LibraryOptions>>()
            .Value;

        Assert.Equal(22, options.MaxActiveLoans);
    }

    [Theory]
    [InlineData("Library:LoanDays", "999", "LoanDays")]
    [InlineData("Library:MaxActiveLoans", "999", "MaxActiveLoans")]
    [InlineData("Library:MaxRenewals", "999", "MaxRenewals")]
    [InlineData("Library:ReservationHoldHours", "999", "ReservationHoldHours")]
    public void Invalid_Options_Contain_Field_Name(
    string settingKey,
    string invalidValue,
    string expectedFieldName)
    {
        using var broken = factory.WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        [settingKey] = invalidValue
                    })));

        var exception = Assert.Throws<OptionsValidationException>(
            () => broken.CreateClient());

        Assert.Contains(expectedFieldName, exception.Message);
    }
}
