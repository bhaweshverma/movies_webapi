using System;
using CsvHelper.Configuration;
using MoviesAPI.Models;

namespace MoviesAPI.Mappers
{
    public class UserMapper: ClassMap<User> 
    {
        public UserMapper()
        {
            Map(x => x.UserId).Name("User_Id");
            Map(x => x.UserName).Name("User_Name");
            Map(x => x.UserType).Name("User_Type");
        }
    }
}