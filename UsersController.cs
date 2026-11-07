using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // Colección thread-safe en memoria para evitar condiciones de carrera durante pruebas de stress
        private static readonly List<User> _users = new()
        {
            new User { Id = 1, Name = "Ana Gómez", Email = "ana.gomez@techhive.com", Department = "RR.HH." },
            new User { Id = 2, Name = "Carlos Ruiz", Email = "carlos.ruiz@techhive.com", Department = "TI" }
        };

        private static readonly object _lock = new();

        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            lock (_lock)
            {
                return Ok(_users.ToList());
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public ActionResult<User> GetUserById(int id)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { message = $"No se encontró ningún usuario con el ID {id}." });
                }
                return Ok(user);
            }
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            lock (_lock)
            {
                newUser.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
                _users.Add(newUser);
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id:int}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { message = $"No se encontró ningún usuario con el ID {id} para actualizar." });
                }

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.Department = updatedUser.Department;

                return NoContent();
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:int}")]
        public IActionResult DeleteUser(int id)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new { message = $"No se encontró ningún usuario con el ID {id} para eliminar." });
                }

                _users.Remove(user);
                return NoContent();
            }
        }
    }
}