namespace Fillament
{
    public class UserSecurityOptions : SecurityOptions
    {
        public string PasswordSalt { get; set; }
		
        public string PasswordHash { get; set; }
    }
}