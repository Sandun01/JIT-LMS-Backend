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
    public class TeacherController : Controller
    {
        private readonly ILogger<TeacherController> _logger;
        private IConfiguration _configuration;
        private MySqlConnection _connection;
        private Helper _helper = new Helper();


        public TeacherController(ILogger<TeacherController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            _connection = new MySqlConnection(connectionString);
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("TeacherController: Get All Teachers");
            try 
            { 
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = "SELECT * FROM teachers ORDER BY teacher_id DESC";

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
                 _logger.LogInformation("Exception: Get All Teachers | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            _logger.LogInformation("TeacherController:Get Teacher by ID");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = "SELECT * FROM teachers Where teacher_id = "+ id;

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
                 _logger.LogInformation("Exception: Get Teacher By ID | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] dynamic TeacherData)
        {
            _logger.LogInformation("TeacherController: Create Teacher");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                string first_name = _helper.getJsonProperty("first_name", "String", TeacherData);
                string last_name = _helper.getJsonProperty("last_name", "String", TeacherData);
                string contact_no = _helper.getJsonProperty("contact_no", "String", TeacherData);
                string email = _helper.getJsonProperty("email", "String", TeacherData);
                // Console.WriteLine(first_name);
                
                if(!string.IsNullOrWhiteSpace(first_name) 
                    && !string.IsNullOrWhiteSpace(last_name) 
                    && !string.IsNullOrWhiteSpace(email)
                    && !string.IsNullOrWhiteSpace(contact_no)
                ){
                    using(_connection) {  
                        _connection.Open();       
                        String query = @$" 
                            Insert Into teachers(first_name, last_name, contact_no, email)
                            Values('{first_name}', '{last_name}', '{contact_no}', '{email}');
                        ";

                        int rowsAffected = 0;
                        using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                            rowsAffected = mySqlCommand.ExecuteNonQuery();
                        }
                        
                        if(rowsAffected > 0){
                            _logger.LogInformation("TeacherController: Teacher Created!");
                            // Console.WriteLine("Success!");
                            return Ok();
                        }else{
                            _logger.LogInformation("TeacherController: Can't Create Teacher");
                            return BadRequest();
                        }
                    }
                }else{
                    _logger.LogInformation("TeacherController: One or more data fields are missing!");
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
                _logger.LogInformation("Exception: Create Teacher | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
             _logger.LogInformation("TeacherController: Delete Teacher");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                int teacher_id = id;
                using(_connection) {  
                    _connection.Open();       
                    String query = @$" 
                        DELETE FROM teachers WHERE teacher_id = {teacher_id};
                    ";

                    int rowsAffected = 0;
                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        rowsAffected = mySqlCommand.ExecuteNonQuery();
                    }
                    
                    if(rowsAffected > 0){
                        _logger.LogInformation("TeacherController: Deleted!");
                        // Console.WriteLine("Success!");
                        return Ok();
                    }else{
                        _logger.LogInformation("TeacherController: Can't Delete Teacher");
                        return BadRequest();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Delete Teacher | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }
 
        [HttpPost("/api/[controller]/subjects")]
        public IActionResult AllocateSubject([FromBody] dynamic data)
        {
            _logger.LogInformation("TeacherController: Allocate Teacher Subjects");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                int subject_id = _helper.getJsonProperty("subject_id", "Int", data);
                int teacher_id = _helper.getJsonProperty("teacher_id", "Int", data);
             
                using(_connection) {  
                    _connection.Open();       
                    String query = @$" 
                        Insert Into teacher_subject(teacher_id, subject_id)
                        Values('{teacher_id}', '{subject_id}');
                    ";

                    int rowsAffected = 0;
                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        rowsAffected = mySqlCommand.ExecuteNonQuery();
                    }
                    
                    if(rowsAffected > 0){
                        _logger.LogInformation("TeacherController: Teacher Subject Allocated!");
                        // Console.WriteLine("Success!");
                        return Ok();
                    }else{
                        _logger.LogInformation("TeacherController: Can't Allocate Subject to Teacher");
                        return BadRequest();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Allocate Subject Teacher | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpDelete("/api/[controller]/subjects/{teacher_id}/{subject_id}")]
        public IActionResult DeallocateSubject(int teacher_id, int subject_id)
        {
            _logger.LogInformation("TeacherController: Deallocate Teacher Subjects");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using(_connection) {  
                    _connection.Open();       
                    String query = @$" 
                        DELETE FROM teacher_subject
                        WHERE teacher_id = '{teacher_id}' AND subject_id = '{subject_id}'
                    ";

                    int rowsAffected = 0;
                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        rowsAffected = mySqlCommand.ExecuteNonQuery();
                    }
                    
                    if(rowsAffected > 0){
                        _logger.LogInformation("TeacherController: Teacher Subject Deallocated!");
                        // Console.WriteLine("Success!");
                        return Ok();
                    }else{
                        _logger.LogInformation("TeacherController: Can't Deallocate Subject to Teacher");
                        return BadRequest();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Deallocate Subject Teacher | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpGet("/api/[controller]/{id}/subjects")]
        public IActionResult GetAllocatedSubjects(string id)
        {
            _logger.LogInformation("TeacherController:Get Teacher Subjects by ID");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = @$"
                        SELECT * FROM teacher_subject
                        LEFT JOIN subjects ON teacher_subject.subject_id = subjects.subject_id
                        WHERE teacher_subject.teacher_id = {id}
                    ";

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
                 _logger.LogInformation("Exception: Get Teacher Subjects By ID | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpGet("/api/[controller]/{id}/classrooms")]
        public IActionResult GetAllocatedClassrooms(string id)
        {
            _logger.LogInformation("TeacherController:Get Teacher Classrooms by ID");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                MySqlDataReader mySqlDataReader;
                
                using(_connection) {  
                    _connection.Open();       
                    String query = @$"
                        SELECT * FROM teacher_classroom
                        LEFT JOIN classrooms ON teacher_classroom.classroom_id = classrooms.classroom_id
                        WHERE teacher_classroom.teacher_id = {id}
                    ";

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
                 _logger.LogInformation("Exception: Get Teacher Classrooms By ID | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpPost("/api/[controller]/classrooms")]
        public IActionResult AllocateClassroom([FromBody] dynamic data)
        {
            _logger.LogInformation("TeacherController: Allocate Teacher Classroom");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                int classroom_id = _helper.getJsonProperty("classroom_id", "Int", data);
                int teacher_id = _helper.getJsonProperty("teacher_id", "Int", data);
             
                using(_connection) {  
                    _connection.Open();       
                    String query = @$" 
                        Insert Into teacher_classroom(teacher_id, classroom_id)
                        Values('{teacher_id}', '{classroom_id}');
                    ";

                    int rowsAffected = 0;
                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        rowsAffected = mySqlCommand.ExecuteNonQuery();
                    }
                    
                    if(rowsAffected > 0){
                        _logger.LogInformation("TeacherController: Teacher Classroom Allocated!");
                        // Console.WriteLine("Success!");
                        return Ok();
                    }else{
                        _logger.LogInformation("TeacherController: Can't Allocate Classroom to Teacher");
                        return BadRequest();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Allocate Subject Teacher | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

        [HttpDelete("/api/[controller]/classrooms/{teacher_id}/{classroom_id}")]
        public IActionResult DeallocateClassroom(int teacher_id, int classroom_id)
        {
            _logger.LogInformation("TeacherController: Deallocate Teacher Classroom");
            try
            {
                DataTable dataTable = new DataTable();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using(_connection) {  
                    _connection.Open();       
                    String query = @$" 
                        DELETE FROM teacher_classroom
                        WHERE teacher_id = '{teacher_id}' AND classroom_id = '{classroom_id}'
                    ";

                    int rowsAffected = 0;
                    using(MySqlCommand mySqlCommand = new MySqlCommand(query, _connection)){
                        rowsAffected = mySqlCommand.ExecuteNonQuery();
                    }
                    
                    if(rowsAffected > 0){
                        _logger.LogInformation("TeacherController: Teacher Subject Deallocated!");
                        // Console.WriteLine("Success!");
                        return Ok();
                    }else{
                        _logger.LogInformation("TeacherController: Can't Deallocate Classroom from Teacher");
                        return BadRequest();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("Exception: Deallocate Classroom Teacher | "+ e.ToString());
                Console.WriteLine(e.ToString());
                return BadRequest();
            }
        }

    }

}
