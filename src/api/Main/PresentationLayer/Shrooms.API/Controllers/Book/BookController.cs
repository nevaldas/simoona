﻿using System;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Shrooms.API.Filters;
using Shrooms.API.Helpers;
using Shrooms.Constants.Authorization.Permissions;
using Shrooms.Constants.BusinessLayer;
using Shrooms.DataTransferObjects.Models;
using Shrooms.DataTransferObjects.Models.Books;
using Shrooms.DataTransferObjects.Models.Books.BookDetails;
using Shrooms.DataTransferObjects.Models.Books.BooksByOffice;
using Shrooms.DataTransferObjects.Models.LazyPaged;
using Shrooms.Domain.Services.Books;
using Shrooms.DomainExceptions.Exceptions.Book;
using Shrooms.Infrastructure.FireAndForget;
using Shrooms.WebViewModels.Models.Book.BookDetails;
using Shrooms.WebViewModels.Models.Book.BooksByOffice;

namespace Shrooms.API.Controllers.Book
{
    [Authorize]
    [RoutePrefix("Book")]
    public class BookController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;
        private readonly IAsyncRunner _asyncRunner;

        public BookController(IMapper mapper, IBookService bookService, IAsyncRunner asyncRunner)
        {
            _mapper = mapper;
            _bookService = bookService;
            _asyncRunner = asyncRunner;
        }

        [HttpPost]
        [Route("Create")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Book)]
        public IHttpActionResult AddBook(NewBookViewModel book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookDto = _mapper.Map<NewBookViewModel, NewBookDTO>(book);
            SetOrganizationAndUser(bookDto);
            try
            {
                _bookService.AddBook(bookDto);
                return Ok();
            }
            catch (BookException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Book)]
        public IHttpActionResult DeleteBook(int bookId)
        {
            if (bookId < 1)
            {
                return BadRequest();
            }

            var userOrg = GetUserAndOrganization();
            try
            {
                _bookService.DeleteBook(bookId, userOrg);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("Edit")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Book)]
        public IHttpActionResult EditBook(EditBookViewModel book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookDto = _mapper.Map<EditBookViewModel, EditBookDTO>(book);
            SetOrganizationAndUser(bookDto);
            try
            {
                _bookService.EditBook(bookDto);
                return Ok();
            }
            catch (BookException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("ListByOffice")]
        [PermissionAuthorize(Permission = BasicPermissions.Book)]
        public IHttpActionResult GetBooksByOffice(int officeId, int page = 1, string searchString = null)
        {
            if (!string.IsNullOrEmpty(searchString) &&
                searchString.Length < ConstBusinessLayer.MinCharactersInBookSearch ||
                officeId < 1)
            {
                return BadRequest();
            }

            var options = new BooksByOfficeOptionsDTO
            {
                OfficeId = officeId,
                Page = page,
                SearchString = searchString
            };
            SetOrganizationAndUser(options);

            var bookDtos = _bookService.GetBooksByOffice(options);
            var result = _mapper.Map<ILazyPaged<BooksByOfficeDTO>, ILazyPaged<BooksByOfficeViewModel>>(bookDtos);
            return Ok(result);
        }

        [HttpGet]
        [Route("Details")]
        [PermissionAuthorize(Permission = BasicPermissions.Book)]
        public IHttpActionResult GetBookDetails(int bookOfficeId)
        {
            if (bookOfficeId < 1)
            {
                return BadRequest();
            }

            var bookWithLogsDto = _bookService.GetBookDetails(bookOfficeId, GetUserAndOrganization());
            var result = _mapper.Map<BookDetailsDTO, BookDetailsViewModel>(bookWithLogsDto);
            return Ok(result);
        }

        [HttpGet]
        [Route("DetailsForAdmin")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Book)]
        public IHttpActionResult GetBookDetailsForAdministrator(int bookOfficeId)
        {
            if (bookOfficeId < 1)
            {
                return BadRequest();
            }

            var bookWithLogsDto = _bookService.GetBookDetailsWithOffices(bookOfficeId, GetUserAndOrganization());
            var result = _mapper.Map<BookDetailsAdministrationDTO, BookDetailsAdministrationViewModel>(bookWithLogsDto);
            return Ok(result);
        }

        [HttpPut]
        [Route("Return")]
        [PermissionAuthorize(Permission = BasicPermissions.Book)]
        public IHttpActionResult ReturnBook(int bookOfficeId)
        {
            if (bookOfficeId < 1)
            {
                return BadRequest();
            }

            var userAndOrg = GetUserAndOrganization();
            try
            {
                _bookService.ReturnBook(bookOfficeId, userAndOrg);
                return Ok();
            }
            catch (BookException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route(template: "ReturnForAdmin")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Book)]
        public IHttpActionResult ReturnBookForAdmin(int bookOfficeId, string userId)
        {
            if (bookOfficeId < 1)
            {
                return BadRequest();
            }

            try
            {
                _bookService.ReturnBook(
                    bookOfficeId: bookOfficeId,
                    userAndOrg: new UserAndOrganizationDTO
                    {
                        OrganizationId = User.Identity.GetOrganizationId(),
                        UserId = userId
                    });
                return Ok();
            }
            catch (BookException e)
            {
                return BadRequest(message: e.Message);
            }
        }

        [HttpPut]
        [Route("Take")]
        [PermissionAuthorize(Permission = BasicPermissions.Book)]
        public IHttpActionResult TakeBook(int bookOfficeId)
        {
            if (bookOfficeId < 1)
            {
                return BadRequest();
            }

            var userAndOrg = GetUserAndOrganization();
            try
            {
                 _bookService.TakeBook(bookOfficeId, userAndOrg);
                return Ok();
            }
            catch (BookException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("FindByIsbn")]
        [PermissionAuthorize(Permission = AdministrationPermissions.Book)]
        public async Task<IHttpActionResult> FindByIsbn(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
            {
                return BadRequest();
            }

            try
            {
                var bookInfoDto = await _bookService.FindBookByIsbn(isbn, GetUserAndOrganization().OrganizationId);
                var result = _mapper.Map<RetrievedBookInfoDTO, RetrievedBookInfoViewModel>(bookInfoDto);
                return Ok(result);
            }
            catch (BookException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
