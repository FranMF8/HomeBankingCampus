namespace HomeBankingMindHub.Services.Interfaces
{
    public interface IEmailService
    {
        public bool ValidateEmail(string email);
        public Task<bool> SendEmail(string email);
    }
}
