using PaymentServiceNet.Application.Dtos;
using PaymentServiceNet.Application.Interfaces;
using PaymentServiceNet.Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaymentServiceNet.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService usService;
   
        private readonly IMapper _mapper;

        public UsersController(IUserService usservice, IMapper mapper,IConfiguration config)
        {
            usService = usservice;
 
            _mapper = mapper;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var listaUsuarios = this.usService.GetUsuarios();

            var listaUsuariosDto = new List<UserDto>();

            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UserDto>(lista));
            }
            return Ok(listaUsuariosDto);
        }

        [Authorize(Roles = "admin")]
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

        [AllowAnonymous]
        [HttpPost("registro")]        
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserAsync([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {

            var rptaservice= await this.usService.Registro(usuarioRegistroDto);
            //bool validarNombreUsuarioUnico = _usRepo.IsUniqueUser(usuarioRegistroDto.UserName);
            //if (!validarNombreUsuarioUnico)
            //{
            //    _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
            //    _respuestaApi.IsSuccess = false;
            //    _respuestaApi.ErrorMessages.Add("El nombre de usuario ya existe");
            //    return BadRequest(_respuestaApi);
            //}

            //var usuario = await _usRepo.Registro(usuarioRegistroDto);
            //if (usuario == null)
            //{
            //    _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
            //    _respuestaApi.IsSuccess = false;
            //    _respuestaApi.ErrorMessages.Add("Error en el registro");
            //    return BadRequest(_respuestaApi);
            //}

            //_respuestaApi.StatusCode = HttpStatusCode.OK;
            //_respuestaApi.IsSuccess = true;         
            return Ok(rptaservice);
        }



    }
}
