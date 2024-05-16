using OnlineConsultationPlatform.Core.DTOs;
using OnlineConsultationPlatform.Core.Infrastructure.Enums;
using OnlineConsultationPlatform.Data.Entities;
using OnlineConsultationPlatform.Data.Repository;

namespace OnlineConsultationPlatform.Core.Services
{
    public interface IUsersService
    {
        List<ApplicationUser> GetUsers(out int totalRecordsCount, bool showDeactivated, int? page = null, int? pageSize = null);

        void UpdateAccessStatus(string userId);

        UserStatisticsDTO GetUserStatistics(string userId);

        List<ApplicationUser> GetMentors(bool includeRelatedMentors = false);
    }

    public class UsersService(
        IRepository repository,
        IAuthenticationService authenticationService,
        IMeetingService meetingService) : IUsersService
    {
        readonly IRepository _repository = repository;
        readonly IAuthenticationService _authenticationService = authenticationService;
        readonly IMeetingService _meetingService = meetingService;

        public List<ApplicationUser> GetUsers(out int totalRecordsCount, bool showDeactivated, int? page = null, int? pageSize = null)
        {
            var users = _repository.SetNoTracking<ApplicationUser>().Where(user => user.IsActive || (showDeactivated && !user.IsActive));

            var administrators = _authenticationService.GetUsersInRole(Roles.Admin);
            if (administrators != null)
            {
                users = users.Where(user => !administrators.Any(admin => admin == user));
            }

            totalRecordsCount = users.Count();

            if (page.HasValue && pageSize.HasValue)
            {
                users = users.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return 
                users.ToList();
        }

        public void UpdateAccessStatus(string userId)
        {
            var user = _authenticationService.GetUserById(userId);

            if (user != null)
            {
                user.IsActive = !user.IsActive;

                _repository.Update(user);
            }
        }

        public UserStatisticsDTO GetUserStatistics(string userId)
        {
            var userMeetings = _repository
                    .SetNoTracking<Meeting>()
                    .Where(x => x.UserId == userId || x.MentorId == userId);

            var bookedMeetings = userMeetings.Count();

            var ratedUserMeetings = userMeetings.Where(x => x.Rating != null).ToList();

            return new UserStatisticsDTO
            {
                BookedMeetings = bookedMeetings,
                NumberOfRatings = ratedUserMeetings.Count,
                AverageRating = (int)Math.Round(ratedUserMeetings?.Average(x => x.Rating) ?? 0)
            };
        }

        public List<ApplicationUser> GetMentors(bool includeRelatedMentors = false)
        {
            var mentors = _authenticationService.GetUsersInRole(Roles.Mentor).AsEnumerable();

            if (includeRelatedMentors)
            {
                var meetings = _meetingService.GetMeetings(out _);

                mentors = mentors.Where(mentor => meetings.Any(meeting => meeting.MentorId == mentor.Id));
            }
            else
            {
                mentors = mentors.Where(x => x.IsActive);
            }

            return mentors.ToList();
        }
    }
}
