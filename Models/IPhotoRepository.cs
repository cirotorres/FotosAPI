namespace FotosAPI.Models
{
    public interface IPhotoRepository
    {
        void Add(Photo photo);
        void Delete(int id);
        List<Photo> Get();
        Photo Get(int id);
        List<Photo> GetAll();
    }
}
