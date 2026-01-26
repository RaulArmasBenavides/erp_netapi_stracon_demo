using SupplierServiceNet.Application.Dtos;
using SupplierServiceNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplierServiceNet.Application.Interfaces
{
    public interface IUserService
    {

        Task<DataUserDto> Registro(UsuarioRegistroDto usuarioRegistroDto);

        Task<UsuarioLoginRespuestaDto> Login(LoginUserDto usuarioLoginDto);

        ICollection<User> GetUsuarios();

        User GetUsuario(string id);
    }
}
