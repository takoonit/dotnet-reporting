using Microsoft.AspNetCore.Mvc;
using Moq;
using Redis.OM;
using Report_MS.Controllers;
using Report_MS.Models;
using Report_MS.Repository;
using Xunit;

namespace Report_MS.Tests;

public class ProfileControllersTests
{
    private readonly ProfileController _controller;
    private readonly Mock<RedisConnectionProvider> _redisConnectionProviderMock;
    private readonly Mock<IMongoRepository<Report>> _reportRepositoryMock;

    public ProfileControllersTests()
    {
        _redisConnectionProviderMock = new Mock<RedisConnectionProvider>();
        _reportRepositoryMock = new Mock<IMongoRepository<Report>>();
        _controller = new ProfileController(_redisConnectionProviderMock.Object, _reportRepositoryMock.Object);
    }

    [Fact]
    public void AddProfile_ReturnsCreatedResult()
    {
        // Arrange
        var profile = new Profile { Id = "1", FirstName = "John", LastName = "Doe" };

        // Act
        var result = _controller.AddProfile(profile);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(profile, createdResult.Value);
    }

    //[Fact]
    //public void FilterBySkill_ReturnsProfilesWithMatchingSkills()
    //{
    //    // Arrange
    //    var profiles = new List<Profile>
    //    {
    //        new Profile { Id = "1", Skills = new[] { "C#", "JavaScript" } },
    //        new Profile { Id = "2", Skills = new[] { "Java", "Python" } },
    //        new Profile { Id = "3", Skills = new[] { "C#", "Python" } }
    //    };
    //    var redisCollection = new RedisCollection<int>(_redisConnectionProviderMock, "collectionKey");
    //    foreach (var item in list)
    //    {
    //        redisCollection.Add(item);
    //    }
    //    var minAge = 20;
    //    var maxAge = 30;
    //    _redisConnectionProviderMock.Setup(expression: x =>x.RedisCollection<Profile>(100)).Returns(new RedisCollection<Profile>(profiles))

    //    // Act
    //    var result = _controller.FilterBySkill("C#");

    //    // Assert
    //    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    //    var filteredProfiles = Assert.IsAssignableFrom<List<Profile>>(okResult.Value);
    //    Assert.Equal(2, filteredProfiles.Count);
    //    Assert.Equal("1", filteredProfiles[0].Id);
    //    Assert.Equal("3", filteredProfiles[1].Id);
    //}

    // Add more unit tests for other controller actions...
}