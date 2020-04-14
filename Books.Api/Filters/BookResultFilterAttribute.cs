using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Filters
{
    public class BookResultFilterAttribute : ResultFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultFromAction= context.Result as ObjectResult;//Actionresult may not necessaryly return Ok, so downcast it to objectresult where all the results contains a value which are derived from
            if(resultFromAction?.Value ==null || resultFromAction.StatusCode<200 || resultFromAction.StatusCode >= 300)//dont manipulate resut if it is notfound because there is nothing to map.in this case continue to the next delegate
            {
                await next();
                return;
            }
            

            //add nuget AutoMapper.Extensions.Microsoft.DependencyInjection
            resultFromAction.Value = Mapper.Map <Models.Book> (resultFromAction.Value);

            //write ienumerable mapping in separate filter to have single responsibility principle
            //if (typeof(IEnumerable).IsAssignableFrom(resultFromAction.Value.GetType()))
            //{
            //    //mapping code
            //}

            await next();//call next delegate in the filter pipeline
        }
    }
}
