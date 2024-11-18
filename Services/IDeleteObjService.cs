using FotosAPI.Models;

namespace FotosAPI.Services
{
    public interface IDeleteObjService
    {
        bool DeleteObj(int id);

        public Photo? FindObjById(int id);


    }
}
