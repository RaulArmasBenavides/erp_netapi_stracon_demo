using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierServiceNet.Core.Interfaces;
using SupplierServiceNet.CrossCutting.Dtos;
using SupplierServiceNet.CrossCutting.Supplier;
using System.Security.Claims;

namespace SupplierServiceNet.Controllers
{
 
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/suppliers")]
    public sealed class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SuppliersController(ISupplierService supplierService, IMapper mapper)
        {
            _supplierService = supplierService;
            _mapper = mapper;
        }

        // GET: api/suppliers
        [Authorize(Roles = "Requester,Approver")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetSuppliers(CancellationToken ct)
        {
            var suppliers = await _supplierService.GetAllAsync(ct);

            var dto = new List<SupplierDto>(suppliers.Count);
            //foreach (var s in suppliers)
            //    dto.Add(_mapper.Map<SupplierDto>(s));

            return Ok(suppliers);
        }

        // GET: api/suppliers/{id}
        [Authorize(Roles = "Requester,Approver")]
        [HttpGet("{id:guid}", Name = "GetSupplier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetSupplier(Guid id, CancellationToken ct)
        {
            var supplier = await _supplierService.GetByIdAsync(id, ct);
            if (supplier is null) return NotFound();

            //return Ok(_mapper.Map<SupplierDto>(supplier));
            return Ok(supplier);
        }

        [Authorize(Roles = "Requester,Approver")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateSupplier([FromForm] CreateSupplierDto dto, CancellationToken ct)
        {
            var createdBy =
             User.FindFirstValue(ClaimTypes.Email)
             ?? User.FindFirstValue("email")
             ?? User.Identity?.Name
             ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
             ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(createdBy))
                return Unauthorized("Token sin claim identificable (email/name/sub).");
            var created = await _supplierService.CreateAsync(dto, createdBy, ct);
            //var createdDto = _mapper.Map<SupplierDto>(created);

            // 201 + Location header
            return CreatedAtRoute("GetSupplier", new { id = created.Id }, created);
        }

        // PATCH: api/suppliers/{id}
        // PATCH parcial “simple”: si una propiedad viene null, NO se modifica
        [Authorize(Roles = "Requester,Approver")]
        [HttpPatch("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PatchSupplier(Guid id, [FromBody] UpdateSupplierDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _supplierService.PatchAsync(id, dto, ct);
            if (updated is null) return NotFound();

            return Ok(_mapper.Map<SupplierDto>(updated));
        }

        // PATCH: api/suppliers/{id}/photo
        [Authorize(Roles = "Requester,Approver")]
        [HttpPatch("{id:guid}/photo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PatchSupplierPhoto(Guid id, [FromBody] UpdateSupplierPhotoDto dto, CancellationToken ct)
        {
            var updated = await _supplierService.PatchPhotoAsync(id, dto.PhotoId, ct);
            if (updated is null) return NotFound();

            return Ok(_mapper.Map<SupplierDto>(updated));
        }

        // DELETE: api/suppliers/{id}
        [Authorize(Roles = "Requester,Approver")]
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteSupplier(Guid id, CancellationToken ct)
        {
            var deleted = await _supplierService.DeleteAsync(id, ct);
            if (!deleted) return NotFound();

            return NoContent();
        }


        [Authorize(Roles = "Approver")]
        [HttpPost("{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ApproveSupplier(Guid id, CancellationToken ct)
        {
            // Prioridad: email -> name -> sub (id)
            var approvedBy =
                User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue("email")
                ?? User.Identity?.Name
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(approvedBy))
                return Unauthorized("Token sin claim identificable (email/name/sub).");

            var approved = await this._supplierService.ApproveAsync(id, approvedBy, ct);
            if (approved is null) return NotFound();

            return Ok(approved);
        }

    }
}
