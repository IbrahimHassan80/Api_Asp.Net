using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
       
        private new List<string> _allowExtension = new List<string> { ".jpg", ".png" };
        private long _maxAllowPosterSize = 3145728; /// 3MB
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies.OrderByDescending(r => r.Rate)
            .Include(m => m.Genre).ToListAsync();
            return Ok(movies);
        }
    

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefault(m => m.Id == id);
            
            if(movie == null)
                return NotFound();

             var dto = new MovieDtailsDto 
             {
                 Id = movie.Id,
                 GenreId = movie.GenreId,
                 GenreName = movie.Genre.Name,
                 Poster = movie.Poster,
                 Rate = movie.Rate,
                 StoreLine = movie.StoreLine,
                 Title = movie.Title,
                 Year = movie.Year,
                 };
             return Ok(dto);    
        }


        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(int genreId)
        {
             var movie = await _context.Movies
             .Where(m => m.GenreId == genreId)
             .Include(m => m.Genre)
             .Select(m => new MovieDtailsDto 
             {
                 Id = m.Id,
                 GenreId = m.GenreId,
                 GenreName = m.Genre.Name,
                 Poster = m.Poster,
                 Rate = m.Rate,
                 StoreLine = m.StoreLine,
                 Title = m.Title,
                 Year = m.Year,
                 })
                 .ToListAsync();

             return Ok(movie);        
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {
            if (dto.Poster == null)
            {
                return BadRequest("Poster Is Required");
            }
            // phot extension
            if (!_allowExtension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only jpg and png image are allowed!");
            // photo size
            if(dto.Poster.Length > _maxAllowPosterSize)
                return BadRequest("Max size allow is 3 MB");

            var isValid = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if(!isValid)
                return BadRequest("Invalid Genre Id");

            using var dataStreame = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStreame);

            var movie = new Movie
            {
                Title = dto.Title,
                Year = dto.Year,
                Rate = dto.Rate,
                StoreLine = dto.StoreLine,
                GenreId = dto.GenreId,
                Poster = dataStreame.ToArray()
            };

            await _context.Movies.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);
        }
    

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound($"No movie was found with ID {id}");

            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid genere ID!");

            if(dto.Poster != null)
            {
                if (!_allowExtension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg images are allowed!");

                if (dto.Poster.Length > _maxAllowPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB!");

                using var dataStream = new MemoryStream();

                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.StoreLine = dto.StoreLine;
            movie.Rate = dto.Rate;

            _context.Movies.Update(movie);

            return Ok(movie);
        }



     [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound($"No movie was found with ID {id}");

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok(movie);
        }
    }
}
