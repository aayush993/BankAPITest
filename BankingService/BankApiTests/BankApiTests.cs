using BankApiTests.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using BankApiTests.Utils;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BankApiTests
{
    [TestFixture]
    public class BankApiTests
    {
        private RestClient _client;
        private int userId;
        private int secondUserId;
        private int accountId;
        private int corporateAccountId;
        private int chequingAccountId;
        private int depositAmount;
        private int withdrawAmount;
        private int expectedBalance;

        [SetUp]
        public void Setup()
        {
            _client = new RestClient("http://localhost:8083/api/Banking");
            depositAmount = 700;
            withdrawAmount = 100;
            expectedBalance = depositAmount - withdrawAmount;
        }

        [Test, Order(1)]
        [Category("HappyPath")]
        public void TestCreateUser()
        {
            // Initialize input variables
            var firstName = "Bil";
            var lastName = "Murrey";
            var email = "bil@example.com";
            var phone = "6125909057";

            //Create Test Input request
            var createUserRequest = Common.CreateUserRequest(firstName, lastName, email, phone);

            //Calling the API to get the Response.
            var response = _client.Execute(createUserRequest);
           
            //Verify Status Code
            Assert.AreEqual(200, (int)response.StatusCode);

            //Validate respone
            Assert.IsTrue(response.Content.Contains(firstName));
            Assert.IsTrue(response.Content.Contains(lastName));
            Assert.IsTrue(response.Content.Contains(email));
            Assert.IsTrue(response.Content.Contains(phone));

            //Deserialize and capture user ID for creating account.
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            userId = content.Id;

        }

        [Test, Order(2)]
        [Category("HappyPath")]
        public void TestCreateAccount()
        {
            var accountType = "Savings";

            //Step2: Add a account
            var createAccountRequest = Common.CreateAccountRequest(userId, accountType);
            var response = _client.Execute(createAccountRequest);

            //Validate response
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("balance"));

            //Deserialize and capture account ID.
            var accountResponse = JsonConvert.DeserializeObject<Account>(response.Content);
            accountId = accountResponse.Id;
        }

        [Test, Order(3)]
        [Category("HappyPath")]
        public void TestDeposit()
        {
            var amount = depositAmount.ToString();
           
            //Step3: Deposit money to the account
            var request = new RestRequest("accounts/"+  accountId + "/deposit", Method.Post);
            request.AddJsonBody(new { Amount = amount });
            var response = _client.Execute(request);
            
            //Validate Response
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("Current Balance:"+ amount));
        }

        [Test, Order(4)]
        [Category("HappyPath")]
        public void TestWithdraw()
        {
            var amount = withdrawAmount.ToString();
            
            //Step4: Withdraw money from the account
            var request = new RestRequest("accounts/" + accountId + "/withdraw", Method.Post);
            request.AddJsonBody(new { Amount = amount });
            var response = _client.Execute(request);

            //Validate Response
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("Current Balance:"+ expectedBalance.ToString()));
        }

        [Test, Order(4)]
        [Category("NegativeScenario")]
        public void TestErrorDepositMoreThanTenThousand()
        {
            int maxAmount = 50000;
            var amount = maxAmount.ToString();

            //Deposit money to the account
            var request = new RestRequest("accounts/" + accountId + "/deposit", Method.Post);
            request.AddJsonBody(new { Amount = amount });
            var response = _client.Execute(request);

            //Validate Failure
            Assert.AreEqual(400, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("Deposit amount cannot be greater than $10,000"));

        }

        [Test, Order(5)]
        [Category("NegativeScenario")]
        public void TestErrorWithdrawFullBalance()
        {

            var amount = expectedBalance.ToString();

            //Step5: Withdraw full balance
            var request = new RestRequest("accounts/" + accountId + "/withdraw", Method.Post);
            request.AddJsonBody(new { Amount = amount });
            var response = _client.Execute(request);

            //Validate Response
            Assert.AreEqual(409, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("Withdrawal amount cannot be greater than 90%"));
        }

        [Test, Order(6)]
        [Category("NegativeScenario")]
        public void TestErrorWithdrawTillHunderedBalance()
        {
            int withdrawableBalance = expectedBalance - 120;
            var amount = withdrawableBalance.ToString();

            //Step6: Withdraw till balance is around 100$
            var request = new RestRequest("accounts/" + accountId + "/withdraw", Method.Post);
            request.AddJsonBody(new { Amount = amount });
            var response = _client.Execute(request);

            //Validate Response
            Assert.AreEqual(200, (int)response.StatusCode);

            //Step7: Withdraw some money to make the balance lesser than 100$
            request = new RestRequest("accounts/" + accountId + "/withdraw", Method.Post);
            request.AddJsonBody(new { Amount = "30" });
            response = _client.Execute(request);

            //Validate Failure
            Assert.AreEqual(409, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("Account balance cannot be less than $100"));
        }

        [Test, Order(7)]
        [Category("HappyPath")]
        public void TestDeleteAccount()
        {
            //Delete Account
            var request = new RestRequest("accounts/" + accountId, Method.Delete);
            var response = _client.Execute(request);

            //Validate success
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("Account successfully deleted"));
        }

        [Test, Order(8)]
        [Category("HappyPath")]
        public void TestDeleteUser()
        {
            //Delete User Account created
            var request = new RestRequest("users/" + userId, Method.Delete);
            var response = _client.Execute(request);

            //Validate success
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("User successfully deleted"));
        }

        [Test, Order(1)]
        [Category("HappyPath")]
        public void TestCreateMultipleAccountsForUser()
        {

            //Add a new user
            var createUserRequest = Common.CreateUserRequest("John", "Doe", "john@example.com", "6122309057");
            var response = _client.Execute(createUserRequest);
            Assert.AreEqual(200, (int)response.StatusCode);
            var content = JsonConvert.DeserializeObject<User>(response.Content);
            secondUserId = content.Id;

            var corporateAccount = "Corporate";
            var chequingAccount = "Chequing";

            //Add account 1
            var createAccountRequest = Common.CreateAccountRequest(secondUserId, corporateAccount);
            response = _client.Execute(createAccountRequest);
            Assert.AreEqual(200, (int)response.StatusCode);
            var accountResponse = JsonConvert.DeserializeObject<Account>(response.Content);
            corporateAccountId = accountResponse.Id;
            
            //Add account 2
            createAccountRequest = Common.CreateAccountRequest(secondUserId, chequingAccount);
            response = _client.Execute(createAccountRequest);
            Assert.AreEqual(200, (int)response.StatusCode);
            accountResponse = JsonConvert.DeserializeObject<Account>(response.Content);
            chequingAccountId = accountResponse.Id;

            //Two Accounts successfully added for a User.
        }

        [Test, Order(2)]
        [Category("NegativeScenario")]
        public void TestErrorDeleteAccountWithZeroBalance()
        {
            //Delete Account
            var request = new RestRequest("accounts/" + chequingAccountId, Method.Delete);
            var response = _client.Execute(request);

            //Validate failure, as account balance is less than 100$
            Assert.AreEqual(409, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("Account balance cannot be less than $100"));

            //Deposit money to the account
            request = new RestRequest("accounts/" + chequingAccountId + "/deposit", Method.Post);
            request.AddJsonBody(new { Amount = depositAmount.ToString() });
            response = _client.Execute(request);
            Assert.AreEqual(200, (int)response.StatusCode);

            //Delete Account 
            request = new RestRequest("accounts/" + chequingAccountId, Method.Delete);
            response = _client.Execute(request);

            //Validate success
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("Account successfully deleted"));
        }

        [Test, Order(3)]
        [Category("HappyPath")]
        public void TestDepositMaxAmountInSingleTransaction()
        {
            var maxBalance = "10000";
            //Deposit max money to the account
            var request = new RestRequest("accounts/" + corporateAccountId + "/deposit", Method.Post);
            request.AddJsonBody(new { Amount = maxBalance });
            var response = _client.Execute(request);

            Assert.AreEqual(200, (int)response.StatusCode);
        }

        [Test, Order(4)]
        [Category("NegativeScenario")]
        public void TestErrorDeleteUserWithActiveAccount()

        {   
            //Delete User Account created
            var request = new RestRequest("users/" + secondUserId, Method.Delete);
            var response = _client.Execute(request);

            //Validate failure, as there are still accounts for user.
            Assert.AreEqual(409, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("User still has accounts"));

            //Delete Account 2
            request = new RestRequest("accounts/" + corporateAccountId, Method.Delete);
            response = _client.Execute(request);
            Assert.AreEqual(200, (int)response.StatusCode);

            //Delete User Account now
            request = new RestRequest("users/" + secondUserId, Method.Delete);
            response = _client.Execute(request);

            //Validate Success
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(response.Content.Contains("User successfully deleted"));
        }


    }
}
