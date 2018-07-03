using crossblog.Controllers;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace crossblog.tests.Controllers
{
    public class CommentsControllerTests
    {
        private CommentsController _commentsController;
        private SqliteConnection _connection;
        private DbContextOptions<CrossBlogDbContext> options;
        private CrossBlogDbContext _context;

        public CommentsControllerTests()
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

            _commentsController = new CommentsController(new ArticleRepository(_context), new CommentRepository(_context));

        }

       
        [Fact]
        public async Task Get_NotFound()
        {
            //Arrange
            _context.Articles = null;

            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        //[Fact]
        //public async Task Get_ReturnsItem()
        //{
        //    // Arrange            
        //    var article = Builder<Article>.CreateNew().Build();
        //    var comment = Builder<Comment>.CreateNew().Build();

        //    article.Comments = new List<Comment>();
        //    article.Comments.Add(comment);

        //    _context.Articles.Add(article);

        //    // Act
        //    var result = await _commentsController.Get(1);

        //    // Assert
        //    Assert.NotNull(result);

        //    var objectResult = result as OkObjectResult;
        //    Assert.NotNull(objectResult);

        //    var content = objectResult.Value as ArticleModel;
        //    Assert.NotNull(content);

        //    Assert.Equal("Title1", content.Title);
        //}

        //[Fact]
        //public async Task Put_UpdateITem()
        //{
        //    // Arrange       

        //    var comment = Builder<Comment>.CreateNew().Build();
        //    _context.Comments.Add(comment);            

        //    var commentUpdated = new CommentModel { Title = "TitleUpdated" };
            
        //    // Act
        //    var result = await _commentsController.Put(comment.Id, commentUpdated);

        //    // Assert
        //    Assert.NotNull(result);

        //    var objectResult = result as OkObjectResult;

        //    Assert.NotNull(objectResult);

        //    var content = objectResult.Value as Comment;

        //    Assert.NotNull(content);

        //    Assert.Equal("TitleUpdated", content.Title);

        //}

        [Fact]
        public async Task Post_InsertITem()
        {
            // Arrange            
            var article = Builder<Article>.CreateNew().Build();
            _context.Articles.Add(article);

            var comment = Builder<CommentModel>.CreateNew().Build();

            // Act
            var result = await _commentsController.Post(article.Id, comment);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as CreatedResult;

            Assert.NotNull(objectResult);

            var content = objectResult.Value as CommentModel;

            Assert.NotNull(content);

            Assert.Equal(1, await _context.Comments.CountAsync());

        }

        [Fact]
        public void Title_must_be_under_255()
        {
            // Arrange            
            var comment = new CommentModel { Title = new string('a', 256), Email = "test@test.com", Content = "Content" };

            var context = new ValidationContext(comment, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(comment, context, result, true);

            // Assert
            Assert.Equal("The field Title must be a string with a maximum length of 255.", result[0].ErrorMessage);
            Assert.False(valid);

        }

        [Fact]
        public void Email_must_be_under_255()
        {
            // Arrange            
            var commentModel = new CommentModel { Email = new string('a', 256), Title = "Title", Content = "Content" };

            var context = new ValidationContext(commentModel, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(commentModel, context, result, true);
            Assert.Equal("The field Email must be a string with a maximum length of 255.", result[0].ErrorMessage);
            Assert.Equal("The Email field is not a valid e-mail address.", result[1].ErrorMessage); 
            // Assert
            Assert.False(valid);

        }

        [Fact]
        public void Email_must_be_valid()
        {
            // Arrange            
            var commentModel = new CommentModel { Email = "teste@", Title = "Title", Content = "Content" };

            var context = new ValidationContext(commentModel, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(commentModel, context, result, true);

            // Assert
            Assert.Equal("The Email field is not a valid e-mail address.", result[0].ErrorMessage);

            Assert.False(valid);
        }

        public void Dispose()
        {
            _connection.Close();
            _context.Dispose();
        }
    }
}
