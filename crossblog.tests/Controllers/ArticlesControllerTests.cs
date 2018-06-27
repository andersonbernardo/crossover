using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using crossblog.Controllers;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace crossblog.tests.Controllers
{
    public class ArticlesControllerTests : IDisposable
    {
        private ArticlesController _articlesController;
        private SqliteConnection _connection;
        private DbContextOptions<CrossBlogDbContext> options;
        private CrossBlogDbContext _context;

        public ArticlesControllerTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            options = new DbContextOptionsBuilder<CrossBlogDbContext>()
                 .UseSqlite(_connection)
                 .Options;

            using (var context = new CrossBlogDbContext(options))
            {
                context.Database.EnsureCreated();
            }

            _context = new CrossBlogDbContext(options);
                            
            _articlesController = new ArticlesController(new ArticleRepository(_context));            

        }        


        [Fact]
        public async Task Get_NotFound()
        {
            //Arrange
            _context.Articles = null;
            
            // Act
            var result = await _articlesController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Get_ReturnsItem()
        {
            // Arrange            
            _context.Articles.AddRange(Builder<Article>.CreateListOfSize(3).Build());
            _context.SaveChanges();

            // Act
            var result = await _articlesController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as ArticleModel;
            Assert.NotNull(content);

            Assert.Equal("Title1", content.Title);
        }      

        [Fact]
        public async Task Put_UpdateITem() {
                                    
            // Arrange            
            _context.Articles.Add(Builder<Article>.CreateNew().Build());
            _context.SaveChanges();            

            var articleModel = new ArticleModel { Title = "TitleUpdated" };
            
            // Act
            var result = await _articlesController.Put(_context.Articles.FirstOrDefault().Id, articleModel);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;

            Assert.NotNull(objectResult);

            var content = objectResult.Value as Article;

            Assert.NotNull(content);

            Assert.Equal("TitleUpdated", content.Title);

        }

        [Fact]
        public async Task Post_InsertiTem()
        {
            // Arrange            
            var article = Builder<ArticleModel>.CreateNew().Build();            

            // Act
            var result = await _articlesController.Post(article);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as CreatedResult;

            Assert.NotNull(objectResult);

            var content = objectResult.Value as Article;

            Assert.NotNull(content);

            Assert.Equal(1, _context.Articles.Count());

        }

        [Fact]
        public void Title_must_be_under_255()
        {
            // Arrange            
            var articleModel = new ArticleModel { Title = new string('a', 256) };
            
            var context = new ValidationContext(articleModel, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(articleModel, context, result, true);            

            // Assert
            Assert.False(valid);          

        }

        public void Dispose()
        {
            _connection.Close();
            _context.Dispose();
        }
    }
}
