using Elastic.Clients.Elasticsearch;
using Play.Catalog.Entities;
using Play.Catalog.Services;

namespace Play.Catalog.Tests.Services;

public class ElasticsearchServiceTests
{
    [Fact]
    public void ElasticsearchService_ShouldBeCreatedWithClient()
    {
        // Arrange
        var mockClient = new Mock<ElasticsearchClient>();

        // Act
        var service = new ElasticsearchService(mockClient.Object);

        // Assert
        service.Should().NotBeNull();
        service.Should().BeAssignableTo<IElasticsearchService>();
    }

    [Fact]
    public void Item_ShouldHaveRequiredProperties()
    {
        // Arrange & Act
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Description = "Test Description",
            Price = 10.99m,
            CreatedDate = DateTimeOffset.UtcNow
        };

        // Assert
        item.Id.Should().NotBeEmpty();
        item.Name.Should().NotBeNullOrEmpty();
        item.Description.Should().NotBeNullOrEmpty();
        item.Price.Should().BeGreaterThan(0);
        item.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Item_ShouldAllowDefaultValues()
    {
        // Arrange & Act
        var item = new Item();

        // Assert
        item.Id.Should().BeEmpty();
        item.Name.Should().BeEmpty();
        item.Description.Should().BeEmpty();
        item.Price.Should().Be(0);
        item.CreatedDate.Should().Be(default(DateTimeOffset));
    }

    [Fact]
    public void Item_ShouldSupportPropertySetting()
    {
        // Arrange
        var item = new Item();
        var id = Guid.NewGuid();
        var createdDate = DateTimeOffset.UtcNow;

        // Act
        item.Id = id;
        item.Name = "Test";
        item.Description = "Description";
        item.Price = 99.99m;
        item.CreatedDate = createdDate;

        // Assert
        item.Id.Should().Be(id);
        item.Name.Should().Be("Test");
        item.Description.Should().Be("Description");
        item.Price.Should().Be(99.99m);
        item.CreatedDate.Should().Be(createdDate);
    }

    [Theory]
    [InlineData("Potion", "Restores HP", 5.99)]
    [InlineData("Sword", "A weapon", 25.50)]
    [InlineData("Shield", "Protection", 15.00)]
    public void Item_ShouldAcceptVariousValidInputs(string name, string description, decimal price)
    {
        // Arrange & Act
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            Price = price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        // Assert
        item.Name.Should().Be(name);
        item.Description.Should().Be(description);
        item.Price.Should().Be(price);
    }

    [Fact]
    public void Item_ShouldSupportNegativePrice()
    {
        // Arrange & Act
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Debt",
            Description = "Negative value",
            Price = -10.00m,
            CreatedDate = DateTimeOffset.UtcNow
        };

        // Assert
        item.Price.Should().BeLessThan(0);
    }

    [Fact]
    public void Item_ShouldSupportVeryLargePrices()
    {
        // Arrange & Act
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Legendary Item",
            Description = "Very expensive",
            Price = decimal.MaxValue,
            CreatedDate = DateTimeOffset.UtcNow
        };

        // Assert
        item.Price.Should().Be(decimal.MaxValue);
    }
}
