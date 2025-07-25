﻿using CarteraCrypto_Api.DTOs;
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
                return NotFound($"The client with the Id {clientId} was not found ");
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
                if (updateDto.cryptoAmount <= 0)
                {
                   return BadRequest("The amount of cryptocurrency must be greater than 0");
                }
                transaction.cryptoAmount = updateDto.cryptoAmount.Value;
            }

            if (updateDto.money.HasValue)
            {
                if (updateDto.money <= 0)
                {
                    return BadRequest("The amount must be greater than 0");
                }
                transaction.money = updateDto.money.Value;
            }

            if (updateDto.datetime != null)
            {
                if (!DateTime.TryParseExact(updateDto.datetime, "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return BadRequest("Invalid date format");
                }
                transaction.datetime = parsedDate;
            }

            if (updateDto.clientId.HasValue)
            {
                var clientExists = await _context.Clients.AnyAsync(c => c.id == updateDto.clientId.Value);
                if (!clientExists)
                {
                    return NotFound($" The client with the Id { updateDto.clientId.Value} was not found");
                }
                transaction.clientId = updateDto.clientId.Value;
            }
           

            await _context.SaveChangesAsync();
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


        [HttpPost]
        public async Task<ActionResult<TransactionResponseDto>> Post(TransactionCreateDto transactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!DateTime.TryParseExact(transactionDto.datetime, "yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture, DateTimeStyles.None, out var parcedDate ))
            { 
                return BadRequest("invalid date format");
            }

            if (transactionDto.cryptoAmount <= 0)
            {
                return BadRequest("The amount of cryptocurrency must be greater than 0");
            }
            if (transactionDto.action.ToLower() == "sale")
            {
                var previousTransactions = await _context.Transactions
                    .Where(t => t.clientId == transactionDto.clientId && t.cryptoCode == transactionDto.cryptoCode.ToLower())
                    .ToListAsync();

                var totalPurchased = previousTransactions.Where(t => t.action == "purchase")
                    .Sum(t => t.cryptoAmount);

                var totalSold = previousTransactions.Where(t => t.action == "sale")
                                    .Sum(t => t.cryptoAmount);

                var available = totalPurchased - totalSold;

                if (transactionDto.cryptoAmount > available)
                {
                    return BadRequest($"Can't sell {transactionDto.cryptoAmount} {transactionDto.cryptoCode.ToUpper()}. Available:{available}");
                }

            }

            string url = $"https://criptoya.com/api/argenbtc/{transactionDto.cryptoCode.ToLower()}/ars";
            decimal arsPrice;

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
                        arsPrice = result.totalAsk;
                    }
                    else
                    {
                        return BadRequest("Could not get price from CriptoYa.");
                    }
                }
                catch(Exception ex)
                {
                    return StatusCode(500, $"Error processing request: {ex.Message}");
                }
              
            }
            decimal arsAmount = transactionDto.cryptoAmount * arsPrice;

            var transaction = new CryptoTransaction
            {
                    cryptoCode = transactionDto.cryptoCode.ToLower(),
                    action = transactionDto.action.ToLower(),
                    cryptoAmount = transactionDto.cryptoAmount,
                    money = arsAmount,
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
