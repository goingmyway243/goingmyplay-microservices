using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Entities;
using Play.Catalog.Services;

namespace Play.Catalog
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IElasticsearchService _elasticsearchService;

        public ItemsController(IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var items = await _elasticsearchService.GetAllItemsAsync();
            var itemDtos = items.Select(item => new ItemDto(
                item.Id,
                item.Name,
                item.Description,
                item.Price,
                item.CreatedDate
            ));
            return Ok(itemDtos);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ItemDto>>> SearchItems([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty");
            }

            var items = await _elasticsearchService.SearchItemsAsync(query);
            var itemDtos = items.Select(item => new ItemDto(
                item.Id,
                item.Name,
                item.Description,
                item.Price,
                item.CreatedDate
            ));
            return Ok(itemDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(Guid id)
        {
            var item = await _elasticsearchService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            var success = await _elasticsearchService.IndexItemAsync(item);
            if (!success)
            {
                return StatusCode(500, "Failed to create item");
            }

            var itemDto = new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, itemDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateItem(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await _elasticsearchService.GetItemAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            var success = await _elasticsearchService.UpdateItemAsync(existingItem);
            if (!success)
            {
                return StatusCode(500, "Failed to update item");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _elasticsearchService.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var success = await _elasticsearchService.DeleteItemAsync(id);
            if (!success)
            {
                return StatusCode(500, "Failed to delete item");
            }

            return NoContent();
        }
    }
}

