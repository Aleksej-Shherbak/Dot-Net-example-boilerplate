using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models.Http;

namespace WebApplication.Infrastructure
{
    public class ErrorResponseGenerator
    {
        public static BadRequestObjectResult ErrorResponse(ActionContext actionContext) {  
            return new BadRequestObjectResult(new { Errors = actionContext.ModelState  
                .Where(modelError => modelError.Value.Errors.Count > 0)  
                .Select(modelError => new ErrorResponse {  
                    ErrorField = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(modelError.Key),  
                    Description = modelError.Value.Errors.FirstOrDefault()?.ErrorMessage  
                }).ToList()});  
        } 
    }
}