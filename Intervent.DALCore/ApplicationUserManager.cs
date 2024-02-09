using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Intervent.DAL
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        /*private int PASSWORD_HISTORY_LIMIT = Convert.ToInt16(ConfigurationManager.AppSettings["PreviousPasswordLimit"].ToString());
        InterventDatabase dbcontext;

        public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public override async Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            if (await IsPreviousPassword(userId, newPassword))
            {
                return await Task.FromResult(IdentityResult.Failed("Cannot reuse old password"));
            }
            var result = await base.ChangePasswordAsync(userId, currentPassword, newPassword);
            if (result.Succeeded)
            {
                var store = Store as UserStore;
                await store.AddToPreviousPasswordsAsync(await FindByIdAsync(userId), PasswordHasher.HashPassword(newPassword));
            }
            return result;
        }
        public async Task CreatePasswordsAsync(int userId, string newPassword)
        {
            var store = Store as UserStore;
            await store.AddToPreviousPasswordsAsync(await FindByIdAsync(userId), PasswordHasher.HashPassword(newPassword));
        }

        public override async Task<IdentityResult> ResetPasswordAsync(int userId, string token, string newPassword)
        {
            if (await IsPreviousPassword(userId, newPassword))
            {
                return await Task.FromResult(IdentityResult.Failed("Cannot reuse old password"));
            }
            var result = await base.ResetPasswordAsync(userId, token, newPassword);
            if (result.Succeeded)
            {
                var store = Store as UserStore;
                await store.AddToPreviousPasswordsAsync(await FindByIdAsync(userId), PasswordHasher.HashPassword(newPassword));
            }
            return result;
        }

        private async Task<bool> IsPreviousPassword(int userId, string newPassword)
        {
            var user = await FindByIdAsync(userId);
            if (user.PreviousUserPasswords.OrderByDescending(x => x.CreateDate).
            Select(x => x.PasswordHash).Take(PASSWORD_HISTORY_LIMIT).Where(x => PasswordHasher.VerifyHashedPassword(x, newPassword) != PasswordVerificationResult.Failed
            ).Any())
            {
                return true;
            }
            return false;
        }*/
    }
}
