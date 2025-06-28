using CarteraCrypto_Api.DTOs;
using CarteraCrypto_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarteraCrypto_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClientController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetClient() 
        {
            var clients = await _context.Clients.ToListAsync();
            var clientDto = clients.Select(c => new ClientDto 
            {
                
                name = c.name,
                email = c.email
            }).ToList();
            return Ok(clientDto);
        }

        [HttpPost]
        public async Task<ActionResult<Client>> Post(Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClient), new { id = client.id }, client);
        }


    }
}
