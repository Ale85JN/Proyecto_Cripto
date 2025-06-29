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
        public async Task<ActionResult<IEnumerable<TransactionDto>>> Get() 
        {
            var transactions = await _context.Transactions
                    .Include(t => t.Client)
                    .OrderByDescending(t => t.datetime)
                    .ToListAsync();

            var transactionDto = transactions.Select(t => new TransactionResponseDto
            { 
               id=t.id,
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
