using System;
namespace JobsityChatroom.WebAPI.Models
{
    public class ApiResponse
    {
        public string Message { get; set; }

        public ApiResponse(string message)
        {
            Message = message;
        }
    }
}
