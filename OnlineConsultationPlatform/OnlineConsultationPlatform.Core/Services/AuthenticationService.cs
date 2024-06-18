using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OnlineConsultationPlatform.Core.Infrastructure.Enums;
using OnlineConsultationPlatform.Data.Entities;
using OnlineConsultationPlatform.Data.Repository;

namespace OnlineConsultationPlatform.Core.Services
{
    public interface IAuthenticationService
    {
        Task<Microsoft.AspNetCore.Identity.SignInResult> SignIn(string? email, string? password, bool rememberMe, bool lockoutOnFailure);

        Task SignOut();

        Task<IdentityResult> RegisterAsync(
            string? firstName,
            string? lastName,
            string? phoneNumber,
            string? email,
            string? password,
            bool isMentor,
            string? returnUrl);

        Task<IdentityResult> ConfirmEmailAsync(string token, string email, string returnUrl);

        bool SendResetPasswordEmail(string? email, string? returnUrl);

        Task<IdentityResult> ResetPassword(string email, string? token, string? newPassword);

        Task<IdentityResult> ConfirmPasswordAsync(string token, string email, string returnUrl);

        ApplicationUser CurrentUser();

        ApplicationUser? GetUserById(string userId);

        ApplicationUser FindUserByEmail(string? email);

        bool IsUserInRole(Roles role);

        List<string> GetAdminUsersForEmailNotifications();

        List<ApplicationUser> GetUsersInRole(Roles role);
    }

    public class AuthenticationService : IAuthenticationService
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly SignInManager<ApplicationUser> _signInManager;
        readonly IEmailService _emailService;
        readonly IUrlHelper _urlHelper;
        readonly IRepository _repository;
        readonly IStringLocalizer _localizer;
        readonly ApplicationUser? _currentUser;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelper urlHelper,
            IRepository repository,
            IStringLocalizer localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _urlHelper = urlHelper;
            _repository = repository;
            _localizer = localizer;

            if (httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                _currentUser = _userManager.FindByNameAsync(httpContextAccessor.HttpContext?.User?.Identity?.Name).Result;
            }
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> SignIn(string? email, string? password, bool rememberMe, bool lockoutOnFailure)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !user.EmailConfirmed)
            {
                return new Microsoft.AspNetCore.Identity.SignInResult();
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, lockoutOnFailure: true);

            return result;
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterAsync(
            string? firstName,
            string? lastName,
            string? phoneNumber,
            string? email,
            string? password,
            bool isMentor,
            string? returnUrl)
        {
            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, isMentor ? Roles.Mentor.ToString() : Roles.Student.ToString());

                var url =
                    _urlHelper.Action(
                        "ConfirmEmail",
                        "Account",
                        new
                        {
                            Token = _userManager.GenerateEmailConfirmationTokenAsync(user).Result,
                            user.Email,
                            ReturnUrl = returnUrl
                        },
                        _urlHelper.ActionContext.HttpContext.Request.Scheme);

                SendEmail(user, url, _localizer["Email_ConfirmEmailSubject"], _localizer["Email_ConfirmEmailBody"]);
            }

            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string token, string email, string returnUrl)
        {
            var user = _repository.Set<ApplicationUser>().FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    user.IsActive = true;

                    _repository.Update(user);
                }

                return result;
            }

            return null;
        }

        public bool SendResetPasswordEmail(string? email, string? returnUrl)
        {
            var user = FindUserByEmail(email);
            if (user != null)
            {
                var url =
                    _urlHelper.Action(
                        "ResetPassword",
                        "Account", new
                        {
                            Token = _userManager.GeneratePasswordResetTokenAsync(user).Result,
                            user.Email,
                            ReturnUrl = returnUrl
                        },
                        _urlHelper.ActionContext.HttpContext.Request.Scheme);

                SendEmail(user, url, _localizer["Email_ResetPasswordSubject"], _localizer["Email_ResetPasswordBody"]);

                return true;
            }

            return false;
        }

        public async Task<IdentityResult> ResetPassword(string email, string? token, string? newPassword)
        {
            var user
                = _repository.Set<ApplicationUser>().FirstOrDefault(x => x.Email == email);

            if (user != null)
            {
                return await _userManager.ResetPasswordAsync(user, token, newPassword);
            }

            return null;
        }

        public async Task<IdentityResult> ConfirmPasswordAsync(string token, string email, string returnUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return await _userManager.ConfirmEmailAsync(user, token);
            }

            return null;
        }

        public ApplicationUser CurrentUser()
        {
            return
                _currentUser;
        }

        public ApplicationUser? GetUserById(string userId)
        {
            return
                _repository.SetNoTracking<ApplicationUser>().FirstOrDefault(x => x.Id == userId);
        }

        public ApplicationUser FindUserByEmail(string? email)
        {
            return
                _repository.SetNoTracking<ApplicationUser>().FirstOrDefault(x => x.Email == email);
        }

        public bool IsUserInRole(Roles role)
        {
            return _userManager.IsInRoleAsync(_currentUser, role.ToString()).Result;
        }

        public List<string> GetAdminUsersForEmailNotifications()
        {
            return
                _userManager.GetUsersInRoleAsync(nameof(Roles.Admin)).Result
                        .Select(x => x.Email)
                        .ToList();
        }

        public List<ApplicationUser> GetUsersInRole(Roles role)
        {
            return [.. _userManager.GetUsersInRoleAsync(role.ToString()).Result];
        }

        void SendEmail(ApplicationUser user, string url, string subject, string template)
        {
            var recievers = new List<string> { user.Email };
            var body = HttpUtility.HtmlEncode(string.Format(template, url));

            _emailService.SendEmail(recievers, subject, HttpUtility.HtmlDecode(body));
        }
    }
}