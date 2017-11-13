using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SituationCenter.Shared.Exceptions;
using SituationCenterCore.Controllers.API.V1;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Models.TokenAuthModels;
using SituationCenterCore.Pages.Account;
using System;
using System.Threading.Tasks;

namespace SituationCenterCore.Tests.ApiControllers
{
    [TestClass]
    public class AccountControllerTests
    {
        [TestMethod]
        public void Registration_RegisterNewUserToEmptyDataBase()
        {
            var (repoMock, controller, authOption) = GetDefalt();
            ApplicationUser createdUser = null;
            string UsedPassword = null;
            repoMock
                .Setup(R => R.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((U, P) => (createdUser, UsedPassword) = (U, P))
                .Returns(Task.FromResult(IdentityResult.Success));

            var registerModel = new RegisterModel.InputModel()
            {
                Email = "alalalala@gmail.com",
                Password = "LongPassword1_",
                ConfirmPassword = "LongPassword1_",
                Sex = true,
                Name  ="Тест",
                Surname = "Тестеров",
                PhoneNumber = "+78885553535",
                Birthday = "25/03/1995"
            };
            var result = controller.Registration(registerModel).Result;
            Assert.AreEqual(StatusCode.OK, result.StatusCode);
            repoMock.Verify(R => R.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(registerModel.Email, createdUser.Email);
            Assert.AreEqual(registerModel.Password, UsedPassword);
            Assert.AreNotEqual(registerModel.Password, createdUser.PasswordHash);
            Assert.AreEqual(registerModel.Sex, createdUser.Sex);
            Assert.AreEqual(registerModel.Name, createdUser.Name);
            Assert.AreEqual(registerModel.Surname, createdUser.Surname);
            Assert.AreEqual(registerModel.PhoneNumber, createdUser.PhoneNumber);
            Assert.AreEqual(registerModel.ParsedBirthday(), createdUser.Birthday);
        }


        private (Mock<IRepository>, AccountController, AuthOptions) GetDefalt()
        {
            var mock = new Mock<IRepository>();
            var authOptions = new AuthOptions
            {
                SecretKey = "Secure key",
                Audience= "DemoAudience",
                Issuer = "DemoIssuer",
                Expiration = TimeSpan.FromMinutes(10)
            };
            var options = new Option<AuthOptions>(authOptions);
            return (mock, new AccountController(new EmptyLogger<AccountController>(), options, mock.Object), authOptions);
        }
    }

    internal class Option<T> : IOptions<T> where T : class, new()
    {
        public T Value { get; }
        public Option(T Val) => Value = Val;
    }
}
