using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Poke.Api.Integration;
using Poke.Api.Model;
using Poke.Api.Service;
using Xunit;

namespace Poke.Api.Tests.Service
{
    public class GetTranslatedDescriptionTests
    {
        public class GetTranslatedDescriptionTestsFixture
        {
            private readonly Mock<ITranslationServiceApi> _mockTranslationApiService;

            public GetTranslatedDescriptionTestsFixture()
            {
                _mockTranslationApiService = new Mock<ITranslationServiceApi>();
            }

            public GetTranslatedDescriptionTestsFixture SetupApiService(string yodaTranslation,
                string shakespeareTranslation)
            {
                _mockTranslationApiService.Setup(o => o.GetYodaTranslation(It.IsAny<string>()))
                    .ReturnsAsync(yodaTranslation);
                _mockTranslationApiService.Setup(o => o.GetShakespeareTranslation(It.IsAny<string>()))
                    .ReturnsAsync(shakespeareTranslation);
                return this;
            }
            
            public GetTranslatedDescriptionTestsFixture CheckGetYodaTranslation(Times times)
            {
                _mockTranslationApiService.Verify(o => o.GetYodaTranslation(It.IsAny<string>()), times);
                return this;
            }
            
            public GetTranslatedDescriptionTestsFixture CheckGetShakespeareTranslation(Times times)
            {
                _mockTranslationApiService.Verify(o => o.GetShakespeareTranslation(It.IsAny<string>()), times);
                return this;
            }

            public GetTranslatedDescriptionHandler CreateSut()
            {
                return new GetTranslatedDescriptionHandler(_mockTranslationApiService.Object);
            }
        }

        [Fact]
        public async Task ExpectShakespeareTranslation_WhenTypeIsShakespeare()
        {
            //Arrange
            var request = new GetTranslatedDescription("sample-description", TranslationType.Shakespeare);
            var fixture = new GetTranslatedDescriptionTestsFixture();
            var getTranslatedDescriptionHandler = fixture
                .SetupApiService("Yoda Description", "Shakespeare Description")
                .CreateSut();

            //Act
            var response = await getTranslatedDescriptionHandler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            fixture.CheckGetShakespeareTranslation(Times.Once());
            fixture.CheckGetYodaTranslation(Times.Never());
            response.Should().Be("Shakespeare Description");
        }
        
        [Fact]
        public async Task ExpectYodaTranslation_WhenTypeIsYoda()
        {
            //Arrange
            var request = new GetTranslatedDescription("sample-description", TranslationType.Yoda);
            var fixture = new GetTranslatedDescriptionTestsFixture();
            var getTranslatedDescriptionHandler = fixture
                .SetupApiService("Yoda Description", "Shakespeare Description")
                .CreateSut();

            //Act
            var response = await getTranslatedDescriptionHandler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            fixture.CheckGetShakespeareTranslation(Times.Never());
            fixture.CheckGetYodaTranslation(Times.Once());
            response.Should().Be("Yoda Description");
        }
        
        [Fact]
        public async Task ExpectException_WhenTypeIsNotValid()
        {
            //Arrange
            var request = new GetTranslatedDescription("sample-description", TranslationType.JustForTest);
            var fixture = new GetTranslatedDescriptionTestsFixture();
            var getTranslatedDescriptionHandler = fixture
                .SetupApiService("Yoda Description", "Shakespeare Description")
                .CreateSut();

            //Act
            var ex = await Record.ExceptionAsync(async () => await getTranslatedDescriptionHandler.Handle(request, It.IsAny<CancellationToken>()));

            //Assert
            fixture.CheckGetShakespeareTranslation(Times.Never());
            fixture.CheckGetYodaTranslation(Times.Never());
            ex.Should().BeOfType<CustomException>();
            ex.Message.Should().Be("TranslationTypeError");
            ((CustomException)ex).StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
    }
}