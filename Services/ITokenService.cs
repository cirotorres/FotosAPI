namespace FotosAPI.Services
{
    public interface ITokenService
    {
        string Generate(string userName, string appId);

    }
}
