namespace HomeBankingMindHub.Handlers.Interfaces
{
    public interface IEncryptionHandler
    {
        string EncryptPassword(string password, out byte[] hash, out byte[] salt);
        bool ValidatePassword(string password, byte[] salt);
    }
}
