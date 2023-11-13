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

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get list of users
        /// </summary>
        /// <remarks>
        /// It must choose the the parameters to find users. Use q field to insert commands
        ///
        /// It is possible insert searches by key and value, use ':' to separate the key and value.
        /// For example: ?q=name:Mary
        /// It return all users where the name is equal to Mary.
        ///
        /// It is possible use '+' (and comparator) and '|' (or comparator) to join more than
        /// one field on the search. Just a warning, don't mix '+' and '-' at the same query,
        /// it's not possible.
        /// For example: ?q=name:Mary+city:London
        ///              ?q=name:Mary|city:London
        ///
        /// It is possible split the values on the search. Use the '*' to split the search string.
        /// For example:?q=name:A*b*c
        /// Return all users where the name starts with 'A', contains 'B' and ends with 'c'.
        /// The values are no case sensitive.
        /// 
        /// </remarks>
        /// <response code="200">Returns the list of the users</response>
        /// <response code="400">If the parameter q is invalid</response>
        /// <response code="500">If server error</response> 
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
                        var values = line.Split(',');

                        var user = new User
                        {
                            name = values[0],
                            city = values[1],
                            country = values[2],
                            favorite_sport = values[3]
                        };

                        _users.Add(user);
                    }

                    System.IO.File.WriteAllText(FilePath, JsonConvert.SerializeObject(_users));
                }

                return Ok("CSV file uploaded successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { messageError = ex.Message });
            }
        }

    }
}