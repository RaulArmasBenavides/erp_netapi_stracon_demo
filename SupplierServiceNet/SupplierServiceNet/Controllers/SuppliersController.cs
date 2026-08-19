using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierServiceNet.Application.Interfaces;
using SupplierServiceNet.Core.Interfaces;
using SupplierServiceNet.CrossCutting.Dtos;
using SupplierServiceNet.CrossCutting.Supplier;
using SupplierServiceNet.CrossCutting.Exceptions;

namespace SupplierServiceNet.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/suppliers")]
    public sealed class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IUserContextService _userContextService;
        private readonly IExcelService _excelService;
        private readonly IMapper _mapper;

        public SuppliersController(
            ISupplierService supplierService,
            IUserContextService userContextService,
            IExcelService excelService,
            IMapper mapper)
        {
            _supplierService = supplierService;
            _userContextService = userContextService;
            _excelService = excelService;
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
            try
            {
                var createdBy = _userContextService.GetUserIdentifier();
                var created = await _supplierService.CreateAsync(dto, createdBy, ct);

                return CreatedAtRoute("GetSupplier", new { id = created.Id }, created);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(ex.Message);
            }
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
            try
            {
                var deletedBy = _userContextService.GetUserIdentifier();
                var deleted = await _supplierService.DeleteAsync(id, deletedBy, ct);
                if (!deleted) return NotFound();

                return NoContent();
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(ex.Message);
            }
        }


        [Authorize(Roles = "Approver")]
        [HttpPost("{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ApproveSupplier(Guid id, CancellationToken ct)
        {
            try
            {
                var approvedBy = _userContextService.GetUserIdentifier();
                var approved = await _supplierService.ApproveAsync(id, approvedBy, ct);

                if (approved is null)
                    return NotFound();

                return Ok(approved);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize(Roles = "Requester,Approver")]
        [HttpGet("export/template")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult DownloadTemplate()
        {
            var excelFile = _excelService.GenerateTemplateExcel();
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Supplier_Template.xlsx");
        }

        [Authorize(Roles = "Requester,Approver")]
        [HttpGet("export")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ExportSuppliers(CancellationToken ct)
        {
            var suppliers = await _supplierService.GetForExportAsync(ct);
            var excelFile = _excelService.ExportSuppliersToExcel(suppliers);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"Suppliers_Export_{timestamp}.xlsx";

            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [Authorize(Roles = "Requester,Approver")]
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ImportSuppliers([FromForm] IFormFile file, CancellationToken ct)
        {
            try
            {
                var importedBy = _userContextService.GetUserIdentifier();
                var suppliersData = await _excelService.ReadSuppliersFromExcelAsync(file);
                var result = await _supplierService.BulkImportAsync(suppliersData, importedBy, ct);

                return Ok(result);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
