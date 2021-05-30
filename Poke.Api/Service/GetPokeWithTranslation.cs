using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Poke.Api.Extensions;
using Poke.Api.Model;

namespace Poke.Api.Service
{
    #region Query /Command 
    public class GetPokeWithTranslation : IRequest<GetPokeInfoResponse>
    {
        public GetPokeWithTranslation(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
    }
    #endregion
    
    #region Query Request Validator 
    public class GetPokeWithTranslationValidator : AbstractValidator<GetPokeWithTranslation>
    {
        public GetPokeWithTranslationValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required");
        }
    }
    #endregion
    
    #region Handler
    public class GetPokeWithTranslationHandler : IRequestHandler<GetPokeWithTranslation, GetPokeInfoResponse>
    {
        private readonly IMediator _mediator;

        public GetPokeWithTranslationHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<GetPokeInfoResponse> Handle(GetPokeWithTranslation request, CancellationToken cancellationToken)
        {
            //If it exists in cache, then mediator cache pipeline will return it from cache
            var pokeInfo = await _mediator.Send(new GetPokeInfo(request.Name), cancellationToken);

            if (pokeInfo.Habitat == PokeHabitat.Cave.GetDescription() || pokeInfo.Islegendary)
            {
                pokeInfo.Description = await _mediator.Send(new GetTranslatedDescription(pokeInfo.Description, TranslationType.Yoda), cancellationToken) ?? pokeInfo.Description;
            }
            else
            {
                pokeInfo.Description = await _mediator.Send(new GetTranslatedDescription(pokeInfo.Description, TranslationType.Shakespeare), cancellationToken)?? pokeInfo.Description;
            }

            return pokeInfo;
        }
    }
    #endregion
}