using PaymentServiceNet.Application.Dtos;
using PaymentServiceNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.Application.Interfaces
{
    public interface IUserService
    {

        Task<DataUserDto> Registro(UsuarioRegistroDto usuarioRegistroDto);

        Task<UsuarioLoginRespuestaDto> Login(LoginUserDto usuarioLoginDto);

        ICollection<AppUsuario> GetUsuarios();

        AppUsuario GetUsuario(string id);
    }
}
