using MoviesApi.Model;
namespace MoviesApi.Services
{
    public interface IGenreServices
    {
         Task<IEnumerable<Genre>> GetAll();
        
         Task<Genre> Add(Genre genre);
    
         Task<Genre> GetById(byte id);

         Genre Update(Genre genre);

         Genre Delete(Genre genre);
    }
}