using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Mappers;
using MoviesAPI.Models;
using MoviesAPI.Services.Helper;

namespace MoviesAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private List<Movie> movies = new List<Movie>();

        public MoviesController()
        {
            FetchAllMovies();
        }

        [HttpGet]
        public IActionResult AllMovies(){

            return Ok(movies);
        
        }
        
        [HttpGet("id/{id}")]
        public IActionResult MovieById(int id){
            FetchAllMovies();
            IEnumerable<Movie> movieById = 
                from m in movies
                where m.MovieId.Equals(id)
                select m;

                return Ok(movieById);
        }
       
        [HttpPost]
        public IActionResult AddMovie([FromBody] Movie movie){
            
            try
                {
                    using(StreamWriter sw = new StreamWriter(@"CSV Files\Movies.csv", true, new UTF8Encoding(true)))
                    using(CsvWriter csvw = new CsvWriter(sw, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        //csvw.NextRecord();
                        csvw.WriteRecord<Movie>(movie);
                        csvw.NextRecord();
                    }
                    FetchAllMovies();
                    IEnumerable<Movie> newMovie = 
                            from m in movies
                            where m.MovieId.Equals(movie.MovieId)
                            select m; 
                    
                    return Ok(newMovie);
                }
                catch(Exception e)
                {
                    return BadRequest(e.Message);
                }
        }

        private void FetchAllMovies()
        {
            try
            {
                using(StreamReader reader = new StreamReader(@"CSV Files\Movies.csv", Encoding.Default))
                using(var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                {
                    csv.Configuration.RegisterClassMap<MovieMapper>();
                    foreach(Movie m in csv.GetRecords<Movie>())
                    {
                        movies.Add((m));
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
    }
}
