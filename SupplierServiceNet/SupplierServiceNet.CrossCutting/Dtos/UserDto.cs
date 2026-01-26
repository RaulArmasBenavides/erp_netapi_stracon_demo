namespace SupplierServiceNet.Application.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
 
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Created { get; set; }

        public string Role { get; set; }
        
    }
}
