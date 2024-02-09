using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Linq.Dynamic.Core;
using CsvFileApi.Models;
using CsvFileApi.Services;

namespace CsvFileApi.Controllers
{
    /// <summary>
    /// Users Controller
    /// </summary>
    [ApiController]
    public class UsersController : ControllerBase
    {
        private List<User> _users = new List<User>();

        private const string FilePath = "users.json";

        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger) => _logger = logger;

        /// <summary>
        /// Get list of users
        /// </summary>
        /// <remarks>
        /// You can choose the parameters to find users. Use "q" field to insert parameters.
        /// 
        /// If the “q” parameter is empty, the query will return all registered users.
        /// 
        /// 
        /// It is possible to enter searches by key and value, use ':' to separate key and value.
        ///
        /// 
        /// Example:
        /// 
        ///              ?q=name:Mary
        ///
        /// 
        /// This query returns all users whose name is the same as Mary.
        ///
        /// 
        /// It is possible to use '+' (and operator) and '|' (or operator) to enter more than
        /// a field in the search. Just a warning, don't mix '+' and '|' in the same query,
        /// not allowed.
        /// 
        /// Example:
        /// 
        ///              ?q=name:Mary+city:London
        /// 
        ///              ?q=name:Mary|city:London
        /// 
        /// It is possible split the values in the search. Use the '*' to split the search string.
        /// 
        /// Example:
        /// 
        ///              ?q=name:A*b*c
        /// 
        /// This query returns all users whose name starts with 'A', contains 'b' and ends with 'c'.
        ///
        ///  
        /// The values are case insensitive.
        /// 
        /// </remarks>
        /// <response code="200">Success, returns the filtered list of the users</response>
        /// <response code="400">Invalid "q" parameter</response>
        /// <response code="500">Server error</response> 
        [HttpGet]
        [Route("api/Users/")]
        public IActionResult GetUsers([FromQuery] string? q)
        {
            try
            {
                if (!System.IO.File.Exists(FilePath))
                    return Ok("File not found! None user found!");

                if (_users.Count == 0)
                    _users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(FilePath))!;

                if (string.IsNullOrWhiteSpace(q))
                    return Ok(_users);

                int status;
                string message;
                string condition = Service.QueryApi2Linq(q, out status, out message);

                if (status == 400)
                    return StatusCode(400, new { messageError = message });

                var filteredUsers = _users
                    .AsQueryable()
                    .Where(condition)
                    .ToList();

                return Ok(filteredUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { messageError = ex.Message });
            }
        }

        /// <summary>
        /// Upload the CSV file
        /// </summary>
        /// <remarks>
        /// Use the "/api/Files" endpoint to upload the CSV file.
        /// </remarks>
        /// <response code="201">Successful file upload</response>
        /// <response code="500">Server error</response>
        [HttpPost]
        [Route("api/Files/")]
        public IActionResult UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File not provided!");

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    _users.Clear();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        string[] values = line.Split(',');

                        var user = new User(values[0], values[1], values[2], values[3]);

                        _users.Add(user);
                    }

                    System.IO.File.WriteAllText(FilePath, JsonConvert.SerializeObject(_users));
                }

                return CreatedAtAction( null , new { fileName = file.FileName }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { messageError = ex.Message });
            }
        }
    }
}