using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Poke.Api.Service;
using Xunit;

namespace Poke.Api.Tests.Service
{
    public class GetPokeWithTranslationTests
    {
        public class GetPokeWithTranslationTestsFixture
        {
            private readonly Mock<IMediator> _mockMediator;
            
            public GetPokeWithTranslationTestsFixture()
            {
                _mockMediator = new Mock<IMediator>();
            }
            
            public GetPokeWithTranslationTestsFixture SetupMediator(GetPokeInfoResponse response, string description)
            {
                _mockMediator.Setup(o => o.Send(It.IsAny<GetPokeInfo>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
                _mockMediator.Setup(o => o.Send(It.IsAny<GetTranslatedDescription>(), It.IsAny<CancellationToken>())).ReturnsAsync(description);
                return this;
            }
            
            public GetPokeWithTranslationTestsFixture CheckGetTranslatedDescription(Times times)
            {
                _mockMediator.Verify(o => o.Send(It.IsAny<GetTranslatedDescription>(), It.IsAny<CancellationToken>()), times);
                return this;
            }
            
            public GetPokeWithTranslationHandler CreateSut()
            {
                return new GetPokeWithTranslationHandler(_mockMediator.Object);
            }
        }

        [Theory]
        [InlineData(true, "cave")]
        [InlineData(false, "cave")]
        [InlineData(true, "another-habitat")]
        public async Task ExpectYodaTranslation_WhenHabitatIsCave(bool isLegendary, string habitat)
        {
            //Arrange
            var getPokeResponse = new GetPokeInfoResponse
            {
                Description = "sample-description",
                Habitat = habitat,
                Islegendary = isLegendary,
                Name = "sample-poke"
            };
            
            var request = new GetPokeWithTranslation("sample-poke");
            var fixture = new GetPokeWithTranslationTestsFixture();
            var getPokeWithTranslationHandler = fixture
                .SetupMediator(getPokeResponse, "yoda-translation")
                .CreateSut();
            
            //Act
            var response = await getPokeWithTranslationHandler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            fixture.CheckGetTranslatedDescription(Times.Once());
            response.Should().NotBeNull();
            response.Name.Should().Be("sample-poke");
            response.Description.Should().Be("yoda-translation");
            response.Habitat.Should().Be(habitat);
            response.Islegendary.Should().Be(isLegendary);
        }
        
        [Fact]
        public async Task ExpectShakespeareTranslation_WhenHabitatIsNotCaveAAndNotLegend()
        {
            //Arrange
            var getPokeResponse = new GetPokeInfoResponse
            {
                Description = "sample-description",
                Habitat = "another-habitat",
                Islegendary = false,
                Name = "sample-poke"
            };
            
            var request = new GetPokeWithTranslation("sample-poke");
            var fixture = new GetPokeWithTranslationTestsFixture();
            var getPokeWithTranslationHandler = fixture
                .SetupMediator(getPokeResponse, "shakespeare-translation")
                .CreateSut();
            
            //Act
            var response = await getPokeWithTranslationHandler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            fixture.CheckGetTranslatedDescription(Times.Once());
            response.Should().NotBeNull();
            response.Name.Should().Be("sample-poke");
            response.Description.Should().Be("shakespeare-translation");
            response.Habitat.Should().Be("another-habitat");
            response.Islegendary.Should().BeFalse();
        }
        
        [Fact]
        public async Task ExpectDefaultDescription_WhenTranslationApiHasProblem()
        {
            //Arrange
            var getPokeResponse = new GetPokeInfoResponse
            {
                Description = "sample-description",
                Habitat = "sample-habitat",
                Islegendary = true,
                Name = "sample-poke"
            };
            
            var request = new GetPokeWithTranslation("sample-poke");
            var fixture = new GetPokeWithTranslationTestsFixture();
            var getPokeWithTranslationHandler = fixture
                .SetupMediator(getPokeResponse, null)
                .CreateSut();
            
            //Act
            var response = await getPokeWithTranslationHandler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            fixture.CheckGetTranslatedDescription(Times.Once());
            response.Should().NotBeNull();
            response.Name.Should().Be("sample-poke");
            response.Description.Should().Be("sample-description");
            response.Habitat.Should().Be("sample-habitat");
            response.Islegendary.Should().BeTrue();
        }
    }
}