using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR.Pipeline;
using ValidationException = FluentValidation.ValidationException;

namespace Poke.Api.Pipeline
{
    //This pipeline manages request validation before executing function called
    public class PreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IValidator<TRequest> _validator;

        public PreProcessor(IValidator<TRequest> validator = null)
        {
            _validator = validator;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            if (_validator == null)
            {
                return Task.CompletedTask;
            }
            
            var failures = _validator.Validate(request).Errors.Where(f => f != null).ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return Task.CompletedTask;
        }
    }
}