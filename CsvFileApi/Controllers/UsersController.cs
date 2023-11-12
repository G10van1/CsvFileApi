using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Linq.Dynamic.Core;
using CsvFileApi.Models;
using CsvFileApi.Services;

namespace CsvFileApi.Controllers
{
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

                string condition = Service.QueryApi2Linq(q);

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
                            Name = values[0],
                            City = values[1],
                            Country = values[2],
                            FavoriteSport = values[3]
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