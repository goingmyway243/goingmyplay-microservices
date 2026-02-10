using Elastic.Clients.Elasticsearch;
using Play.Catalog.Entities;

namespace Play.Catalog.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "catalog-items";

    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<bool> IndexItemAsync(Item item)
    {
        try
        {
            var response = await _client.IndexAsync(item, IndexName);
            return response.IsValidResponse;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Item?> GetItemAsync(Guid id)
    {
        try
        {
            var response = await _client.GetAsync<Item>(IndexName, id);
            return response.IsValidResponse ? response.Source : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
        try
        {
            var response = await _client.SearchAsync<Item>(s => s
                .Indices(IndexName)
                .Size(1000)
                .Query(q => q.MatchAll(new Elastic.Clients.Elasticsearch.QueryDsl.MatchAllQuery()))
            );

            return response.IsValidResponse ? response.Documents : Enumerable.Empty<Item>();
        }
        catch (Exception)
        {
            return Enumerable.Empty<Item>();
        }
    }

    public async Task<bool> UpdateItemAsync(Item item)
    {
        try
        {
            var response = await _client.UpdateAsync<Item, Item>(IndexName, item.Id, u => u
                .Doc(item)
            );
            return response.IsValidResponse;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteItemAsync(Guid id)
    {
        try
        {
            var response = await _client.DeleteAsync(IndexName, id);
            return response.IsValidResponse;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<IEnumerable<Item>> SearchItemsAsync(string searchText)
    {
        try
        {
            var response = await _client.SearchAsync<Item>(s => s
                .Indices(IndexName)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(searchText)
                        .Fields(new[] { "name", "description" })
                    )
                )
            );

            return response.IsValidResponse ? response.Documents : Enumerable.Empty<Item>();
        }
        catch (Exception)
        {
            return Enumerable.Empty<Item>();
        }
    }
}
