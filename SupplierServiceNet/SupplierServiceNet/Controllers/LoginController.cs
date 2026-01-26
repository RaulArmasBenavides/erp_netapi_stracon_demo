using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplierServiceNet.Application.Dtos;
using SupplierServiceNet.Application.Interfaces;

namespace SupplierServiceNet.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class LoginController : ControllerBase
    {

        private readonly IUserService usService;

        private readonly IMapper _mapper;

        public LoginController(IUserService usservice, IMapper mapper, IConfiguration config)
        {
            usService = usservice;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginUserDto usuarioLoginDto)
        {
            var rptaservice = await this.usService.Login(usuarioLoginDto);
            return Ok(rptaservice);
        }
    }
}
