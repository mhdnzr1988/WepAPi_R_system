﻿using System.Net;
using System.Text.Json;


namespace WepAPiR_system.CommonUtility
{
    public class GlobalExceptionHandingClass
    {
        private readonly RequestDelegate _next; //next middleware component in the HTTP request pipeline
        private readonly ILogger<GlobalExceptionHandingClass> _logger; //ogger used to log messages
        public GlobalExceptionHandingClass(RequestDelegate next, ILogger<GlobalExceptionHandingClass> logger) //Dependancy injected by ASP.NET Core
        {
            _next = next;   //
            _logger = logger;
        }

        //This method is the entry point for middleware logic
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Proceed to the next middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error. Please try again later.",
                    Detailed = ex.Message // You can hide this in production
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
