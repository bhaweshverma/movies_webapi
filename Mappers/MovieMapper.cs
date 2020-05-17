using System;
using CsvHelper.Configuration;
using MoviesAPI.Models;

namespace MoviesAPI.Mappers
{
    public class MovieMapper: ClassMap<Movie> 
    {
        public MovieMapper()
        {
            Map(x => x.MovieId).Name("Movie_Id");
            Map(x => x.MovieName).Name("Movie_Name");
            Map(x => x.MovieDescription).Name("Movie_Description");
        }
    }
}