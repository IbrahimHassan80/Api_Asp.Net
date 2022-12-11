using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Model;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreServices _genresServices;

        public GenresController(IGenreServices genresServices)
        {
            _genresServices = genresServices;
        }
    
        [HttpGet]
        public async Task<IActionResult> GetALLAsync()
        {
            var genres = await _genresServices.GetAll();
            return Ok(genres);
        }
    
        [HttpPost]
        public async Task<IActionResult> CreateAsync(GenreDto dto)
        {
           var genre = new Genre { Name = dto.Name };
           await _genresServices.Add(genre);
            return Ok(genre);

        }
    
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(byte id, [FromBody] GenreDto dto)
        {
            var genre = await _genresServices.GetById(id);
            if (genre == null)
                return NotFound($"No Genre Was Found With Id {id}");

            genre.Name = dto.Name;
            _genresServices.Update(genre);
            return Ok(genre);
        }
    
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _genresServices.GetById(id);
            if (genre == null)
                return NotFound($"No Genre Was Found With Id {id}");

            _genresServices.Delete(genre);
            
            return Ok(genre);
        }
    }
}