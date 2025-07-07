namespace FPTU_ProposalGuard.Application.Configurations;

public class AmazonS3Settings
{
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
    public required string Region { get; init; }
    public required string BucketName { get; init; }
}