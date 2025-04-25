using Microsoft.AspNetCore.Mvc;
using Moq;
using WepAPiR_system.Controllers;
using WepAPiR_system.Models; // Adjust namespace if Story is elsewhere
using WepAPiR_system.Services; // Adjust namespace for IHackerNewsService
using Xunit;

namespace WepAPiR_system.Tests
{
    public class StoriesControllerTests
    {
        private readonly Mock<IHackerNewsService> _mockService;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _mockService = new Mock<IHackerNewsService>();
            _controller = new StoriesController(_mockService.Object);
        }

        [Fact]
        public async Task Get_ValidPageAndPageSize_ReturnsOkWithStories()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string query = null;
            var stories = new List<Story>
            {
                new Story { Id = 1, Title = "Test Story", Score = "100", Time = "1697050500", Url = "https://example.com" }
            };

            // Assume BAL_HackerNews uses IHackerNewsService to get stories
            _mockService.Setup(s => s.GetNewestStoriesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                        .ReturnsAsync(stories);

            // Act
            var result = await _controller.Get(page, pageSize, query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStories = Assert.IsAssignableFrom<List<Story>>(okResult.Value);
            Assert.Single(returnedStories);
            Assert.Equal(stories[0].Id, returnedStories[0].Id);
        }

        [Fact]
        public async Task Get_WithQuery_ReturnsOkWithFilteredStories()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string query = "test";
            var stories = new List<Story>
            {
                new Story { Id = 1, Title = "Test Story", Score = "100", Time = "1697050500", Url = "https://example.com" }
            };

            _mockService.Setup(s => s.GetNewestStoriesAsync(page, pageSize, query))
                        .ReturnsAsync(stories);

            // Act
            var result = await _controller.Get(page, pageSize, query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStories = Assert.IsAssignableFrom<List<Story>>(okResult.Value);
            Assert.Single(returnedStories);
            Assert.Equal(stories[0].Title, returnedStories[0].Title);
        }

        [Fact]
        public async Task Get_NegativePage_ReturnsBadRequest()
        {
            // Arrange
            int page = -1;
            int pageSize = 10;

            // Act
            var result = await _controller.Get(page, pageSize);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Get_ZeroPageSize_ReturnsBadRequest()
        {
            // Arrange
            int page = 1;
            int pageSize = 0;

            // Act
            var result = await _controller.Get(page, pageSize);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Get_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            _mockService.Setup(s => s.GetNewestStoriesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                        .ThrowsAsync(new System.Exception("Service error"));

            // Act
            var result = await _controller.Get(page, pageSize);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}