using SupplierServiceNet.Core.Entities;

namespace SupplierServiceNet.Core.IRepositorio
{
    public interface IUserRepository
    {
        ICollection<User> GetUsuarios();
        User GetUsuario(string usuarioId);

        User GetUsuarioByUserName(string userName);
        bool IsUniqueUser(string usuario);

        Task<User> GetUsuarioByUserNameOrEmailAsync(string usernameOrEmail);
    }
}
