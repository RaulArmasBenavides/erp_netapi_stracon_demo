using System.Data;
using ApiMovies.Core.Entities;
using ApiMovies.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using ApiMovies.Core.IRepositorio;

namespace ApiMovies.Repositorio
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

        public AppUsuario GetUsuario(string usuarioId)
        {
            return _bd.AppUsuario.FirstOrDefault(c => c.Id == usuarioId);
        }

        public AppUsuario GetUsuarioByUserName(string userName)
        {
           return _bd.AppUsuario.FirstOrDefault(u => u.UserName == userName);
        }

        public ICollection<AppUsuario> GetUsuarios()
        {
            return _bd.AppUsuario.OrderBy(c => c.Name).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuariobd = _bd.AppUsuario.FirstOrDefault(u => u.Name== usuario);
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
