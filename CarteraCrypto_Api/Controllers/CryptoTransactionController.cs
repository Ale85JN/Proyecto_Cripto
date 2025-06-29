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

            var transactionDto = transactions.Select(t => new TransactionDto
            { 
               id=t.id,
               cryptoCode = t.cryptoCode,
               action = t.action,
               cryptoAmount = t.cryptoAmount,
               money = t.money,
               datetime = t.datetime.ToString(),
               clientId = t.clientId,
               clientName = t.Client?.name

            }).ToList();
            return Ok(transactionDto);
        }
        [HttpPost]
        public async Task<ActionResult<CryptoTransaction>> Post(TransactionDto transactionDto)
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
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                catch(Exception ex)
                {
                    return StatusCode(500, $"Error al procesar la solicitud: {ex.Message}");
                }
            }
           
            var transaction = new CryptoTransaction
            {
                    cryptoCode = transactionDto.cryptoCode,
                    action = transactionDto.action,
                    cryptoAmount = transactionDto.cryptoAmount,
                    money = transactionDto.money,
                    clientId = transactionDto.clientId,
                    datetime = parcedDate
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = transaction.id}, transaction);
        }
    }
}
