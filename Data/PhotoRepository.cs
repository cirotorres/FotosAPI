using FotosAPI.Models;

namespace FotosAPI.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly ConnectionContext _context = new ConnectionContext();
        public void Add(Photo photo)
        {
            _context.Photos.Add(photo);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var photo = _context.Photos.Find(id);
            if (photo != null)
            {
                _context.Photos.Remove(photo);
                _context.SaveChanges();
            }
        }

        public List<Photo> Get()
        {
            return _context.Photos.ToList();
        }

        public Photo? Get(int id)
        {
            return _context.Photos.Find(id);
        }

        public List<Photo> GetAll()
        {
            return _context.Photos.ToList();
        }

        
    }
}
