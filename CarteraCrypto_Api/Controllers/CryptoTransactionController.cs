using CarteraCrypto_Api.DTOs;
using CarteraCrypto_Api.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
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
               crypto_Code = t.crypto_Code,
               action = t.action,
               crypto_Amount = t.crypto_Amount,
               money = t.money,
               datetime = t.datetime.ToString(),
               clientId = t.client_Id,
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

            if (transactionDto.crypto_Amount <= 0)
            {
                return BadRequest("La cantidad de Criptomonedas debe ser mayor a 0");
            }

            var transaction = new CryptoTransaction
            {
                crypto_Code = transactionDto.crypto_Code,
                action = transactionDto.action,
                crypto_Amount = transactionDto.crypto_Amount,
                money = transactionDto.money,
                client_Id = transactionDto.clientId,
                datetime = parcedDate
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = transaction.id}, transaction);
        }
    }
}
