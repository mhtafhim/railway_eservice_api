using Jose.native;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text;
using sib_api_v3_sdk.Model;

namespace railway_eservice_api.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class RegController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        [HttpPost("sign_up")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database");
                  
                    // Check if email exists
                    using (SqlCommand emailCommand = new SqlCommand("SELECT * FROM users WHERE email = @email", connection))
                    {
                        emailCommand.Parameters.AddWithValue("@email", request.Email);
                        SqlDataReader emailReader = (SqlDataReader)await emailCommand.ExecuteReaderAsync();
                        if (emailReader.HasRows)
                        {
                            return BadRequest(new { message = "Email already exists" });
                        }
                        emailReader.Close();
                    }
                    Console.WriteLine("Email does not exist");
                    // Check if phone exists
                    using (SqlCommand emailCommand = new SqlCommand("SELECT * FROM users WHERE PhoneNumber = @phone", connection))
                    {
                        emailCommand.Parameters.AddWithValue("@phone", request.PhoneNumber);
                        SqlDataReader emailReader = (SqlDataReader)await emailCommand.ExecuteReaderAsync();
                        if (emailReader.HasRows)
                        {
                            return BadRequest(new { message = "Phone already exists" });
                        }
                        emailReader.Close();
                    }
                    Console.WriteLine("Phone does not exist");

                    // Check if NID exists
                    using (SqlCommand emailCommand = new SqlCommand("SELECT * FROM users WHERE NID = @nid", connection))
                    {
                        emailCommand.Parameters.AddWithValue("@nid", request.NID);
                        SqlDataReader emailReader = (SqlDataReader)await emailCommand.ExecuteReaderAsync();
                        if (emailReader.HasRows)
                        {
                            return BadRequest(new { message = "NID already exists" });
                        }
                        emailReader.Close();
                    }
                    Console.WriteLine("NID does not exist");

              //      EmailSender emailSender = new EmailSender();

               //    emailSender.SendEmail(request.Email, request.Name, "Welcome to Railway eService. Your account has been created successfully.");


                    // Hash the password
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, 10);
                    Console.WriteLine(hashedPassword);

                    // Insert user data
                    using (SqlCommand insertUserCommand = new SqlCommand("INSERT INTO users (Name, PhoneNumber, NID, Email, Password) VALUES (@name, @phoneNumber, @nid, @email, @password);\r\n", connection))
                    {
                        Console.WriteLine("ekhane ashche 2");
                        insertUserCommand.Parameters.AddWithValue("@name", request.Name);
                        insertUserCommand.Parameters.AddWithValue("@phoneNumber", request.PhoneNumber);
                        insertUserCommand.Parameters.AddWithValue("@nid", request.NID);
                        insertUserCommand.Parameters.AddWithValue("@email", request.Email);
                        insertUserCommand.Parameters.AddWithValue("@password", hashedPassword);
                        Console.WriteLine("ekhane ashche 3");
                        int userId = Convert.ToInt32(await insertUserCommand.ExecuteScalarAsync());

                        Console.WriteLine("User added successfully");

                        // Generate and return JWT token
                        var payload = new Dictionary<string, object>()
                        {
                            { "email", request.Email },
                            { "id", userId }
                        };

                        var secretKey = Encoding.ASCII.GetBytes("secretkey");
                        Console.WriteLine("ekhane ashche");

                        string token = Jose.JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
                        Console.Write(token);
                        return Ok(new { token = token, user = new { email = request.Email, id = userId , name = request.Name } });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
            }
        }

      
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();

                    // Check if email or username exists
                    using (SqlCommand selectUserCommand = new SqlCommand("SELECT * FROM users WHERE Email = @email", connection))
                    {
                        selectUserCommand.Parameters.AddWithValue("@email", request.email);
                        SqlDataReader userReader = (SqlDataReader)await selectUserCommand.ExecuteReaderAsync();

                        if (userReader.HasRows)
                        {
                            await userReader.ReadAsync();
                            string storedPasswordHash = userReader["password"].ToString();

                            // Check password
                            if (BCrypt.Net.BCrypt.Verify(request.Password, storedPasswordHash))
                            {
                                int userId = Convert.ToInt32(userReader["id"]);
                                string name = userReader["name"].ToString();

                                var payload = new Dictionary<string, object>()
                               {
                                       { "email", userReader["email"] },
                                       { "id", userId },
                                    {"name",name }
                               };

                                var secretKey = Encoding.ASCII.GetBytes("secretkey");

                                string token = Jose.JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);



                                return Ok(new { token = token, user = new { email = userReader["email"], id = userId ,name = name} });
                            }
                            else
                            {
                                return BadRequest(new { message = "Wrong password" });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Wrong email/username" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
            }
        }

        [HttpPost("send_otp")]
        public async Task<IActionResult> SendOTP([FromBody] otp_request request)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database");

                    // Check if email exists
                    using (SqlCommand emailCommand = new SqlCommand("SELECT * FROM users WHERE email = @email", connection))
                    {
                        emailCommand.Parameters.AddWithValue("@email", request.email);
                        SqlDataReader emailReader = (SqlDataReader)await emailCommand.ExecuteReaderAsync();
                        if (emailReader.HasRows)
                        {
                            return BadRequest(new { message = "Email already exist" });
                        }
                        emailReader.Close();
                    }
                    Console.WriteLine("Email not exists");
    
                    Console.WriteLine(request.otp);


                    // Send OTP
                    EmailSender emailSender = new EmailSender();
                    emailSender.SendEmail(request.email, request.name, "Hello "+ request.name + ", Your OTP is " + request.otp);

                    return Ok(new { message = "OTP sent successfully" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
            }
        }



        [HttpPost("admin_login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();

                    // Check if email or username exists
                    using (SqlCommand selectUserCommand = new SqlCommand("SELECT * FROM admins WHERE Email = @email", connection))
                    {
                        selectUserCommand.Parameters.AddWithValue("@email", request.email);
                        SqlDataReader userReader = (SqlDataReader)await selectUserCommand.ExecuteReaderAsync();

                        if (userReader.HasRows)
                        {
                            await userReader.ReadAsync();
                            string storedPasswordHash = userReader["password"].ToString();

                            // Check password
                            if (BCrypt.Net.BCrypt.Verify(request.Password, storedPasswordHash))
                            {
                                int userId = Convert.ToInt32(userReader["id"]);
                                string name = userReader["name"].ToString();

                                var payload = new Dictionary<string, object>()
                               {
                                       { "email", userReader["email"] },
                                       { "id", userId },
                                    {"name",name }
                               };

                                var secretKey = Encoding.ASCII.GetBytes("secretkey");

                                string token = Jose.JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);



                                return Ok(new { token = token, user = new { email = userReader["email"], id = userId, name = name } });
                            }
                            else
                            {
                                return BadRequest(new { message = "Wrong password" });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Wrong email/username" });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
            }
        }


        [HttpPost("add_admin")]
        public async Task<IActionResult> admin_Register([FromBody] User request)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database");

                    // Check if email exists
                    using (SqlCommand emailCommand = new SqlCommand("SELECT * FROM admins WHERE email = @email", connection))
                    {
                        emailCommand.Parameters.AddWithValue("@email", request.Email);
                        SqlDataReader emailReader = (SqlDataReader)await emailCommand.ExecuteReaderAsync();
                        if (emailReader.HasRows)
                        {
                            return BadRequest(new { message = "Email already exists" });
                        }
                        emailReader.Close();
                    }
                    Console.WriteLine("Email does not exist");
                    // Check if phone exists
                    using (SqlCommand emailCommand = new SqlCommand("SELECT * FROM admins WHERE PhoneNumber = @phone", connection))
                    {
                        emailCommand.Parameters.AddWithValue("@phone", request.PhoneNumber);
                        SqlDataReader emailReader = (SqlDataReader)await emailCommand.ExecuteReaderAsync();
                        if (emailReader.HasRows)
                        {
                            return BadRequest(new { message = "Phone already exists" });
                        }
                        emailReader.Close();
                    }
                    Console.WriteLine("Phone does not exist");

                    // Check if NID exists
                    using (SqlCommand emailCommand = new SqlCommand("SELECT * FROM admins WHERE NID = @nid", connection))
                    {
                        emailCommand.Parameters.AddWithValue("@nid", request.NID);
                        SqlDataReader emailReader = (SqlDataReader)await emailCommand.ExecuteReaderAsync();
                        if (emailReader.HasRows)
                        {
                            return BadRequest(new { message = "NID already exists" });
                        }
                        emailReader.Close();
                    }
                    Console.WriteLine("NID does not exist");

                    //      EmailSender emailSender = new EmailSender();

                    //    emailSender.SendEmail(request.Email, request.Name, "Welcome to Railway eService. Your account has been created successfully.");


                    // Hash the password
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, 10);
                    Console.WriteLine(hashedPassword);

                    // Insert user data
                    using (SqlCommand insertUserCommand = new SqlCommand("INSERT INTO admins (Name, PhoneNumber, NID, Email, Password) VALUES (@name, @phoneNumber, @nid, @email, @password);\r\n", connection))
                    {
                        Console.WriteLine("ekhane ashche 2");
                        insertUserCommand.Parameters.AddWithValue("@name", request.Name);
                        insertUserCommand.Parameters.AddWithValue("@phoneNumber", request.PhoneNumber);
                        insertUserCommand.Parameters.AddWithValue("@nid", request.NID);
                        insertUserCommand.Parameters.AddWithValue("@email", request.Email);
                        insertUserCommand.Parameters.AddWithValue("@password", hashedPassword);
                        Console.WriteLine("ekhane ashche 3");
                        int userId = Convert.ToInt32(await insertUserCommand.ExecuteScalarAsync());

                        Console.WriteLine("User added successfully");

                        // Generate and return JWT token
                        var payload = new Dictionary<string, object>()
                        {
                            { "email", request.Email },
                            { "id", userId }
                        };

                        var secretKey = Encoding.ASCII.GetBytes("secretkey");
                      //  Console.WriteLine("ekhane ashche");

                        string token = Jose.JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);
                        Console.Write(token);
                        return Ok(new { token = token, user = new { email = request.Email, id = userId, name = request.Name } });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
            }
        }






    }

    public class admins
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string NID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
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

    public class LoginRequest
    {
        public string email { get; set; }
        public string Password { get; set; }
    }

    public class otp_request
    {
        public string email { get; set; }
        public string name { get; set; }
        public int otp { get; set; }
    }

}
