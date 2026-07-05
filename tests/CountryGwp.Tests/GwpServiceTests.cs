using CountryGwp.API.Models;
using CountryGwp.API.Repositories;
using CountryGwp.API.Services;
using Moq;
using NUnit.Framework;

namespace CountryGwp.Tests;

[TestFixture]
public sealed class GwpServiceTests
{
    private Mock<IGwpRepository> _repositoryMock;
    private GwpService _gwpService;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IGwpRepository>();
        _gwpService = new GwpService(_repositoryMock.Object);
    }

    [Test]
    public async Task GetAverageGwpAsync_CalculatesCorrectAverageForValidData()
    {
        // Arrange
        var country = "ae";
        var lobs = new[] { "transport" };

        var record = new GwpRecord
        {
            Country = country,
            VariableId = "gwp",
            VariableName = "Direct Premiums",
            LineOfBusiness = "transport",
            YearValues = new Dictionary<int, decimal?>
            {
                { 2008, 100m },
                { 2009, 200m },
                { 2010, 300m },
                { 2011, 400m },
                { 2012, 500m },
                { 2013, 600m },
                { 2014, 700m },
                { 2015, 800m }
            }
        };

        _repositoryMock
            .Setup(r => r.GetGwpDataAsync(country, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new[] { record });

        // Act
        var result = await _gwpService.GetAverageGwpAsync(country, lobs);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ContainsKey("transport"), Is.True);
        Assert.That(result["transport"], Is.EqualTo(450.0m)); // (100+...+800)/8 = 450
    }

    [Test]
    public async Task GetAverageGwpAsync_IgnoresNullYearsWhenCalculatingAverage()
    {
        // Arrange
        var country = "ae";
        var lobs = new[] { "property" };

        var record = new GwpRecord
        {
            Country = country,
            VariableId = "gwp",
            VariableName = "Direct Premiums",
            LineOfBusiness = "property",
            YearValues = new Dictionary<int, decimal?>
            {
                { 2008, 100m },
                { 2009, null }, // Missing year
                { 2010, 300m },
                { 2011, null }, // Missing year
                { 2012, 500m },
                { 2013, 700m },
                { 2014, null },
                { 2015, 900m }
            }
        };

        _repositoryMock
            .Setup(r => r.GetGwpDataAsync(country, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(new[] { record });

        // Act
        var result = await _gwpService.GetAverageGwpAsync(country, lobs);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ContainsKey("property"), Is.True);
        // Available: 100, 300, 500, 700, 900. Count = 5. Sum = 2500. Average = 500.
        Assert.That(result["property"], Is.EqualTo(500.0m));
    }
}
