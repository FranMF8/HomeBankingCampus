namespace HomeBankingMindHub.Handlers.Interfaces
{
    public interface IEncryptionHandler
    {
        void EncryptPassword(string password, out byte[] hash, out byte[] salt);
        bool ValidatePassword(string password, byte[] hash, byte[] salt);
    }
}
