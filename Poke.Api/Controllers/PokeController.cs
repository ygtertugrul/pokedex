using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Poke.Api.Service;

namespace Poke.Api.Controllers
{
    [ApiController]
    [Route("pokemon")]
    public class PokeController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public PokeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Pokemon Info
        /// </summary>
        [HttpGet("{pokeName}")]
        public async Task<ActionResult> GetPokemonInfo([FromRoute] string pokeName)
        {
            var pokemonInfo = await _mediator.Send(new GetPokeInfo(pokeName));
            return Ok(pokemonInfo);
        }
        
        /// <summary>
        /// GetPokemon Info with Description Translation
        /// </summary>
        [HttpGet("translated/{pokeName}")]
        public async Task<ActionResult> GetPokemonWithTranslatedDescription([FromRoute] string pokeName)
        {
            var pokemonTranslatedInfo = await _mediator.Send(new GetPokeWithTranslation(pokeName));
            return Ok(pokemonTranslatedInfo);
        }
    }
}