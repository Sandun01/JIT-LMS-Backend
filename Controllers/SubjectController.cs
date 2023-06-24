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
    public class SubjectController : Controller
    {
        private readonly ILogger<SubjectController> _logger;
        private IConfiguration _configuration;
        private MySqlConnection _connection;
        private Helper _helper = new Helper();


        public SubjectController(ILogger<SubjectController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            _connection = new MySqlConnection(connectionString);
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("SubjectController: Get All Subjects");
            try 
            { 
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = "SELECT * FROM subjects";

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
                 _logger.LogInformation("Exception: Get All Subjects | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            _logger.LogInformation("SubjectController:Get Subject by ID");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = "SELECT * FROM subjects Where subject_id = "+ id;

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
                 _logger.LogInformation("Exception: Get Subject By ID | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] dynamic SubjectData)
        {
            _logger.LogInformation("SubjectController: Create Subject");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                string subject_name = _helper.getJsonProperty("subject_name", "String", SubjectData);
                // Console.WriteLine(subject_name);
                
                if(!string.IsNullOrWhiteSpace(subject_name)){
                    using(_connection) {  
                        _connection.Open();       
                        String query = @$" 
                            Insert Into subjects(subject_name)
                            Values('{subject_name}');
                        ";

                        int rowsAffected = 0;
                        using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                            rowsAffected = mySqlCommand.ExecuteNonQuery();
                        }
                        
                        if(rowsAffected > 0){
                            _logger.LogInformation("SubjectController: Subject Created!");
                            // Console.WriteLine("Success!");
                            return Ok();
                        }else{
                            _logger.LogInformation("SubjectController: Can't Create Subject");
                            return BadRequest();
                        }
                    }
                }else{
                    _logger.LogInformation("SubjectController: Please Enter Subject Name!");
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Error!",
                        Detail = "Please Enter Subject Name!",
                        Status = 400
                    };
                    return BadRequest(problemDetails);
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Create Subject | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }
 
    }
}
