namespace SupplierServiceNet.Application.Dtos
{
    public class UsuarioLoginRespuestaDto
    {
        public DataUserDto User { get; set; }
        public string Access_token { get; set; }

        public string Token_type { get; set; } = "bearer";
    }
}
