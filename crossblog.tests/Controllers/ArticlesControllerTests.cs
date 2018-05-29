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
    public class ArticlesControllerTests
    {
        private ArticlesController _articlesController;

        private Mock<IArticleRepository> _articleRepositoryMock = new Mock<IArticleRepository>();

        public ArticlesControllerTests()
        {
            _articlesController = new ArticlesController(_articleRepositoryMock.Object);
        }

        [Fact]
        public async Task Search_ReturnsEmptyList()
        {
            // Arrange
            var articleDbSetMock = Builder<Article>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _articleRepositoryMock.Setup(m => m.Query()).Returns(articleDbSetMock.Object);

            // Act
            var result = await _articlesController.Search("Invalid");

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as ArticleListModel;
            Assert.NotNull(content);

            Assert.Empty(content.Articles);
        }

        //[Fact]
        //public async Task Search_ReturnsList()
        //{
        //    var connection = new SqliteConnection("DataSource=:memory:");
        //    connection.Open();          

        //    // Arrange
        //    var options = new DbContextOptionsBuilder<CrossBlogDbContext>()
        //            .UseSqlite(connection)
        //            .Options;


        //    using (var context = new CrossBlogDbContext(options))
        //    {
        //        context.Database.EnsureCreated();
        //    }

        //    using (var context = new CrossBlogDbContext(options))
        //    {
        //        context.Articles.Add(new Article { Title = "Title1", Content = "Content1" });
        //        context.Articles.Add(new Article { Title = "Title2", Content = "Content2" });
        //        context.Articles.Add(new Article { Title = "Title3", Content = "Content3" });
        //        context.SaveChanges();
                
        //    }

        //    using (var context = new CrossBlogDbContext(options))
        //    {

        //        var _ArticleRepository = new ArticleRepository(context);


        //        var articlesController = new ArticlesController(_ArticleRepository);

        //        // Act
        //        var result = await _articlesController.Search("Title");

        //        // Assert
        //        Assert.NotNull(result);

        //        var objectResult = result as OkObjectResult;
        //        Assert.NotNull(objectResult);

        //        var content = objectResult.Value as ArticleListModel;
        //        Assert.NotNull(content);

        //        Assert.Equal(3,content.Articles.Count());            
        //    }

        //    connection.Close();
        //}

        [Fact]
        public async Task Get_NotFound()
        {
            // Arrange
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(null));

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
            _articleRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Article>(Builder<Article>.CreateNew().Build()));
            
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
            var article = Builder<Article>.CreateNew().Build();

            _articleRepositoryMock.Setup(x => x.GetAsync(1)).Returns(Task.FromResult<Article>(article));

            var articleModel = new ArticleModel { Title = "TitleUpdated" };
            
            // Act
            var result = await _articlesController.Put(article.Id, articleModel);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;

            Assert.NotNull(objectResult);

            var content = objectResult.Value as Article;

            Assert.NotNull(content);

            Assert.Equal("TitleUpdated", content.Title);

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

    }
}
