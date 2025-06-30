using CarteraCrypto_Api.DTOs;
using CarteraCrypto_Api.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Transactions;

namespace CarteraCrypto_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoTransactionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CryptoTransactionController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> Get()
        {
            var transactions = await _context.Transactions
                    .Include(t => t.Client)
                    .OrderByDescending(t => t.datetime)
                    .ToListAsync();

            var transactionDto = transactions.Select(t => new TransactionResponseDto
            {
                id = t.id,
                cryptoCode = t.cryptoCode,
                action = t.action,
                cryptoAmount = t.cryptoAmount,
                money = t.money,
                datetime = t.datetime,
                clientId = t.clientId,
                clientName = t.Client?.name

            }).ToList();
            return Ok(transactionDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponseDto>> Get(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Client)
                .FirstOrDefaultAsync(t => t.id == id);

            if (transaction == null)
            {
                return NotFound();
            }
            var responseDto = new TransactionResponseDto
            {
                id = transaction.id,
                cryptoCode = transaction.cryptoCode,
                action = transaction.action,
                cryptoAmount = transaction.cryptoAmount,
                money = transaction.money,
                datetime = transaction.datetime,
                clientId = transaction.clientId,
                clientName = transaction.Client?.name
            };
            return Ok(responseDto);
        }

        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetByClientId(int clientId)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.id == clientId);
            if (!clientExists)
            {
                return NotFound($"No se encontro el cliente con el Id {clientId}");
            }

            var transactions = await _context.Transactions
                .Include(t => t.Client).Where(t => t.clientId == clientId)
                .OrderByDescending(t => t.datetime).ToListAsync();

            var responseList = transactions.Select(t => new TransactionResponseDto
            {
                id = t.id,
                cryptoCode = t.cryptoCode,
                action = t.action,
                cryptoAmount = t.cryptoAmount,
                money = t.money,
                datetime = t.datetime,
                clientId = t.clientId,
                clientName = t.Client?.name
            }).ToList();
            return Ok(responseList);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTransaction(int id, TransactionUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var transaction = await _context.Transactions
                .Include(t => t.Client)
                .FirstOrDefaultAsync(t => t.id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (updateDto.cryptoCode != null)
            {
                transaction.cryptoCode = updateDto.cryptoCode.ToLower();
            }
            if (updateDto.action != null)
            { 
                transaction.action = updateDto.action.ToLower();
            }
            if (updateDto.cryptoAmount.HasValue)
            {
                if (updateDto.cryptoAmount != 0)
                {
                   return BadRequest("La cantidada de criptomonedas debe ser mayor a 0 ");
                }
                transaction.cryptoAmount = updateDto.cryptoAmount.Value;
            }
            if (updateDto.money.HasValue)
            {
                if (updateDto.money <= 0)
                {
                    return BadRequest("El monto debe ser mayor a 0");
                }
                transaction.money = updateDto.money.Value;
            }

            if (updateDto.datetime != null)
            {
                if (!DateTime.TryParseExact(updateDto.datetime, "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return BadRequest("Formato de fecha invalido");
                }
                transaction.datetime = parsedDate;
            }

            if (updateDto.clientId.HasValue)
            {
                var clientExists = await _context.Clients.AnyAsync(c => c.id == updateDto.clientId.Value);
                if (!clientExists)
                {
                    return NotFound($"No se encontró el cliente con ID {updateDto.clientId.Value}");
                }
                transaction.clientId = updateDto.clientId.Value;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

          
            var responseDto = new TransactionResponseDto
            {
                id = transaction.id,
                cryptoCode = transaction.cryptoCode,
                action = transaction.action,
                cryptoAmount = transaction.cryptoAmount,
                money = transaction.money,
                datetime = transaction.datetime,
                clientId = transaction.clientId,
                clientName = transaction.Client?.name
            };

            return Ok(responseDto);
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.id == id);
        }
        


        [HttpPost]
        public async Task<ActionResult<TransactionResponseDto>> Post(TransactionCreateDto transactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!DateTime.TryParseExact(transactionDto.datetime, "yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture, DateTimeStyles.None, out var parcedDate ))
            { 
                return BadRequest("Formato de fecha invalido");
            }

            if (transactionDto.cryptoAmount <= 0)
            {
                return BadRequest("La cantidad de Criptomonedas debe ser mayor a 0");
            }

            string url = $"https://criptoya.com/api/argenbtc/{transactionDto.cryptoCode.ToLower()}/ars";
            decimal ARS_Price;

            using (var httpClient = new HttpClient())
            { 
                httpClient.DefaultRequestHeaders.Accept.Add
                ( new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        CriptoYaResponse result = JsonConvert.DeserializeObject<CriptoYaResponse>(responseContent);
                        ARS_Price = result.totalAsk;
                    }
                    else
                    {
                        return BadRequest("No se pudo obtener el precio de CriptoYa.");
                    }
                }
                catch(Exception ex)
                {
                    return StatusCode(500, $"Error al procesar la solicitud: {ex.Message}");
                }
              
            }
            decimal ARS_Amount = transactionDto.cryptoAmount * ARS_Price;

            var transaction = new CryptoTransaction
            {
                    cryptoCode = transactionDto.cryptoCode.ToLower(),
                    action = transactionDto.action.ToLower(),
                    cryptoAmount = transactionDto.cryptoAmount,
                    money = ARS_Amount,
                    clientId = transactionDto.clientId,
                    datetime = parcedDate
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var client = await _context.Clients.FindAsync(transaction.clientId);

            var responseDto = new TransactionResponseDto
            {
                id = transaction.id,
                cryptoCode = transaction.cryptoCode,
                action = transaction.action,
                cryptoAmount = transaction.cryptoAmount,
                money = transaction.money,
                datetime = transaction.datetime,
                clientId = transaction.clientId,
                clientName = client?.name

            };

            return CreatedAtAction(nameof(Get), new { id = transaction.id}, responseDto);
        }
       
       
    }
}
