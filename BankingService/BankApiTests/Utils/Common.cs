using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApiTests.Utils
{
    internal class Common
    {
        public static RestRequest CreateUserRequest(string firstName, string lastName, string email, string phone)
        {
            var req = new RestRequest("users", Method.Post);
            req.AddJsonBody(new { FirstName = firstName, LastName = lastName, Email = email, Phone = phone });
            return req;
        }

        public static RestRequest CreateAccountRequest(int userId, string accountType)
        {
            var request = new RestRequest("users/accounts", Method.Post);
            request.AddJsonBody(new { UserId = userId, AccountType = accountType });
            return request;
        }
    }
}
