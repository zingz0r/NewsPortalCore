using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NewsPortal.Data.DTO;
using NewsPortal.Data.Entity;
using NewsPortal.WebAPI.Controllers;
using NewsPortal.WebAPI.Test.FakeManagers;
using Xunit;

namespace NewsPortal.WebAPI.Test
{
    public class NewsPortalWebAPITest : IDisposable
    {
        private readonly NewsPortalContext _context;

        private Mock<FakeUserManager> _mockUserManager;
        private Mock<FakeSignInManager> _mockSignInManager;
        private User _testUser1;
        private User _testUser2;
        private List<Article> _articlesData;
        private IEnumerable<ArticleDTO> _articleDTOs;

        public NewsPortalWebAPITest()
        {
            var options = new DbContextOptionsBuilder<NewsPortalContext>()
                .UseInMemoryDatabase("NewsPortalWebAPITest")
                .Options;

            _context = new NewsPortalContext(options);
            _context.Database.EnsureCreated();

            InitUsers();
            InitArticles();
        }

        private void InitArticles()
        {



            _articlesData = new List<Article>
            {
                new Article { Title = "TestTitle1", Summary = "TestSummary1", Text  = "TestTExt1", Date = new DateTime(2015,11,21,23,42,22), IsFeatured = false, UserId = _testUser1.Id},
                new Article { Title = "TestTitle2", Summary = "TestSummary2", Text  = "TestTExt2", Date = new DateTime(2017,11,21,23,42,22), IsFeatured = false, UserId = _testUser2.Id},

            };
            _context.Article.AddRange(_articlesData);
            _context.SaveChanges();


            _articleDTOs = _articlesData.Select(ArticleDTO.ConvertArticleToDTO);
        }

        private void InitUsers()
        {
            if (_context == null)
                throw new ArgumentNullException("Context is null");

            _testUser1 = new User
            {
                UserName = "TestUserName_1",
                Email = "TestUserName_1@test.it"
            };

            _testUser2 = new User
            {
                UserName = "TestUserName_2",
                Email = "TestUserName_2@test.it"
            };

            var userList = new List<User>
            {
                _testUser1,
                _testUser2

            }.AsQueryable();

            _context.Users.AddRange(userList);
            _context.SaveChanges();

            // get the created ids from db
            _testUser1.Id = _context.Users.FirstOrDefault(x => x.UserName == _testUser1.UserName).Id;
            _testUser2.Id = _context.Users.FirstOrDefault(x => x.UserName == _testUser2.UserName).Id;

            // sing in users
            _mockUserManager = new Mock<FakeUserManager>();
            _mockUserManager.Setup(x => x.Users)
                .Returns(userList);

            _mockSignInManager = new Mock<FakeSignInManager>();

            _mockSignInManager.Setup(
                    x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async void ArticleTest()
        {
            var controller = new ArticlesController(_context, _mockUserManager.Object);


            var userSpecificArticleDTO = _articleDTOs.Where(x => x.UserId == _testUser1.Id);

            // make test user to login
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _testUser1.UserName)
                    }))
                }
            };

            var result = await controller.GetMyArticles();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ArticleDTO>>(objectResult.Value);
            Assert.Equal(userSpecificArticleDTO, model);
        }


        [Fact]
        public async void CreateArticle()
        {
            var newArticle = new ArticleDTO
            {
                Author = _testUser1,
                Date = DateTime.MaxValue,
                IsFeatured = false,
                Text = "Added Test Text",
                Summary = "Added Test Summary",
            };

            var controller = new ArticlesController(_context, _mockUserManager.Object);
            var result = await controller.PostArticle(newArticle);

            // Assert
            var objectResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult.Value);
            Assert.Equal(_articleDTOs.Count() + 1, _context.Article.Count());
            
            Assert.Equal(newArticle, model);
        }

        [Fact]
        public async void UpdateArticle()
        {
            var localArticle = _context.Article.First();
            var localArticleDTO = ArticleDTO.ConvertArticleToDTO(localArticle);

            var controller = new ArticlesController(_context, _mockUserManager.Object);
            // make test user to login
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _testUser1.UserName)
                    }))
                }
            };

            string modifiedText = "Valid";

            localArticleDTO.Text = modifiedText;
            localArticleDTO.Title = modifiedText;
            localArticleDTO.Summary = modifiedText;

            var result = await controller.PutArticle(localArticleDTO.Id, localArticleDTO);
            var objectResult = Assert.IsType<OkResult>(result);

            var modifiedArticle = await _context.FindAsync<Article>(localArticleDTO.Id);

            Assert.Equal(modifiedArticle.Text, modifiedText);
            Assert.Equal(modifiedArticle.Title, modifiedText);
            Assert.Equal(modifiedArticle.Summary, modifiedText);
        }

        [Fact]
        public async void GetArticleById()
        {
            var localArticle = _context.Article.First();

            var localArticleDTO = ArticleDTO.ConvertArticleToDTO(localArticle);

            var controller = new ArticlesController(_context, _mockUserManager.Object);
            // make test user to login
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _testUser1.UserName)
                    }))
                }
            };

            var result = await controller.GetArticle(localArticle.Id);

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult.Value);

            Assert.Equal(localArticleDTO, model);
        }

        [Fact]
        public async void DeleteArticle()
        {
            var deleteArticle = _context.Article.First();
            var deleteArticleDTO = ArticleDTO.ConvertArticleToDTO(deleteArticle);


            var controller = new ArticlesController(_context, _mockUserManager.Object);
            // make test user to login
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _testUser1.UserName)
                    }))
                }
            };

            var result = await controller.DeleteArticle(deleteArticle.Id);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ArticleDTO>(objectResult.Value);
            Assert.Equal(_articleDTOs.Count() - 1, _context.Article.Count());
            Assert.Equal(deleteArticleDTO, model);

        }

    }
}
