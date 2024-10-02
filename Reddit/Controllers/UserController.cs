using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync<User>();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return NoContent();
            }

            return Ok(user);
        }


        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("userDto null");
            }

            var user = new User()
            {
                Name = userDto.Name,
                Email = userDto.Email,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(UserDto user, int id)
        {
            if (user == null)
            {
                return BadRequest($"UserDto null");
            }


            var userDto = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

            if (userDto is null)
            {
                return NotFound();
            }

            userDto.Name = user.Name;
            userDto.Email = user.Email;


            _context.Entry(userDto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok($"user with the Id:{id} updated successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                return NotFound($"No user found with the Id: {id}");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"User with the Id: {id} deleted successfully!");
        }
    }
}
