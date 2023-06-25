using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using System.Data;
using MySql.Data.MySqlClient;

namespace lms_backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ClassroomController : Controller
    {
        private readonly ILogger<ClassroomController> _logger;
        private IConfiguration _configuration;
        private MySqlConnection _connection;
        private Helper _helper = new Helper();


        public ClassroomController(ILogger<ClassroomController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            _connection = new MySqlConnection(connectionString);
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("ClassroomController:Get All Classes");
            try 
            { 
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = "SELECT * FROM classrooms ORDER BY classroom_id DESC";

                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        mySqlDataReader = mySqlCommand.ExecuteReader();
                        dataTable.Load(mySqlDataReader);
                        mySqlDataReader.Close();
                        _connection.Close();
                    }
                    
                    string json = _helper.ConvertDataTableToJson(dataTable);
                    return Content(json, "application/json");
                }
            }
            catch (Exception e)
            {
                 _logger.LogInformation("Exception: Get all Classrooms | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            _logger.LogInformation("ClassroomController:Get class by ID");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = "SELECT * FROM classrooms Where classroom_id = "+ id;

                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        mySqlDataReader = mySqlCommand.ExecuteReader();
                        dataTable.Load(mySqlDataReader);
                        mySqlDataReader.Close();
                        _connection.Close();
                    }
                    
                    string json = _helper.ConvertDataTableToJson(dataTable);
                    return Content(json, "application/json");
                }
                
            }
            catch (Exception e)
            {
                 _logger.LogInformation("Exception: Get class by ID | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] dynamic classroomData)
        {
            _logger.LogInformation("ClassroomController: Create Classroom");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                string classroom_name = _helper.getJsonProperty("classroom_name", "String", classroomData);
                // Console.WriteLine(classroom_name);
                
                if(!string.IsNullOrWhiteSpace(classroom_name)){
                    using(_connection) {  
                        _connection.Open();       
                        String query = @$" 
                            Insert Into classrooms(classroom_name)
                            Values('{classroom_name}');
                        ";

                        int rowsAffected = 0;
                        using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                            rowsAffected = mySqlCommand.ExecuteNonQuery();
                        }
                        
                        if(rowsAffected > 0){
                            _logger.LogInformation("ClassroomController: Classroom Created!");
                            // Console.WriteLine("Success!");
                            return Ok();
                        }else{
                            _logger.LogInformation("ClassroomController: Can't Create Classroom");
                            return BadRequest();
                        }
                    }
                }else{
                    _logger.LogInformation("ClassroomController: Please Enter Classroom Name!");
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Error!",
                        Detail = "Please Enter Classroom Name.",
                        Status = 400
                    };
                    return BadRequest(problemDetails);
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Create Classroom | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
             _logger.LogInformation("ClassroomController: Delete Classroom");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                int classroom_id = id;
                using(_connection) {  
                    _connection.Open();       
                    String query = @$" 
                        DELETE FROM classrooms WHERE classroom_id = {classroom_id};
                    ";

                    int rowsAffected = 0;
                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        rowsAffected = mySqlCommand.ExecuteNonQuery();
                    }
                    
                    if(rowsAffected > 0){
                        _logger.LogInformation("ClassroomController: Deleted!");
                        // Console.WriteLine("Success!");
                        return Ok();
                    }else{
                        _logger.LogInformation("ClassroomController: Can't Delete Classroom");
                        return BadRequest();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Delete Classroom | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }
 
    }
}
