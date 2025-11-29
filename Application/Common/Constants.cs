namespace EasyMart.API.Application.Common
{
    public class Constants
    {
        public static class ResponseCodes
        {
            public const string Success = "200";
            public const string Error = "400";
        }
        public static class CustomMessages
        {
            public const string Success = "Success";
            public const string ProductCreated = "Product Created";
            public const string ProductAlreadyExists = "Product with provided name already exists";
        }

    }
}
