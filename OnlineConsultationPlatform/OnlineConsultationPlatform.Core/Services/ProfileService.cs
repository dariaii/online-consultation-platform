using Microsoft.AspNetCore.Http;
using OnlineConsultationPlatform.Data.Entities;
using OnlineConsultationPlatform.Data.Repository;

namespace OnlineConsultationPlatform.Core.Services
{
    public interface IProfileService
    {
        ApplicationUser? GetProfile();

        void EditProfile(
            string? firstName,
            string? lastName,
            string? phoneNumber);

        bool UploadProfilePicture(IFormFile file);
    }

    public class ProfileService(
        IRepository repository,
        IAuthenticationService authenticationService,
        IFileService fileService) : IProfileService
    {
        readonly IRepository _repository = repository;
        readonly IFileService _fileService = fileService;

        readonly ApplicationUser _currentUser = authenticationService.CurrentUser();

        public ApplicationUser? GetProfile()
        {
            var profile = _repository.SetNoTracking<ApplicationUser>().FirstOrDefault(x => x.Id == _currentUser.Id);

            return profile;
        }

        public void EditProfile(
            string? firstName,
            string? lastName,
            string? phoneNumber)
        {
            var profile = _repository.Set<ApplicationUser>().FirstOrDefault(x => x.Id == _currentUser.Id);

            if (profile != null && profile != null)
            {
                profile.FirstName = firstName;
                profile.LastName = lastName;
                profile.PhoneNumber = phoneNumber;

                _repository.Update(profile);
            }
        }

        public bool UploadProfilePicture(IFormFile file)
        {
            var user = _repository.Set<ApplicationUser>().FirstOrDefault(record => record.Id == _currentUser.Id);

            user.ProfilePicturePath = _fileService.UploadProfilePicture(file).Result;

            _repository.Update(user);

            return true;
        }
    }
}