using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace railway_eservice_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BookingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

       
        [HttpPost("add_booking")]
        public async Task<IActionResult> add_Booking([FromBody] Booking request)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database");

                    string insertQuery = "INSERT INTO Booking (Journey_ID, user_id, BookingDate, BookingTime, Class, NoOfTickets, TotalPrice)\r\nVALUES \r\n(@Journey_ID , @User_ID, getdate(), FORMAT(GETDATE(), 'HH:mm:ss'), @Class, @NoOfTickets, @TotalPrice);";
                    using (SqlCommand insertUserCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertUserCommand.Parameters.AddWithValue("@Journey_ID", request.Journey_ID);
                        insertUserCommand.Parameters.AddWithValue("@User_ID", request.User_ID);
                        insertUserCommand.Parameters.AddWithValue("@Class", request.Class);
                        insertUserCommand.Parameters.AddWithValue("@NoOfTickets", request.NoOfTickets);
                        insertUserCommand.Parameters.AddWithValue("@TotalPrice", request.TotalPrice);
                        await insertUserCommand.ExecuteNonQueryAsync();

                        return Ok(new { message = "Booking added successfully" });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return StatusCode(500);
                }
            }
        }


        [HttpGet("get_available_trains")]
        public async Task<IActionResult> get_available_trains([FromQuery] string startStation, string destinationStation, DateTime journeyDate)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                List<show_available_trains> trains_list = new List<show_available_trains>();
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database");

                    string selectQuery = "SELECT * FROM " +
                        "(SELECT   " +
                        "journey_id,train_info.train_id,train_name,StartStation,DestinationStation,journey_data,DepartureTime,DestinationArrivalTime,total_seat,eco_seat,business_seat,first_class_seat,total_coach   " +
                        " FROM TrainSchedule JOIN Train_info ON TrainSchedule.Train_ID = Train_info.train_id   " +
                        " WHERE  StartStation = @startStation    AND DestinationStation = @destinationStation   AND journey_data = @journeyDate) AS train_info_schedule " +
                        " JOIN " +
                        "(SELECT journey_id, SUM(CASE WHEN class = 'Economy' THEN noOfTickets END) AS economy_seat,SUM(CASE WHEN class = 'Business' THEN noOfTickets END) AS business_seat," +
                        " SUM(CASE WHEN class = 'First Class' THEN noOfTickets END) AS first_class_seat" +
                        " FROM ( SELECT journey_id, class,  COUNT(class) AS ClassCount, SUM(nooftickets) AS noOfTickets" +
                        "   FROM  (  SELECT * FROM booking WHERE BookingDate = @journeyDate2) AS new_booking  " +
                        " GROUP BY new_booking.journey_id, new_booking.class " +
                        " HAVING new_booking.journey_id IN (  SELECT Journey_ID  FROM TrainSchedule  WHERE StartStation = @startStation2 AND DestinationStation = @destinationStation2   AND journey_data = @journeyDate3 )) AS summary_table " +
                        "  GROUP BY journey_id) AS vacancy_table ON train_info_schedule.Journey_ID = vacancy_table.journey_id;";
                    using (SqlCommand selectUserCommand = new SqlCommand(selectQuery, connection))
                    {
                        
                        selectUserCommand.Parameters.AddWithValue("@startStation", startStation);
                        selectUserCommand.Parameters.AddWithValue("@destinationStation", destinationStation);
                        selectUserCommand.Parameters.AddWithValue("@journeyDate", journeyDate);
                        selectUserCommand.Parameters.AddWithValue("@journeyDate2", journeyDate);
                        selectUserCommand.Parameters.AddWithValue("@startStation2", startStation);
                        selectUserCommand.Parameters.AddWithValue("@destinationStation2", destinationStation);
                        selectUserCommand.Parameters.AddWithValue("@journeyDate3", journeyDate);
                      //  selectUserCommand.ExecuteNonQuery();
                         Console.WriteLine(selectUserCommand.CommandText);
                        Console.WriteLine(selectUserCommand.Parameters);
                        SqlDataReader reader = (SqlDataReader)await selectUserCommand.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {   
                            while (await reader.ReadAsync())
                            {
                                trains_list.Add(new show_available_trains()
                                {
                                    JourneyId = reader.GetInt32(0),
                                    TrainId = reader.GetInt32(1),
                                    TrainName = reader.GetString(2),
                                    StartStation = reader.GetString(3),
                                    DestinationStation = reader.GetString(4),
                                    JourneyDate = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    DepartureTime = reader.GetTimeSpan(6),
                                    DestinationArrivalTime = reader.GetTimeSpan(7),
                                    TotalSeat = reader.GetInt32(8),
                                    EcoSeat = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                    BusinessSeat = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                                    FirstClassSeat = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                                    TotalCoach = reader.IsDBNull(12) ? 0 : reader.GetInt32(12),
                                    EconomySeatBooked = reader.IsDBNull(14) ? 0 : reader.GetInt32(14),
                                    BusinessSeatBooked = reader.IsDBNull(15) ? 0 : reader.GetInt32(15),
                                    FirstClassSeatBooked = reader.IsDBNull(16) ? 0 : reader.GetInt32(16)
                                });

                            }
                            return Ok(trains_list);
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


        [HttpDelete("delete_booking")]
        public async Task<IActionResult> Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                    string deleteQuery = "DELETE FROM Booking WHERE Booking_ID = @id";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@id", id);
                        await deleteCommand.ExecuteNonQueryAsync();
                        return Ok(new { message = "Booking deleted successfully" });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return StatusCode(500);
                }
            }
        }

        [HttpPut("update_booking")]
        public async Task<IActionResult> Update([FromBody] Booking request)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                    string updateQuery = "UPDATE Booking SET Journey_ID = @Journey";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Journey", request.Journey_ID);
                        await updateCommand.ExecuteNonQueryAsync();
                        return Ok(new { message = "Booking updated successfully" });
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return StatusCode(500);
                }
            }
        }


        //select* from fare_info

        [HttpGet("get_fare")]
        public async Task<IActionResult> get_fare()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                List<Fare> fare_list = new List<Fare>();
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database");

                    string selectQuery = "SELECT * FROM fare_info";
                    using (SqlCommand selectUserCommand = new SqlCommand(selectQuery, connection))
                    {
                        SqlDataReader reader = (SqlDataReader)await selectUserCommand.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                fare_list.Add(new Fare()
                                {
                                    Class_Type = reader.GetString(1),
                                    Fare_Amount = reader.GetInt32(2)
                                });

                            }
                            return Ok(fare_list);
                        }
                        else
                        {
                            return BadRequest(new { message = "Fare not found" });
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


        //select sum(NoOfTickets) from booking where BookingDate = getdate()
     /*   [HttpGet("get_total_booking")]
        public async Task<IActionResult> get_total_booking()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database");

                    string selectQuery = "SELECT sum(NoOfTickets) FROM booking WHERE BookingDate = getdate()";
                    using (SqlCommand selectUserCommand = new SqlCommand(selectQuery, connection))
                    {
                        SqlDataReader reader = (SqlDataReader)await selectUserCommand.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                return Ok(new { TotalBooking = reader.GetInt32(0) });
                            }
                        }
                        else
                        {
                            return BadRequest(new { message = "Booking not found" });
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
        */

        





    }

    public class Fare
    {
      
        public string Class_Type { get; set; }
        public int Fare_Amount { get; set; }
    }


    public class Booking
    {
        public int Booking_ID { get; set; }
        public int Journey_ID { get; set; }
        public int User_ID { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan BookingTime { get; set; }
        public string Class { get; set; }
        public int NoOfTickets { get; set; }
        public float TotalPrice { get; set; }
    }

    public class show_available_trains
    {
        public int JourneyId { get; set; }
        public int TrainId { get; set; }
        public string TrainName { get; set; }
        public string StartStation { get; set; }
        public string DestinationStation { get; set; }
        public string JourneyDate { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan DestinationArrivalTime { get; set; }
        public int TotalSeat { get; set; }
        public int EcoSeat { get; set; }
        public int BusinessSeat { get; set; }
        public int FirstClassSeat { get; set; }
        public int TotalCoach { get; set; }
        public int EconomySeatBooked { get; set; }
        public int BusinessSeatBooked { get; set; }
        public int FirstClassSeatBooked { get; set; }
    }

}
