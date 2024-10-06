namespace OnlyanKreditSistemi.Services.Interfaces
{
    public interface IMailService
    {
        Task SendMail(string from, string to, string subject, string link);   
    }
}
