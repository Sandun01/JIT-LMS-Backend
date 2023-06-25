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
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;
        private IConfiguration _configuration;
        private MySqlConnection _connection;
        private Helper _helper = new Helper();


        public StudentController(ILogger<StudentController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            _connection = new MySqlConnection(connectionString);
        }

        [HttpGet]
        public Object Get()
        {
            _logger.LogInformation("StudentController:Get All Classes");
            try 
            { 
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = @"
                        SELECT * FROM students
                        LEFT JOIN classrooms ON students.classroom_id = classrooms.classroom_id
                        ORDER BY student_id DESC
                    ";

                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        mySqlDataReader = mySqlCommand.ExecuteReader();
                        dataTable.Load(mySqlDataReader);
                        mySqlDataReader.Close();
                        _connection.Close();
                    }
                    
                    string json = _helper.ConvertDataTableToJson(dataTable);
                    // Console.WriteLine(json);
                    return Content(json, "application/json");
                }
            }
            catch (Exception e)
            {
                 _logger.LogInformation("Exception: Get all Students | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public Object Get(string id)
        {
            _logger.LogInformation("StudentController:Get class by ID");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       

                     String query = @$"
                        SELECT * FROM students
                        LEFT JOIN classrooms ON students.classroom_id = classrooms.classroom_id
                        Where student_id = {id}
                    ";

                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        mySqlDataReader = mySqlCommand.ExecuteReader();
                        dataTable.Load(mySqlDataReader);
                        mySqlDataReader.Close();
                        _connection.Close();
                    }
                    
                    string json = _helper.ConvertDataTableToJson(dataTable);
                    // Console.WriteLine(json);
                    return Content(json, "application/json");
                }
                
            }
            catch (Exception e)
            {
                 _logger.LogInformation("Exception: Get Student by ID | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] dynamic student)
        {
            _logger.LogInformation("StudentController: Create Student");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                string first_name = _helper.getJsonProperty("first_name", "String", student);
                string last_name = _helper.getJsonProperty("last_name", "String", student);
                string contact_person = _helper.getJsonProperty("contact_person", "String", student);
                string contact_no = _helper.getJsonProperty("contact_no", "String", student);
                string email = _helper.getJsonProperty("email", "String", student);
                string dob = _helper.getJsonProperty("dob", "String", student);

                int age = _helper.getJsonProperty("age", "Int", student);
                int classroom_id = _helper.getJsonProperty("classroom_id", "Int", student);
                
                if(!string.IsNullOrWhiteSpace(first_name) 
                    && !string.IsNullOrWhiteSpace(last_name) 
                    && !string.IsNullOrWhiteSpace(contact_person)
                    && !string.IsNullOrWhiteSpace(contact_no)
                    && !string.IsNullOrWhiteSpace(email)
                    && !string.IsNullOrWhiteSpace(dob)
                ){
                    using(_connection) {  
                        _connection.Open();       
                        String query = @$" 
                            Insert Into students(first_name, last_name, contact_person, contact_no, email, dob , age, classroom_id)
                            Values('{first_name}', '{last_name}', '{contact_person}', '{contact_no}', '{email}', '{dob}', {age}, {classroom_id});
                        ";

                        int rowsAffected = 0;
                        using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                            rowsAffected = mySqlCommand.ExecuteNonQuery();
                        }
                        
                        if(rowsAffected > 0){
                            _logger.LogInformation("StudentController: Student Created!");
                            // Console.WriteLine("Success!");
                            return Ok();
                        }else{
                            _logger.LogInformation("StudentController: Can't Create Student");
                            return BadRequest();
                        }
                    }
                }else{
                    _logger.LogInformation("StudentController: One or more data fields are missing!");
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Error!",
                        Detail = "One or more data fields are missing, Please Re check all data added.",
                        Status = 400
                    };
                    return BadRequest(problemDetails);
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Create Student | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
             _logger.LogInformation("StudentController: Delete Student");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                int student_id = id;
                using(_connection) {  
                    _connection.Open();       
                    String query = @$" 
                        DELETE FROM students WHERE student_id = {student_id};
                    ";

                    int rowsAffected = 0;
                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        rowsAffected = mySqlCommand.ExecuteNonQuery();
                    }
                    
                    if(rowsAffected > 0){
                        _logger.LogInformation("StudentController: Deleted!");
                        // Console.WriteLine("Success!");
                        return Ok();
                    }else{
                        _logger.LogInformation("StudentController: Can't Delete Student");
                        return BadRequest();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Delete Student | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }
 
    }
}
