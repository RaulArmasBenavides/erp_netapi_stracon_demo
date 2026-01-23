using ApiMovies.Core.Entities;

namespace ApiMovies.Core.IRepositorio
{
    public interface IUserRepository
    {
        ICollection<AppUsuario> GetUsuarios();
        AppUsuario GetUsuario(string usuarioId);

        AppUsuario GetUsuarioByUserName(string userName);
        bool IsUniqueUser(string usuario);
   
        //Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        ////Task<User> Registro(UsuarioRegistroDto usuarioRegistroDto);

        //Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto);
    }
}
