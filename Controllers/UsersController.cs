using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Models;
using System.Linq;
using System.IO;
using System.Text;
using CsvHelper;
using MoviesAPI.Mappers;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using MoviesAPI.Services.Helper;
using System.Net.Http;
using System.Threading.Tasks;

// created contoller class manually
namespace MoviesAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {
        public List<User> users = new List<User>();
       public UsersController()
       {
           _fetchAllUsers();
       }

        [HttpGet]
        public async Task<IActionResult> AllUsers([FromHeader] string Authorization){
            AuthorizeAPI authorizeAPI = new AuthorizeAPI();
            HttpClient client = authorizeAPI.InitializeClient();
            client.DefaultRequestHeaders.Add("token", Authorization);
            HttpResponseMessage response = await client.GetAsync("api/authorize/validatetoken");
            var result = await response.Content.ReadAsStringAsync();
            if(result == "true")
            {
                return Ok(users);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("id/{id}")]
        public IEnumerable<User> UserById(int id){
            IEnumerable<User> userById = 
                from u in users
                where u.UserId.Equals(id)
                select u;

            return userById;
        }

        [HttpPost]
        public IEnumerable<User> AddUser([FromBody] User user){
            try
            {
                using(StreamWriter sw = new StreamWriter(@"CSV Files\Users.csv",true, new UTF8Encoding(true)))
                using(CsvWriter csvw = new CsvWriter(sw, System.Globalization.CultureInfo.CurrentCulture)){
                    //csvw.NextRecord();
                    csvw.WriteRecord<User>(user);
                    csvw.NextRecord();
                }
                _fetchAllUsers();
                return users;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void _fetchAllUsers(){
            try
            {
                using(StreamReader reader = new StreamReader(@"CSV Files\Users.csv", Encoding.Default))
                using(var csv = new CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                {
                    csv.Configuration.RegisterClassMap<UserMapper>();
                    foreach(User u in csv.GetRecords<User>())
                    {
                        users.Add((u));
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
