using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Dtos
{
    public class MovieDtailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; }
        
        public int Year { get; set; }

        public double Rate { get; set; }

        public string StoreLine { get; set; }
        
        public byte[] Poster { get; set; }

        public byte GenreId { get; set; }
    
        public string GenreName { get; set; }
    }
}