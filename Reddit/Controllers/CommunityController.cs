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
    public class CommunityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommunityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Community>>> GetCommunities()
        {
            return await _context.Communities.ToListAsync<Community>();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Community>> GetCommunity(int id)
        {
            var community = await _context.Communities.FindAsync(id);
            if (community == null)
            {
                return NotFound();
            }

            return Ok(community);
        }

        [HttpPost]
        public async Task<ActionResult<Community>> PostCommunity(CommunityDto communityDto)
        {
            if (communityDto == null)
            {
                return NoContent();
            }

            var community = new Community()
            {
                Name = communityDto.Name,
                Description = communityDto.Description,
                OwnerId = communityDto.OwnerId,
            };

            _context.Communities.Add(community);
            await _context.SaveChangesAsync();

            return Ok($"Created Successfully!");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Community>> UpdateCommunity(CommunityDto communityDto, int id)
        {
            if(communityDto == null)
            {
                return NoContent();
            }

            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == id);

            if(community == null)
            {
                return NotFound($"No community exists with the following Id: {id}");
            }

            community.Name = communityDto.Name;
            community.Description = communityDto.Description;
            community.OwnerId = communityDto.OwnerId;

            _context.Entry(community).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok($"Community with the id:{id} updated successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Community>> DeleteCommunity(int id)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == id);

            if(community is null)
            {
                return NotFound($"No community exists with the following Id: {id}");
            }

            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();

            return Ok($"Community deleted successfully!");
        }
    }
}
