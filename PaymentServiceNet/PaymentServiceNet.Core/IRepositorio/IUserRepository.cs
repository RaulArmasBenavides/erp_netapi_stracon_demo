using PaymentServiceNet.Core.Entities;

namespace PaymentServiceNet.Core.IRepositorio
{
    public interface IUserRepository
    {
        ICollection<User> GetUsuarios();
        User GetUsuario(string usuarioId);

        User GetUsuarioByUserName(string userName);
        bool IsUniqueUser(string usuario);
   
        //Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        ////Task<User> Registro(UsuarioRegistroDto usuarioRegistroDto);

        //Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto);
    }
}
