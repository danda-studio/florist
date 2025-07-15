using FloristAI.Adapter;
using FloristAI.Adapter.Models;
using FloristAI.Router;
using Moq;
using Xunit;

namespace FloristAI.Tests
{
    public class AdapterRouterTests
    {
        [Fact]
        public async Task RouteAsync_CommandWithoutRedirect_ReturnsExceptedMessage()
        {
            var mockAdapter = new Mock<IMessageAdapter>();
                mockAdapter.Setup(a => a.RouteKey).Returns("start");
                mockAdapter.Setup(a => a.ProcessMessage("/start", 123))
                    .ReturnsAsync(new MessageResult { Text = "Hello from start" });

            var router = new AdapterRouter(new[] { mockAdapter.Object });

            var result = await router.RouteAsync("/start", 123);

            Assert.Equal("Hello from start", result.Text);
        }

        [Fact]
        public async Task RouteAsync_CommandWithRedirect_FollowsRedirect()
        {
            var mockStart = new Mock<IMessageAdapter>();
                mockStart.Setup(a => a.RouteKey).Returns("start");
                mockStart.Setup(a => a.ProcessMessage("/start", 123))
                .ReturnsAsync(new MessageResult
                {
                    Text = "Redirecting...",
                    RedirectRouteKey = "menu"
                });
            
            var mockMenu = new Mock<IMessageAdapter>();
                mockMenu.Setup(a => a.RouteKey).Returns("menu");
                mockMenu.Setup(a => a.ProcessMessage("menu", 123))
                .ReturnsAsync(new MessageResult { Text = "Menu opened." });

            var router = new AdapterRouter(new[] { mockStart.Object, mockMenu.Object });

            var result = await router.RouteAsync("/start", 123);

            Assert.Equal("Menu opened.", result.Text);
        }

        [Fact]
        public async Task RouteAsync_CallbackHandleCorrectly()
        {
            var mockAdapter = new Mock<IMessageAdapter>();
                mockAdapter.Setup(a => a.RouteKey).Returns("role_select");
                mockAdapter.Setup(a => a.ProcessMessage("client", 123))
                .ReturnsAsync(new MessageResult { Text = "Client role selected." });

            var router = new AdapterRouter(new[] { mockAdapter.Object });
            var result = await router.RouteAsync("role_select:client", 123);

            Assert.Equal("Client role selected.", result.Text);
        }

        [Fact]
        public async Task RouteAsync_CallbackWithRedirect_FollowsRedirect()
        {
            var mockCallback = new Mock<IMessageAdapter>();
            mockCallback.Setup(a => a.RouteKey).Returns("role_select");
            mockCallback.Setup(a => a.ProcessMessage("admin", 123))
                .ReturnsAsync(new MessageResult
                {
                    Text = "Redirecting to admin...",
                    RedirectRouteKey = "admin_menu"
                });
            var mockAdmin = new Mock<IMessageAdapter>();
            mockAdmin.Setup(a => a.RouteKey).Returns("admin_menu");
            mockAdmin.Setup(a => a.ProcessMessage("admin_menu", 123))
                .ReturnsAsync(new MessageResult { Text = "Admin menu." });

            var router = new AdapterRouter(new[] { mockCallback.Object, mockAdmin.Object});

            var result = await router.RouteAsync("role_select:admin", 123);

            Assert.Equal("Admin menu.", result.Text);
        }

        [Fact]
        public async Task RouteAsync_UnknownCommand_ReturnsFallback()
        {
            var router = new AdapterRouter(new IMessageAdapter[0]);

            var result = await router.RouteAsync("/unknown", 123);

            Assert.Equal("Неизвестная команда", result.Text);
        }

        [Fact]
        public async Task RouteAsync_UnknownCallback_ReturnsFallback()
        {
            // Arrange
            var router = new AdapterRouter(new IMessageAdapter[0]);

            // Act
            var result = await router.RouteAsync("nonexistent:command", 123);

            // Assert
            Assert.Equal("Неизвестный callback", result.Text);
        }
    }
}
