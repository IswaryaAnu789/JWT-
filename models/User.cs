namespace LoginJwt.models
{
    public class User
    {
        public int UserId {  get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email {  get; set; } = string.Empty;

        public string Password {  get; set; } = string.Empty;

        public int Isactive { get; set; } = 1;

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
