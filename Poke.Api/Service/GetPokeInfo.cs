using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Poke.Api.Integration;
using Poke.Api.Model;

namespace Poke.Api.Service
{
    #region Query /Command 
    public class GetPokeInfo : IRequest<GetPokeInfoResponse>, ICacheable
    {
        public GetPokeInfo(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
        public string CacheKey => $"poke-info:{Name}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
    }
    #endregion
    
    #region Query Request Validator 
    public class GetPokeInfoValidator : AbstractValidator<GetPokeInfo>
    {
        public GetPokeInfoValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Pokemon Name is required");
        }
    }
    #endregion
    
    #region Handler 
    public class GetPokeInfoHandler : IRequestHandler<GetPokeInfo, GetPokeInfoResponse>
    {
        private readonly IPokeApiService _pokeApiService;

        public GetPokeInfoHandler(IPokeApiService pokeApiService)
        {
            _pokeApiService = pokeApiService;
        }

        public async Task<GetPokeInfoResponse> Handle(GetPokeInfo request, CancellationToken cancellationToken)
        {
            var pokeApiResponse = await _pokeApiService.GetPokeInfo(request.Name);
            var description = pokeApiResponse.flavor_text_entries
                .FirstOrDefault(d => d.language.name == "en")?.flavor_text.Replace("\n", " ");;

            return new GetPokeInfoResponse
            {
                Name = pokeApiResponse.name,
                Description = description,
                Habitat = pokeApiResponse.habitat.name,
                Islegendary = pokeApiResponse.is_legendary
            };
        }
    }
    #endregion

    #region Service Response

    public class GetPokeInfoResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Habitat { get; set; }
        public bool Islegendary { get; set; }
    }
    #endregion
}