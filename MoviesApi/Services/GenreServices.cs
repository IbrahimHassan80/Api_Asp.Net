using Microsoft.EntityFrameworkCore;
using MoviesApi.Model;

namespace MoviesApi.Services
{
    public class GenreServices : IGenreServices
    {
        private readonly ApplicationDbContext _context;

        public GenreServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAll()
        {
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }
    
         public async Task<Genre> Add(Genre genre)
        {
            await  _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return genre;
        }

        public async Task<Genre> GetById(byte id)
        {
           return await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
        }

        public Genre Update(Genre genre)
        {
            _context.Genres.Update(genre);
            _context.SaveChanges();
            
            return genre;
        }
    
        public Genre Delete(Genre genre)
        {
            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return genre;
        }
    
    }
}