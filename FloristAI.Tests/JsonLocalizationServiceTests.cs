using FloristAI.Application.Language;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text.Json;
using Xunit;

namespace FloristAI.Tests
{
    public class JsonLocalizationServiceTests
    {
        private const string TestFolder = "TestLocalization";

        private void PrepareTestJson(string langCode, Dictionary<string, string> content)
        {
            var localizationDir = Path.Combine(Directory.GetCurrentDirectory(), "Localization"); 
            Directory.CreateDirectory(localizationDir);

            var path = Path.Combine(localizationDir, $"{langCode}.json");
            var json = JsonSerializer.Serialize(content);
            File.WriteAllText(path, json);
        }


        private JsonLocalizationService CreateService()
        {
            var envMock = new Mock<IHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());

            var logger = NullLogger<JsonLocalizationService>.Instance;

            return new JsonLocalizationService(envMock.Object, logger);
        }

        [Fact]
        public void GetString_ReturnsLocalizedValue_WhenKeyExists()
        {
            PrepareTestJson("ru", new Dictionary<string, string>
            {
                { "Role_Admin", "Администратор" }
            });

            var service = CreateService();
            var result = service.GetString("Role_Admin", "ru");

            Assert.Equal("Администратор", result);
        }

        [Fact]
        public void GetString_ReturnsKey_WhenKeyNotExists()
        {
            PrepareTestJson("ru", new Dictionary<string, string>());

            var service = CreateService();
            var result = service.GetString("Unknown_Key", "ru");

            Assert.Equal("Unknown_Key", result);
        }

        [Fact]
        public void GetString_ReturnsKey_WhenLangNotLoaded()
        {
            var service = CreateService();
            var result = service.GetString("Role_Admin", "unknown");

            Assert.Equal("Role_Admin", result);
        }
    }
}
