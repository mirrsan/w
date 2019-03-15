using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZadatakNeki.Models;

namespace ZadatakNeki.Filter
{
    public class MajFilter : IResultFilter, IExceptionFilter
    {

        public void OnException(ExceptionContext context)
        {
                context.ExceptionHandled = true;

                VratiRezultat rezultat = new VratiRezultat()
                {
                    IsError = true,
                    Error = new Error()
                    {
                        Message = "Desila se neka greska, snadji se videe sta je to.",
                        Exception = context.Exception.Message,
                        StackTrace = context.Exception.StackTrace
                    }
                };
                context.Result = new ObjectResult(rezultat);
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            try
            {
                ObjectResult result = context.Result as ObjectResult;

                if (result.StatusCode >= 200 && result.StatusCode < 300)
                {
                    VratiRezultat vrati = new VratiRezultat()
                    {
                        Data = context.Result
                    };
                    context.Result = new ObjectResult(vrati);
                }
            }
            catch (InvalidCastException ex)
            {
                Error greska = new Error()
                {
                    Message = "'context.result' nije dobro kastovan",
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace
                };
                context.Result = new ObjectResult(greska);
            }
            catch (Exception ex)
            {
                Error greska = new Error()
                {
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace
                };
                context.Result = new ObjectResult(greska);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}

