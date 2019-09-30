﻿using AutoMapper;
using Shrooms.API.Hubs;
using Shrooms.DataTransferObjects.Models;
using Shrooms.DataTransferObjects.Models.Wall.Posts;
using Shrooms.Domain.Services.Email.Posting;
using Shrooms.Domain.Services.Notifications;
using Shrooms.Domain.Services.UserService;
using Shrooms.Infrastructure.FireAndForget;
using Shrooms.WebViewModels.Models.Notifications;

namespace Shrooms.API.BackgroundWorkers
{
    public class NewPostNotifier : IBackgroundWorker
    {
        private readonly IPostNotificationService _postNotificationService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NewPostNotifier(IPostNotificationService postNotificationService, IUserService userService, INotificationService notificationService, IMapper mapper)
        {
            _postNotificationService = postNotificationService;
            _userService = userService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public void Notify(NewlyCreatedPostDTO createdPost, UserAndOrganizationHubDto userAndOrganizationHubDto)
        {
            _postNotificationService.NotifyAboutNewPost(createdPost);

            var membersToNotify = _userService.GetWallUserAppNotificationEnabledIds(createdPost.User.UserId, createdPost.WallId);

            var notificationDto = _notificationService.CreateForPost(userAndOrganizationHubDto, createdPost, createdPost.WallId, membersToNotify).GetAwaiter().GetResult();

            NotificationHub.SendNotificationToParticularUsers(_mapper.Map<NotificationViewModel>(notificationDto), userAndOrganizationHubDto, membersToNotify);
            NotificationHub.SendWallNotification(createdPost.WallId, membersToNotify, createdPost.WallType, userAndOrganizationHubDto);
        }
    }
}
