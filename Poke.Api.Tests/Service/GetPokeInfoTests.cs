using System.Collections.Generic;
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
    public class GetPokeInfoTests
    {
        public class GetPokeInfoTestsFixture
        {
            private readonly Mock<IPokeApiService> _mockPokeApiService;
            
            public GetPokeInfoTestsFixture()
            {
                _mockPokeApiService = new Mock<IPokeApiService>();
            }
            
            public GetPokeInfoTestsFixture SetupApiService(Pokemon response)
            {
                _mockPokeApiService.Setup(o => o.GetPokeInfo(It.IsAny<string>())).ReturnsAsync(response);
                return this;
            } 
            
            public GetPokeInfoHandler CreateSut()
            {
                return new GetPokeInfoHandler(_mockPokeApiService.Object);
            }
        }

        [Fact]
        public async Task ExpectRightValues_WhenPokeFound()
        {
            //Arrange
            var apiResponse = new Pokemon
            {
                flavor_text_entries = new List<Description>
                {
                    new Description
                    {
                        flavor_text = "The sample description",
                        language = new Language
                        {
                            name = "en"
                        }
                    }
                },
                habitat = new Habitat
                {
                    name = "sample-habitat"
                },
                is_legendary = true,
                name = "sample-poke"
            };
            
            var request = new GetPokeInfo("sample-poke");
            var fixture = new GetPokeInfoTestsFixture();
            var getPokeInfoHandler = fixture
                .SetupApiService(apiResponse)
                .CreateSut();
            
            //Act
            var response = await getPokeInfoHandler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            response.Should().NotBeNull();
            response.Name.Should().Be("sample-poke");
            response.Description.Should().Be("The sample description");
            response.Habitat.Should().Be("sample-habitat");
            response.Islegendary.Should().BeTrue();
        }
        
        [Fact]
        public async Task ExpectDescriptionNullandAcceptable_WhenEnDescriptionIsNotFound()
        {
            //Arrange
            var apiResponse = new Pokemon
            {
                flavor_text_entries = new List<Description>
                {
                    new Description
                    {
                        flavor_text = "The sample description",
                        language = new Language
                        {
                            name = "none"
                        }
                    }
                },
                habitat = new Habitat
                {
                    name = "sample-habitat"
                },
                is_legendary = true,
                name = "sample-poke"
            };
            
            var request = new GetPokeInfo("sample-poke");
            var fixture = new GetPokeInfoTestsFixture();
            var getPokeInfoHandler = fixture
                .SetupApiService(apiResponse)
                .CreateSut();
            
            //Act
            var response = await getPokeInfoHandler.Handle(request, It.IsAny<CancellationToken>());

            //Assert
            response.Should().NotBeNull();
            response.Name.Should().Be("sample-poke");
            response.Description.Should().BeNull();
            response.Habitat.Should().Be("sample-habitat");
            response.Islegendary.Should().BeTrue();
        }
    }
}