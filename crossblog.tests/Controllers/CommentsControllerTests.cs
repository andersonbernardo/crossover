using crossblog.Controllers;
using crossblog.Domain;
using crossblog.Model;
using crossblog.Repositories;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
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

        private Mock<ICommentRepository> _commentRepositoryMock = new Mock<ICommentRepository>();
        private Mock<IArticleRepository> _articleRepositoryMock = new Mock<IArticleRepository>();

        public CommentsControllerTests()
        {
            _commentsController = new CommentsController(_articleRepositoryMock.Object, _commentRepositoryMock.Object);
        }
       
       
        [Fact]
        public async Task Get_NotFound()
        {
            // Arrange
            _commentRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Comment>(null));

            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Get_ReturnsItens()
        {

            // Arrange
            var articleDbSetMock = Builder<Article>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _articleRepositoryMock.Setup(m => m.Query()).Returns(articleDbSetMock.Object);

            // Arrange
            _commentRepositoryMock.Setup(m => m.GetAsync(1)).Returns(Task.FromResult<Comment>(Builder<Comment>.CreateNew().Build()));
            
            // Act
            var result = await _commentsController.Get(1);

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;

            Assert.NotNull(objectResult);

            var content = objectResult.Value as CommentModel;

            Assert.NotNull(content);

            Assert.Equal("Title1", content.Title);
        }

        [Fact]
        public async Task Put_UpdateITem()
        {

            // Arrange
            var comment = Builder<Comment>.CreateNew().Build();

            _commentRepositoryMock.Setup(x => x.GetAsync(1)).Returns(Task.FromResult<Comment>(comment));

            var commentModel = new CommentModel { Title = "TitleUpdated" };

            // Act
            var result = await _commentsController.Put(comment.Id, commentModel);

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
            var articleModel = new CommentModel { Title = new string('a', 256), Email = "teste@teste.com.br" };

            var context = new ValidationContext(articleModel, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(articleModel, context, result, true);

            // Assert
            Assert.False(valid);

        }

        [Fact]
        public void Email_must_be_under_255()
        {
            // Arrange            
            var articleModel = new CommentModel { Email = $"{new string('a', 256)}@teste.com.br" };

            var context = new ValidationContext(articleModel, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(articleModel, context, result, true);

            // Assert
            Assert.False(valid);

        }

        [Fact]
        public void Email_must_be_valid()
        {
            // Arrange            
            var articleModel = new CommentModel { Email = "teste" };

            var context = new ValidationContext(articleModel, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(articleModel, context, result, true);

            // Assert
            Assert.False(valid);

        }
    }
}
