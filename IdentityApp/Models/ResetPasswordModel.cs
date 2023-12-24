namespace IdentityApp.Models{
    public class ResetPasswordModel{
        public string Id { get; set; }
        public string? Password { get; set; }
        public string? PasswordCheck { get; set; }
        public string? token { get; set; }
    }
}