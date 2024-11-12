using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;
using Reddit.Repositories;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICommunityRepository _communityRepository;
        public CommunityController(AppDbContext context, ICommunityRepository communityRepository)
        {
            this._context = context;
            this._communityRepository = communityRepository;
        }

        [HttpGet]
        public async Task<PagedList<Community>> GetCommunities(int pageNumber = 1, int pageSize = 3, bool? isAscending = null, string? sortKey = null, string? searchKey = null)
        {
            return await _communityRepository.GetCommunities(pageNumber, pageSize, isAscending, sortKey, searchKey);
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Community>>> GetCommunities()
        //{
        //    return await _context.Communities.ToListAsync();
        //}

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
