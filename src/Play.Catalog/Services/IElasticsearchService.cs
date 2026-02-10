using Play.Catalog.Entities;

namespace Play.Catalog.Services;

public interface IElasticsearchService
{
    Task<bool> IndexItemAsync(Item item);
    Task<Item?> GetItemAsync(Guid id);
    Task<IEnumerable<Item>> GetAllItemsAsync();
    Task<bool> UpdateItemAsync(Item item);
    Task<bool> DeleteItemAsync(Guid id);
    Task<IEnumerable<Item>> SearchItemsAsync(string searchText);
}
