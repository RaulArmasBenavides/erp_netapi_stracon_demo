using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierServiceNet.Application.Dtos;
using SupplierServiceNet.Application.Interfaces;
using SupplierServiceNet.Core.Entities;

namespace SupplierServiceNet.Controllers
{
     
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService usService;
   
        private readonly IMapper _mapper;

        public UsersController(IUserService usservice, IMapper mapper,IConfiguration config)
        {
            usService = usservice;
            _mapper = mapper;
        }

        [Authorize(Roles = "Approver")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var listaUsuarios = this.usService.GetUsuariosAsync();

            return Ok(listaUsuarios);
        }

        [Authorize(Roles = "Approver")]
        [HttpGet("{usuarioId:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(int usuarioId)
        {
            var itemUsuario = this.usService.GetUsuario(usuarioId.ToString());

            if (itemUsuario == null)
            {
                return NotFound();
            }

            var itemUsuarioDto = _mapper.Map<UserDto>(itemUsuario);
            return Ok(itemUsuarioDto);
        }

        [Authorize(Roles = "Approver")]
        [HttpPost]        
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserAsync([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            var rptaservice= await this.usService.Registro(usuarioRegistroDto);    
            return Ok(rptaservice);
        }
    }
}
