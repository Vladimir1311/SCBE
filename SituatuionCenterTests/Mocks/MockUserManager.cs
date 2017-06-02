using Microsoft.AspNetCore.Identity;
using SituationCenterBackServer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace SituatuionCenterTests.Mocks
{
    public class MockUserManager : UserManager<ApplicationUser>
    {
        
        public MockUserManager(IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
        /*public MockUserManager() : this(
            new UserStore<ApplicationUser>(),
            GetOptions(),
            new PasswordHasher<ApplicationUser>,{}
        private static IOptions<IdentityOptions> GetOptions()
        {
            var op = new Mock<IOptions<IdentityOptions>>();
            op.Setup(O => O.Value).Returns(new IdentityOptions());
            return op.Object;
        }*/
    }

    public class UserStore<T> : IUserStore<T> where T : class
    {
        public Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    class PasswordHasher<T> : IPasswordHasher<T> where T : class
    {
        public string HashPassword(T user, string password)
        {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(T user, string hashedPassword, string providedPassword)
        {
            return PasswordVerificationResult.Success;
        }
    }
}
