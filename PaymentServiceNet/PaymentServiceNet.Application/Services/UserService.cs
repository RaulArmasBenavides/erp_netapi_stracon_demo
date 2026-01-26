using SupplierServiceNet.Application.Dtos;
using SupplierServiceNet.Application.Interfaces;
using SupplierServiceNet.Core.Entities;
using SupplierServiceNet.Infrastructure.Repositorio.WorkContainer;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using SupplierServiceNet.Core.IRepositorio;
namespace SupplierServiceNet.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private IConfiguration _config;

        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(IUnitOfWork unitOfWork, IConfiguration config, UserManager<User> userManager, IMapper mapper , RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(LoginUserDto usuarioLoginDto)
        {
            var usuario = this._unitOfWork.Users.GetUsuarioByUserName(usuarioLoginDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);
            if (usuario == null || !isValid )
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Access_token = "",
                    User = null
                };
            }
            //Aquí existe el usuario entonces podemos procesar el login
            var roles = await this._userManager.GetRolesAsync(usuario);
            var manejadorToken = new JwtSecurityTokenHandler();
            string keyconfig = _config.GetSection("ApiSettings:Secreta").Value.ToString();
 
            var key = Encoding.ASCII.GetBytes(keyconfig); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.Name, usuario.UserName.ToString()),
                    new(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = manejadorToken.CreateToken(tokenDescriptor);
            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Access_token = manejadorToken.WriteToken(token),
                User = _mapper.Map<DataUserDto>(usuario),

            };
            return usuarioLoginRespuestaDto;
        }

        public async Task<DataUserDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            User usuario = new()
            {
                UserName = usuarioRegistroDto.NombreUsuario,
                Email = usuarioRegistroDto.NombreUsuario,
                NormalizedEmail = usuarioRegistroDto.NombreUsuario.ToUpper(),
                NormalizedUserName = usuarioRegistroDto.Nombre,
            };
            var result = await this._userManager.CreateAsync(usuario, usuarioRegistroDto.Password);
            if (result.Succeeded)
            {
                //if (!_roleManager.RoleExistsAsync("Requester").GetAwaiter().GetResult())
                //{
                //    await _roleManager.CreateAsync(new IdentityRole("Requester"));
                //    await _roleManager.CreateAsync(new IdentityRole("Approver"));
                //}
                await EnsureRoleExistsAsync("Requester");
                await EnsureRoleExistsAsync("Approver");
                await this._userManager.AddToRoleAsync(usuario, "Requester");
                var usuarioRetornado = this._unitOfWork.Users.GetUsuarioByUserName(usuarioRegistroDto.NombreUsuario);
                return _mapper.Map<DataUserDto>(usuarioRetornado);
            }
            return new DataUserDto();
        }

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));
        }


        public async Task<ICollection<UserDto>> GetUsuariosAsync()
        {
            var usuarios = _userManager.Users.ToList();
            var usuariosDto = new List<UserDto>();

            foreach (var usuario in usuarios)
            {
                var roles = _userManager.GetRolesAsync(usuario).GetAwaiter().GetResult();

                usuariosDto.Add(new UserDto
                {
                    Id = usuario.Id,
                    UserName = usuario.UserName,
                    Email = usuario.Email,
                    PhoneNumber = usuario.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "Sin rol"
                });
            }

            return usuariosDto;
        }


        public User GetUsuario(string id)
        {
            return this._unitOfWork.Users.GetUsuario(id);
        }
    }
}