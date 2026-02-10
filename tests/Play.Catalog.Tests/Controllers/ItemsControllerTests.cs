using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Play.Catalog;
using Play.Catalog.Entities;
using Play.Catalog.Services;

namespace Play.Catalog.Tests.Controllers;

public class ItemsControllerTests
{
    private readonly Mock<IElasticsearchService> _mockElasticsearchService;
    private readonly ItemsController _controller;

    public ItemsControllerTests()
    {
        _mockElasticsearchService = new Mock<IElasticsearchService>();
        _controller = new ItemsController(_mockElasticsearchService.Object);
    }

    [Fact]
    public async Task GetItems_ReturnsOkWithItems()
    {
        // Arrange
        var items = new List<Item>
        {
            new() { Id = Guid.NewGuid(), Name = "Item1", Description = "Desc1", Price = 10, CreatedDate = DateTimeOffset.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Item2", Description = "Desc2", Price = 20, CreatedDate = DateTimeOffset.UtcNow }
        };
        _mockElasticsearchService.Setup(s => s.GetAllItemsAsync())
            .ReturnsAsync(items);

        // Act
        var result = await _controller.GetItems();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItems = okResult.Value.Should().BeAssignableTo<IEnumerable<ItemDto>>().Subject;
        returnedItems.Should().HaveCount(2);
        _mockElasticsearchService.Verify(s => s.GetAllItemsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetItem_WithValidId_ReturnsOkWithItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item
        {
            Id = itemId,
            Name = "Test Item",
            Description = "Test Description",
            Price = 15.99m,
            CreatedDate = DateTimeOffset.UtcNow
        };
        _mockElasticsearchService.Setup(s => s.GetItemAsync(itemId))
            .ReturnsAsync(item);

        // Act
        var result = await _controller.GetItem(itemId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItem = okResult.Value.Should().BeOfType<ItemDto>().Subject;
        returnedItem.Id.Should().Be(itemId);
        returnedItem.Name.Should().Be("Test Item");
        returnedItem.Price.Should().Be(15.99m);
    }

    [Fact]
    public async Task GetItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockElasticsearchService.Setup(s => s.GetItemAsync(itemId))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _controller.GetItem(itemId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateItem_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateItemDto("New Item", "New Description", 25.50m);
        _mockElasticsearchService.Setup(s => s.IndexItemAsync(It.IsAny<Item>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateItem(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var createdItem = createdResult.Value.Should().BeOfType<ItemDto>().Subject;
        createdItem.Name.Should().Be("New Item");
        createdItem.Description.Should().Be("New Description");
        createdItem.Price.Should().Be(25.50m);
        createdResult.ActionName.Should().Be(nameof(ItemsController.GetItem));
    }

    [Fact]
    public async Task CreateItem_WhenIndexFails_ReturnsInternalServerError()
    {
        // Arrange
        var createDto = new CreateItemDto("New Item", "New Description", 25.50m);
        _mockElasticsearchService.Setup(s => s.IndexItemAsync(It.IsAny<Item>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CreateItem(createDto);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Failed to create item");
    }

    [Fact]
    public async Task UpdateItem_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = new Item
        {
            Id = itemId,
            Name = "Old Name",
            Description = "Old Description",
            Price = 10,
            CreatedDate = DateTimeOffset.UtcNow
        };
        var updateDto = new UpdateItemDto("Updated Name", "Updated Description", 20);

        _mockElasticsearchService.Setup(s => s.GetItemAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockElasticsearchService.Setup(s => s.UpdateItemAsync(It.IsAny<Item>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateItem(itemId, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockElasticsearchService.Verify(s => s.UpdateItemAsync(It.Is<Item>(i =>
            i.Id == itemId &&
            i.Name == "Updated Name" &&
            i.Description == "Updated Description" &&
            i.Price == 20)), Times.Once);
    }

    [Fact]
    public async Task UpdateItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateDto = new UpdateItemDto("Updated Name", "Updated Description", 20);
        _mockElasticsearchService.Setup(s => s.GetItemAsync(itemId))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _controller.UpdateItem(itemId, updateDto);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockElasticsearchService.Verify(s => s.UpdateItemAsync(It.IsAny<Item>()), Times.Never);
    }

    [Fact]
    public async Task UpdateItem_WhenUpdateFails_ReturnsInternalServerError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = new Item
        {
            Id = itemId,
            Name = "Old Name",
            Description = "Old Description",
            Price = 10,
            CreatedDate = DateTimeOffset.UtcNow
        };
        var updateDto = new UpdateItemDto("Updated Name", "Updated Description", 20);

        _mockElasticsearchService.Setup(s => s.GetItemAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockElasticsearchService.Setup(s => s.UpdateItemAsync(It.IsAny<Item>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateItem(itemId, updateDto);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Failed to update item");
    }

    [Fact]
    public async Task DeleteItem_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item
        {
            Id = itemId,
            Name = "Test Item",
            Description = "Test Description",
            Price = 15,
            CreatedDate = DateTimeOffset.UtcNow
        };

        _mockElasticsearchService.Setup(s => s.GetItemAsync(itemId))
            .ReturnsAsync(item);
        _mockElasticsearchService.Setup(s => s.DeleteItemAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteItem(itemId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockElasticsearchService.Verify(s => s.DeleteItemAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task DeleteItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockElasticsearchService.Setup(s => s.GetItemAsync(itemId))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _controller.DeleteItem(itemId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockElasticsearchService.Verify(s => s.DeleteItemAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteItem_WhenDeleteFails_ReturnsInternalServerError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item
        {
            Id = itemId,
            Name = "Test Item",
            Description = "Test Description",
            Price = 15,
            CreatedDate = DateTimeOffset.UtcNow
        };

        _mockElasticsearchService.Setup(s => s.GetItemAsync(itemId))
            .ReturnsAsync(item);
        _mockElasticsearchService.Setup(s => s.DeleteItemAsync(itemId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteItem(itemId);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Failed to delete item");
    }

    [Fact]
    public async Task SearchItems_WithValidQuery_ReturnsOkWithResults()
    {
        // Arrange
        var searchQuery = "sword";
        var items = new List<Item>
        {
            new() { Id = Guid.NewGuid(), Name = "Iron Sword", Description = "A basic sword", Price = 20, CreatedDate = DateTimeOffset.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Steel Sword", Description = "A better sword", Price = 40, CreatedDate = DateTimeOffset.UtcNow }
        };
        _mockElasticsearchService.Setup(s => s.SearchItemsAsync(searchQuery))
            .ReturnsAsync(items);

        // Act
        var result = await _controller.SearchItems(searchQuery);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItems = okResult.Value.Should().BeAssignableTo<IEnumerable<ItemDto>>().Subject;
        returnedItems.Should().HaveCount(2);
        _mockElasticsearchService.Verify(s => s.SearchItemsAsync(searchQuery), Times.Once);
    }

    [Fact]
    public async Task SearchItems_WithEmptyQuery_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.SearchItems("");

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Search query cannot be empty");
        _mockElasticsearchService.Verify(s => s.SearchItemsAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SearchItems_WithWhitespaceQuery_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.SearchItems("   ");

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("Search query cannot be empty");
    }
}
