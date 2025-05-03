using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WepAPiR_system.Controllers;
using WepAPiR_system.Models;
using WepAPiR_system.Services;

namespace HackerNews.Tests
{
    public class StoriesControllerTests
    {
        private readonly Mock<IHackerNewsService> _serviceMock;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _serviceMock = new Mock<IHackerNewsService>();
            _controller = new StoriesController(_serviceMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithListOfStories()
        {
            // Arrange
            var expectedStories = new List<Story>
            {
                new Story { Id = 1, Title = "Test Story 1", Url = "http://example.com/1" },
                new Story { Id = 2, Title = "Test Story 2", Url = "http://example.com/2" }
            };

            _serviceMock.Setup(s => s.GetNewestStoriesAsync(1, 5, null))
                        .ReturnsAsync(expectedStories);

            // Act
            var result = await _controller.Get(1, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Story>>(okResult.Value);
            Assert.Equal(2, ((List<Story>)returnValue).Count);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_EvenIfEmpty()
        {
            // Arrange
            _serviceMock.Setup(s => s.GetNewestStoriesAsync(1, 5, null))
                        .ReturnsAsync(new List<Story>());

            // Act
            var result = await _controller.Get(1, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Story>>(okResult.Value);
            Assert.Empty(returnValue);
        }
    }
}
