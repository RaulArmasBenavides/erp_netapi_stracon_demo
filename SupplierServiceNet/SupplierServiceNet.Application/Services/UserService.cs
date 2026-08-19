using SupplierServiceNet.Application.Dtos;
using SupplierServiceNet.Application.Interfaces;
using SupplierServiceNet.Core.Entities;
using SupplierServiceNet.Infrastructure.Repositorio.WorkContainer;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SupplierServiceNet.Core.IRepositorio;
using SupplierServiceNet.CrossCutting.Options;
using SupplierServiceNet.CrossCutting.Exceptions;

namespace SupplierServiceNet.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApiSettings _apiSettings;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IMapper mapper,
            RoleManager<IdentityRole> roleManager,
            IOptions<ApiSettings> apiSettings)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _apiSettings = apiSettings.Value;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(LoginUserDto usuarioLoginDto)
        {
            // Normalizar el input
            var normalizedInput = usuarioLoginDto.UserName.ToLower();

            // Buscar usuario por username o email
            var usuario = await _unitOfWork.Users.GetUsuarioByUserNameOrEmailAsync(normalizedInput);

            if (usuario == null)
            {
                throw new AuthenticationException("Invalid credentials");
            }

            // Verificar contraseña
            bool isValid = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);

            if (!isValid)
            {
                throw new AuthenticationException("Invalid credentials");
            }

            // Aquí existe el usuario y la contraseña es válida
            var roles = await this._userManager.GetRolesAsync(usuario);
            var manejadorToken = new JwtSecurityTokenHandler();
            var keyconfig = _apiSettings.Secreta;

            var key = Encoding.ASCII.GetBytes(keyconfig);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddDays(_apiSettings.TokenExpirationDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = manejadorToken.CreateToken(tokenDescriptor);
            var userdto = _mapper.Map<DataUserDto>(usuario);
            userdto.Role = roles.FirstOrDefault();
            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Access_token = manejadorToken.WriteToken(token),
                User = userdto
            };

            return usuarioLoginRespuestaDto;
        }

        public async Task<DataUserDto> Registro(UsuarioRegistroDto dto)
        {
            var usuario = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                NormalizedEmail = dto.Email.ToUpper(),
                NormalizedUserName = dto.UserName.ToUpper(),
                CreatedAt = DateTime.Now
                
            };

            var result = await _userManager.CreateAsync(usuario, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new ApplicationException($"Error al registrar usuario: {errors}");
            }

            await EnsureRoleExistsAsync("Requester");
            await EnsureRoleExistsAsync("Approver");

            await _userManager.AddToRoleAsync(usuario, "Requester");

            var usuarioRetornado =
                await _unitOfWork.Users.GetUsuarioByUserNameOrEmailAsync(dto.UserName);

            return _mapper.Map<DataUserDto>(usuarioRetornado);
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
                    Role = roles.FirstOrDefault() ?? "Sin rol",
                    Created = usuario.CreatedAt
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