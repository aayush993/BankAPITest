using BankingService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BankingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankingController : ControllerBase
    {
        [HttpPost("users")]
        public IActionResult CreateUser(JObject payload)
        {
            var user = payload.ToObject<User>();

            if (user == null)
            {
                return BadRequest("Check your Input and retry");
            }

            var userCreated = BankingOperations.CreateUser(user);
            return Ok(user);
        }

        /*[HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = BankingOperations.GetUsers();
            return Ok(users);
        }*/

        [HttpDelete("users/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                BankingOperations.DeleteUser(userId);
                return Ok("User " +
                    "successfully deleted.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("users/accounts/")]
        public IActionResult CreateAccount(JObject payload)
        {
            try
            {
                var accountRequest = payload.ToObject<CreateAccountRequest>();
                AccountType accType;
                Enum.TryParse<AccountType>(accountRequest.AccountType, out accType);

                var accountCreated = BankingOperations.CreateAccount(accountRequest.UserId, accType);

                return Ok(accountCreated);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("accounts/{accountId}")]
        public IActionResult DeleteAccount(int accountId)
        {
            try
            {
                BankingOperations.DeleteAccount(accountId);
                return Ok("Account successfully deleted");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("accounts/{accountId}/deposit")]
        public IActionResult Deposit(int accountId, JObject payload)
        {
            try
            {
                var deposit = payload.ToObject<TransactionRequest>();
                var currentBalance = BankingOperations.Deposit(accountId, deposit.Amount);
                return Ok("Current Balance:"+ currentBalance);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("accounts/{accountId}/withdraw")]
        public IActionResult Withdraw(int accountId, JObject payload)
        {
            try
            {
                var withdraw = payload.ToObject<TransactionRequest>();
                var currentBalance = BankingOperations.Withdraw(accountId, withdraw.Amount);
                return Ok("Current Balance:" + currentBalance);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
