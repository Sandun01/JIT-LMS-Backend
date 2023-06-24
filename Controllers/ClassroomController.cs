using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using System.Data;
using MySql.Data.MySqlClient;

namespace lms_backend.Controllers
{
    [Controller]
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
        public Object Get()
        {
            _logger.LogInformation("ClassroomController:Get All Classes");
            try 
            { 
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = "SELECT * FROM classrooms";

                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        mySqlDataReader = mySqlCommand.ExecuteReader();
                        dataTable.Load(mySqlDataReader);
                        mySqlDataReader.Close();
                        _connection.Close();
                    }
                    
                    string json = _helper.ConvertDataTableToJson(dataTable);
                    Console.WriteLine(json);
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
        public Object Get(string id)
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
                    Console.WriteLine(json);
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
 
    }
}
