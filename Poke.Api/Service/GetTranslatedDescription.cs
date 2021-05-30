using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Poke.Api.Integration;
using Poke.Api.Model;

namespace Poke.Api.Service
{
    #region Query /Command 
    public class GetTranslatedDescription : IRequest<string>, ICacheable
    {
        public GetTranslatedDescription(string description, TranslationType type)
        {
            Description = description;
            Type = type;
        }
        
        public string Description { get; }
        public TranslationType Type { get; }
        public string CacheKey => $"translation:{Type}:{Description}";
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(30);
    }
    #endregion
    
    #region Query Request Validator 
    public class GetTranslatedDescriptionValidator : AbstractValidator<GetTranslatedDescription>
    {
        public GetTranslatedDescriptionValidator()
        {
            RuleFor(p => p.Description).NotEmpty().WithMessage("Description is required");
        }
    }
    #endregion
    
    #region Handler
    public class GetTranslatedDescriptionHandler : IRequestHandler<GetTranslatedDescription, string>
    {
        private readonly ITranslationServiceApi _translationServiceApi;

        public GetTranslatedDescriptionHandler(ITranslationServiceApi translationServiceApi)
        {
            _translationServiceApi = translationServiceApi;
        }

        public async Task<string> Handle(GetTranslatedDescription request, CancellationToken cancellationToken)
        {
            if (request.Type == TranslationType.Yoda)
            {
                return await _translationServiceApi.GetYodaTranslation(request.Description);
            }

            if (request.Type == TranslationType.Shakespeare)
            {
                return await _translationServiceApi.GetShakespeareTranslation(request.Description);
            }

            throw new CustomException("TranslationTypeError", (int) HttpStatusCode.BadRequest);
        }
    }
    #endregion
}