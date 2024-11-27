namespace FotosAPI.Models
{
    public interface IPhotoRepository
    {
        Task Add(Photo photo);
        void Delete(int id);
        Photo Get(int id);
        List<Photo> GetAll();
       
    }
}
