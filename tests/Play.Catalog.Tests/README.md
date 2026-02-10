# Play.Catalog.Tests

Unit tests for the Play.Catalog microservice with Elasticsearch integration.

## Test Structure

```
Play.Catalog.Tests/
??? Controllers/
?   ??? ItemsControllerTests.cs      # Unit tests for ItemsController (15 tests)
??? Services/
    ??? ElasticsearchServiceTests.cs # Unit tests for Item entity and service (8 tests)
```

## Testing Stack

- **xUnit**: Test framework
- **Moq**: Mocking framework for creating test doubles
- **FluentAssertions**: Fluent assertion library for more readable tests
- **Microsoft.AspNetCore.Mvc.Testing**: Integration testing for ASP.NET Core

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run tests with detailed output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run tests with coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run specific test class
```bash
dotnet test --filter "FullyQualifiedName~ItemsControllerTests"
```

### Run specific test
```bash
dotnet test --filter "FullyQualifiedName~ItemsControllerTests.GetItems_ReturnsOkWithItems"
```

## Test Coverage

### ItemsController Tests (15 tests) ?
- ? **GetItems** - Returns all items successfully
- ? **GetItem** - Returns item by ID
- ? **GetItem** - Returns NotFound for invalid ID
- ? **CreateItem** - Creates new item successfully
- ? **CreateItem** - Returns error when indexing fails
- ? **UpdateItem** - Updates existing item
- ? **UpdateItem** - Returns NotFound for invalid ID
- ? **UpdateItem** - Returns error when update fails
- ? **DeleteItem** - Deletes item successfully
- ? **DeleteItem** - Returns NotFound for invalid ID
- ? **DeleteItem** - Returns error when delete fails
- ? **SearchItems** - Returns search results for valid query
- ? **SearchItems** - Returns BadRequest for empty query
- ? **SearchItems** - Returns BadRequest for whitespace query

### ElasticsearchService Tests (8 tests) ?
- ? **Service Initialization** - ElasticsearchService can be created with client
- ? **Item Entity** - Should have required properties
- ? **Item Entity** - Should allow default values
- ? **Item Entity** - Should support property setting
- ? **Item Entity** - Should accept various valid inputs (Theory with 3 test cases)
- ? **Item Entity** - Should support negative prices
- ? **Item Entity** - Should support very large prices

### Test Results
```
Total tests: 23
     Passed: 23 ?
     Failed: 0
    Skipped: 0
```

## Best Practices

1. **Arrange-Act-Assert (AAA) Pattern**: All tests follow the AAA pattern for clarity
2. **Descriptive Test Names**: Test names clearly describe what is being tested using the pattern `MethodName_Scenario_ExpectedResult`
3. **Isolated Tests**: Each test is independent and doesn't rely on other tests
4. **Mock External Dependencies**: IElasticsearchService is mocked to avoid external dependencies
5. **FluentAssertions**: Uses fluent syntax for readable assertions
6. **Data-Driven Tests**: Uses `[Theory]` and `[InlineData]` for testing multiple scenarios

## Adding New Tests

When adding new functionality to the Play.Catalog service:

1. Create corresponding test methods in the appropriate test class
2. Follow the AAA pattern (Arrange-Act-Assert)
3. Use descriptive test method names (e.g., `MethodName_Scenario_ExpectedResult`)
4. Mock external dependencies using Moq
5. Use FluentAssertions for assertions
6. Consider using `[Theory]` for data-driven tests

Example:
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var mockService = new Mock<IElasticsearchService>();
    var controller = new ItemsController(mockService.Object);
    
    // Act
    var result = await controller.MethodName();
    
    // Assert
    result.Should().BeOfType<OkObjectResult>();
}
```

## Integration Tests

For integration tests with actual Elasticsearch:
- Consider using **Testcontainers** to spin up Elasticsearch in Docker
- Create a separate `Integration` folder for integration tests
- Use `WebApplicationFactory` for end-to-end API testing

Example setup:
```csharp
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
}
```

## Notes

- **Controller Tests**: Fully unit tested with mocked IElasticsearchService to ensure fast, reliable tests
- **Service Tests**: Focus on entity validation and service initialization
- **Elasticsearch Integration**: For full Elasticsearch integration testing, consider using Testcontainers in a separate integration test project
- **Test Isolation**: All tests are isolated and can run in parallel
- **No External Dependencies**: Tests do not require Elasticsearch to be running

## CI/CD Integration

These tests are designed to run in CI/CD pipelines without external dependencies:

```yaml
# Example GitHub Actions workflow
- name: Run tests
  run: dotnet test --no-build --verbosity normal
```

## Code Coverage

To generate code coverage reports:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Then use tools like **ReportGenerator** to create HTML reports:

```bash
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```
