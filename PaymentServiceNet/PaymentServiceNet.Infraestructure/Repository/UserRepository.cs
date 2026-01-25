using System.Data;
using PaymentServiceNet.Core.Entities;
using PaymentServiceNet.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using PaymentServiceNet.Core.IRepositorio;

namespace PaymentServiceNet.Repositorio
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _bd;
        private string claveSecreta;

    
        public UserRepository(ApplicationDbContext bd, IConfiguration config)
        {
            _bd = bd;
            claveSecreta = "";//config.GetValue<string>("ApiSettings:Secreta");
            //_userManager = userManager;
            //_roleManager = roleManager;
        }

        public User GetUsuario(string usuarioId)
        {
            return _bd.Users.FirstOrDefault(c => c.Id == usuarioId);
        }

        public User GetUsuarioByUserName(string userName)
        {
           return _bd.Users.FirstOrDefault(u => u.UserName == userName);
        }

        public ICollection<User> GetUsuarios()
        {
            return _bd.Users.OrderBy(c => c.NormalizedUserName).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuariobd = _bd.Users.FirstOrDefault(u => u.NormalizedUserName == usuario);
            if (usuariobd == null)
            {
                return true;
            }
            return false;
        }


        //Método para encriptar contraseña con MD5 se usa tanto en el Acceso como en el Registro
        //public static string obtenermd5(string valor)
        //{
        //    MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        //    byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
        //    data = x.ComputeHash(data);
        //    string resp = "";
        //    for (int i = 0; i < data.Length; i++)
        //        resp += data[i].ToString("x2").ToLower();
        //    return resp;
        //}
    }
}
