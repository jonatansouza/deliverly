using DeliverlyCore.Pricing.Domain.UseCases.TariffTables;
using Microsoft.AspNetCore.Mvc;

namespace DeliverlyCore.Controllers
{
    [ApiController]
    [Route("api/tariff-tables")]
    public class TariffTablesController : ControllerBase
    {
        private readonly CreateTariffTableUseCase _create;
        private readonly GetTariffTableByIdUseCase _getById;
        private readonly ListTariffTablesUseCase _list;
        private readonly UpdateTariffTableUseCase _update;
        private readonly DeleteTariffTableUseCase _delete;

        public TariffTablesController(
            CreateTariffTableUseCase create,
            GetTariffTableByIdUseCase getById,
            ListTariffTablesUseCase list,
            UpdateTariffTableUseCase update,
            DeleteTariffTableUseCase delete)
        {
            _create = create;
            _getById = getById;
            _list = list;
            _update = update;
            _delete = delete;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _list.ExecuteAsync(null, ct);
            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _getById.ExecuteAsync(id, ct);
            if (result.IsFailure) return NotFound(result.Error);
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTariffTableRequest request, CancellationToken ct)
        {
            var result = await _create.ExecuteAsync(request, ct);
            if (result.IsFailure) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTariffTableRequest request, CancellationToken ct)
        {
            if (id != request.Id) return BadRequest("Route id and body id must match.");
            var result = await _update.ExecuteAsync(request, ct);
            if (result.IsFailure) return result.Error.Contains("not found") ? NotFound(result.Error) : BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _delete.ExecuteAsync(id, ct);
            if (result.IsFailure) return NotFound(result.Error);
            return NoContent();
        }
    }
}
