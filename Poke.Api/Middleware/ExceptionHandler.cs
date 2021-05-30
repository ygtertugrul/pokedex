using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Poke.Api.Model;

namespace Poke.Api.Middleware
{
    //Exception Handler in .NetCore Middleware
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CustomException customException)
            {
                await ClearResponseAndBuildErrorDto(context, customException.StatusCode, new List<string> { customException.Message }).ConfigureAwait(false);
            }
            catch (ValidationException validationException)
            {
                var messages = new List<string>();
                if (validationException.Errors.Any())
                {
                    messages.AddRange(validationException.Errors.Select(i => i.ErrorMessage));
                }
                else
                {
                    messages.Add(validationException.Message);
                }
                await ClearResponseAndBuildErrorDto(context, StatusCodes.Status400BadRequest, messages).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ClearResponseAndBuildErrorDto(context, StatusCodes.Status500InternalServerError, new List<string> { e.Message }).ConfigureAwait(false);
            }
        }

        private static Task ClearResponseAndBuildErrorDto(HttpContext context, int statusCode, List<string> messages)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new {Messages = messages}), Encoding.UTF8);
        }
    }
}