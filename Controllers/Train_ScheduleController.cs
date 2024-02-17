using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace railway_eservice_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Train_ScheduleController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public Train_ScheduleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

       
        [HttpPost("add_train_schedule")]
        public async Task<IActionResult> add_Train([FromBody] Train_Schedule request)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database");

                    //  string insertQuery = "INSERT INTO TrainSchedule (Train_ID, StartStation, DestinationStation, DepartureTime, Duration, DestinationArrivalTime) VALUES (@ID, @StartStation, @DestinationStation, @DepartureTime, @Duration, @DestinationArrivalTime)";
                    string insertQuery2 = "INSERT INTO TrainSchedule (Train_ID, StartStation, DestinationStation, journey_data, DepartureTime, DestinationArrivalTime) VALUES (@Train_ID, @startStation, @destinationstation, @journeydate, @departureTime, @DestinationArrivalTime);";
                    // Insert user data
                    using (SqlCommand insertUserCommand = new SqlCommand( insertQuery2, connection))
                    {
                        insertUserCommand.Parameters.AddWithValue("@Train_ID", request.Train_ID);
                        insertUserCommand.Parameters.AddWithValue("@startStation", request.StartStation);
                        insertUserCommand.Parameters.AddWithValue("@destinationstation", request.DestinationStation);
                        insertUserCommand.Parameters.AddWithValue("@journeydate", request.JourneyDate);
                        insertUserCommand.Parameters.AddWithValue("@departureTime", request.DepartureTime);
                        insertUserCommand.Parameters.AddWithValue("@DestinationArrivalTime", request.DestinationArrivalTime);
                        await insertUserCommand.ExecuteNonQueryAsync();


                        return Ok(new { message = "Train " + request.Train_ID + " added successfully" });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return StatusCode(500);
                }
                
            }
        }



        //write a get endpoint with a argument of train id
       
        [HttpGet("show_train_schedule")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                List<Train_Schedule_show> trains = new List<Train_Schedule_show>();
                try
                {
                    Console.WriteLine(id);
                    String query = "select Journey_ID, trainSchedule.train_id,startstation,DestinationStation,journey_data,DepartureTime,DestinationArrivalTime,train_name from TrainSchedule join Train_info on TrainSchedule.Train_ID = Train_info.train_id where journey_data >= getdate()";
                    if(id != 0)
                    {
                        query += "and trainSchedule.Train_ID = @id";
                    }
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        Console.WriteLine(cmd.CommandText);
                        SqlDataReader reader = (SqlDataReader)await cmd.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                trains.Add(new Train_Schedule_show()
                                {
                                    Journey_ID = reader.GetInt32("Journey_ID"),
                                   Train_ID = reader.GetInt32("train_id"),
                                   StartStation = reader.GetString("startstation"),
                                   DestinationStation = reader.GetString("DestinationStation"),
                                   JourneyDate = reader.GetDateTime("journey_data"),
                                   DepartureTime = reader.GetTimeSpan(5),
                                   DestinationArrivalTime = reader.GetTimeSpan(6),
                                   TrainName = reader.GetString("train_name")

                                });
                            }
                            return Ok(trains);
                        }
                        else
                        {
                            return BadRequest(new { message = "Train not found" });
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return StatusCode(500);
                }
            }
        }

        [HttpDelete("delete_train_schedule")]
        public async Task<IActionResult> Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "DELETE FROM TrainSchedule WHERE Journey_ID = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        await cmd.ExecuteNonQueryAsync();
                        return Ok(new { message = "Train " + id + " deleted successfully" });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return StatusCode(500);
                }
            }
        }


        [HttpPut("update_train_schedule")]
        public async Task<IActionResult> Update([FromBody] Train_Schedule request)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "UPDATE TrainSchedule SET Train_ID = @Train_ID, StartStation = @startStation, DestinationStation = @destinationstation, journey_data = @journeydate, DepartureTime = @departureTime, DestinationArrivalTime = @DestinationArrivalTime WHERE Journey_ID = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Train_ID", request.Train_ID);
                        cmd.Parameters.AddWithValue("@startStation", request.StartStation);
                        cmd.Parameters.AddWithValue("@destinationstation", request.DestinationStation);
                        cmd.Parameters.AddWithValue("@journeydate", request.JourneyDate);
                        cmd.Parameters.AddWithValue("@departureTime", request.DepartureTime);
                        cmd.Parameters.AddWithValue("@DestinationArrivalTime", request.DestinationArrivalTime);
                        cmd.Parameters.AddWithValue("@id", request.Journey_ID);
                        await cmd.ExecuteNonQueryAsync();
                        return Ok(new { message = "Train " + request.Train_ID + " updated successfully" });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return StatusCode(500);
                }
            }
        }




    }


    public class Train_Schedule
    {
        public int Journey_ID { get; set; }
        public int Train_ID { get; set; }
        public string StartStation { get; set; }
        public string DestinationStation { get; set; }
        public DateTime JourneyDate { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan DestinationArrivalTime { get; set; }
    }

    public class Train_Schedule_show
    {
        public int Journey_ID { get; set; }
        public int Train_ID { get; set; }
        public string StartStation { get; set; }
        public string DestinationStation { get; set; }
        public DateTime JourneyDate { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan DestinationArrivalTime { get; set; }
        public string TrainName { get; set; }
    }
}
