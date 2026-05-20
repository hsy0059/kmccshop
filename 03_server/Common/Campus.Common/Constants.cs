namespace Campus.Common;

public static class Constants
{
    public const string JwtSecretKey = "CampusPlatform-JwtSecretKey-Min32Chars!@#";
    public const string JwtIssuer = "CampusPlatform";
    public const string JwtAudience = "CampusPlatformUser";
    public const int JwtExpireHours = 72;

    public const string RedisConnectionString = "localhost:6379";

    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public const int MaxFileSize = 10 * 1024 * 1024;

    public const int VerificationCodeLength = 6;
    public const int VerificationCodeExpireMinutes = 5;

    public const string UserAvatarDirectory = "uploads/avatars";
    public const string ProductImageDirectory = "uploads/products";
    public const string PostImageDirectory = "uploads/posts";
    public const string FeedbackImageDirectory = "uploads/feedbacks";
}
