﻿namespace Shrooms.API.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using AutoMapper;
    using Shrooms.DataTransferObjects.Models.Notification;
    using Shrooms.Domain.Services.Notifications;
    using Shrooms.WebViewModels.Models.Notifications;

    [Authorize]
    public class NotificationController : BaseController
    {
        private INotificationService _notificationService;
        private IMapper _mapper;

        public NotificationController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationViewModel>> GetAll()
        {
            var result = await _notificationService.GetAll(GetUserAndOrganization());

            return MakeCommentsStacked(result);
        }

        [HttpPut]
        public async Task<IHttpActionResult> MarkAsRead(IEnumerable<int> ids)
        {
            await _notificationService.MarkAsRead(GetUserAndOrganization(), ids);

            return Ok();
        }

        [HttpPut]
        public async Task<IHttpActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsRead(GetUserAndOrganization());

            return Ok();
        }

        private IEnumerable<NotificationViewModel> MakeCommentsStacked(IEnumerable<NotificationDto> comments)
        {
            List<NotificationViewModel> stackedList = new List<NotificationViewModel>();

            foreach (var item in comments)
            {
                var parentComment = stackedList.Where(x => CompareSourcesIds(x.sourceIds, item.SourceIds)).FirstOrDefault();
                if (parentComment == null)
                {
                    stackedList.Add(_mapper.Map<NotificationViewModel>(item));
                }
                else
                {
                    parentComment.stackedIds.Add(item.Id);
                    if (parentComment.title.Equals(item.Title) == false)
                    {
                        parentComment.others++;
                    }
                }
            }

            return stackedList.AsEnumerable();
        }

        private bool CompareSourcesIds(SourcesViewModel viewModel, SourcesDto dtoModel)
        {
            if (viewModel.postId != dtoModel.PostId || viewModel.eventId != dtoModel.EventId || viewModel.projectId != dtoModel.ProjectId || viewModel.wallId != dtoModel.WallId)
            {
                return false;
            }

            return true;
        }
    }
}