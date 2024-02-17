using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace railway_eservice_api.Controllers
{

    [ApiController]
    public class testController : ControllerBase
    {

        private readonly IConfiguration _configuration;

        public testController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

      

        [Route("api/test")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
          //  Console.WriteLine("Get request received");
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
            //    Console.WriteLine("Connection string: " + _configuration.GetConnectionString("DefaultConnection"));
                List<User> users = new List<User>();
                try
                {    
                 //   Console.WriteLine("Opening connection111");    
                    await conn.OpenAsync();
               //     Console.WriteLine("Opening connection2");
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM users", conn))
                    {
                //        Console.WriteLine("Opening connection3");
                        SqlDataReader reader = (SqlDataReader)await cmd.ExecuteReaderAsync();
                 //       Console.WriteLine("Opening connection4");
                        if (reader.HasRows)
                        {
                 //           Console.WriteLine("Opening connection5");
                            while (await reader.ReadAsync())
                            {
                                users.Add(new User()
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    PhoneNumber = reader.GetString(2),
                                    NID = reader.GetString(3),
                                    Email = reader.GetString(4),
                                    Password = reader.GetString(5)
                                    
                                });
                            }
                            return Ok(users);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest();
                }
            }
        }

   
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string NID { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }


    }
}
