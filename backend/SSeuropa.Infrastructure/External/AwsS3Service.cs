using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;

namespace SSeuropa.Infrastructure.External;

public class AwsS3Service : IAwsS3Service
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<AwsS3Service> _logger;
    private readonly IAmazonS3? _s3Client;

    public string BucketName { get; }
    public string Region { get; }
    public bool IsConfigured { get; }

    public AwsS3Service(IConfiguration config, IWebHostEnvironment env, ILogger<AwsS3Service> logger)
    {
        _config = config;
        _env = env;
        _logger = logger;

        var accessKey = _config["Aws:AccessKeyId"]
            ?? _config["AWS_ACCESS_KEY_ID"]
            ?? Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");

        var secretKey = _config["Aws:SecretAccessKey"]
            ?? _config["AWS_SECRET_ACCESS_KEY"]
            ?? Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

        Region = _config["Aws:Region"]
            ?? _config["AWS_REGION"]
            ?? Environment.GetEnvironmentVariable("AWS_REGION")
            ?? "eu-central-1";

        BucketName = _config["Aws:BucketName"]
            ?? _config["AWS_S3_BUCKET_NAME"]
            ?? Environment.GetEnvironmentVariable("AWS_S3_BUCKET_NAME")
            ?? "sseuropa-learning-media";

        if (!string.IsNullOrWhiteSpace(accessKey) && !string.IsNullOrWhiteSpace(secretKey))
        {
            try
            {
                var regionEndpoint = RegionEndpoint.GetBySystemName(Region) ?? RegionEndpoint.EUCentral1;
                _s3Client = new AmazonS3Client(accessKey, secretKey, regionEndpoint);
                IsConfigured = true;
                _logger.LogInformation("AWS S3 Service initialized with bucket '{BucketName}' in region '{Region}'.", BucketName, Region);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to initialize AWS S3 Client with provided credentials. Falling back to local storage adapter.");
                IsConfigured = false;
            }
        }
        else
        {
            _logger.LogInformation("AWS S3 credentials not detected. Operating in secure local dev simulation mode.");
            IsConfigured = false;
        }
    }

    public Task<PresignedUploadResponseDto> GeneratePresignedUploadUrlAsync(
        string objectKey,
        string mimeType,
        long fileSize,
        TimeSpan ttl,
        CancellationToken ct = default)
    {
        var expiresAt = DateTime.UtcNow.Add(ttl);

        if (IsConfigured && _s3Client is not null)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = expiresAt,
                ContentType = mimeType
            };

            var presignedUrl = _s3Client.GetPreSignedURL(request);
            return Task.FromResult(new PresignedUploadResponseDto(
                UploadUrl: presignedUrl,
                ObjectKey: objectKey,
                BucketName: BucketName,
                Region: Region,
                ExpiresAtUtc: expiresAt,
                IsDirectUpload: true
            ));
        }

        // Local dev fallback URL
        var fallbackUrl = $"/api/uploads/local-direct?key={Uri.EscapeDataString(objectKey)}";
        return Task.FromResult(new PresignedUploadResponseDto(
            UploadUrl: fallbackUrl,
            ObjectKey: objectKey,
            BucketName: "local-simulation",
            Region: "local",
            ExpiresAtUtc: expiresAt,
            IsDirectUpload: true
        ));
    }

    public async Task<InitiateMultipartResponseDto> InitiateMultipartUploadAsync(
        string objectKey,
        string mimeType,
        long fileSize,
        CancellationToken ct = default)
    {
        const long partSize = 10 * 1024 * 1024; // 10MB per part
        var totalParts = (int)Math.Ceiling((double)fileSize / partSize);
        if (totalParts <= 0) totalParts = 1;

        if (IsConfigured && _s3Client is not null)
        {
            var initReq = new InitiateMultipartUploadRequest
            {
                BucketName = BucketName,
                Key = objectKey,
                ContentType = mimeType
            };

            var res = await _s3Client.InitiateMultipartUploadAsync(initReq, ct);
            return new InitiateMultipartResponseDto(
                UploadId: res.UploadId,
                ObjectKey: objectKey,
                PartSize: partSize,
                TotalParts: totalParts
            );
        }

        return new InitiateMultipartResponseDto(
            UploadId: Guid.NewGuid().ToString("N"),
            ObjectKey: objectKey,
            PartSize: partSize,
            TotalParts: totalParts
        );
    }

    public Task<string> GeneratePresignedPartUploadUrlAsync(
        string objectKey,
        string uploadId,
        int partNumber,
        TimeSpan ttl,
        CancellationToken ct = default)
    {
        var expiresAt = DateTime.UtcNow.Add(ttl);

        if (IsConfigured && _s3Client is not null)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = objectKey,
                UploadId = uploadId,
                PartNumber = partNumber,
                Verb = HttpVerb.PUT,
                Expires = expiresAt
            };

            return Task.FromResult(_s3Client.GetPreSignedURL(request));
        }

        return Task.FromResult($"/api/uploads/local-part?uploadId={uploadId}&partNumber={partNumber}&key={Uri.EscapeDataString(objectKey)}");
    }

    public async Task<CompleteMultipartResponseDto> CompleteMultipartUploadAsync(
        string objectKey,
        string uploadId,
        IReadOnlyList<MultipartPartDto> parts,
        CancellationToken ct = default)
    {
        if (IsConfigured && _s3Client is not null)
        {
            var completeReq = new CompleteMultipartUploadRequest
            {
                BucketName = BucketName,
                Key = objectKey,
                UploadId = uploadId,
                PartETags = parts.Select(p => new PartETag { PartNumber = p.PartNumber, ETag = p.ETag }).ToList()
            };

            var res = await _s3Client.CompleteMultipartUploadAsync(completeReq, ct);
            return new CompleteMultipartResponseDto(
                ObjectKey: objectKey,
                Location: res.Location ?? $"https://{BucketName}.s3.{Region}.amazonaws.com/{objectKey}",
                Message: "S3 Multipart upload completed successfully."
            );
        }

        return new CompleteMultipartResponseDto(
            ObjectKey: objectKey,
            Location: $"/uploads/{objectKey}",
            Message: "Local simulated multipart upload completed successfully."
        );
    }

    public async Task AbortMultipartUploadAsync(
        string objectKey,
        string uploadId,
        CancellationToken ct = default)
    {
        if (IsConfigured && _s3Client is not null)
        {
            await _s3Client.AbortMultipartUploadAsync(new AbortMultipartUploadRequest
            {
                BucketName = BucketName,
                Key = objectKey,
                UploadId = uploadId
            }, ct);
        }
    }

    public Task<string> GeneratePresignedDownloadUrlAsync(
        string objectKey,
        TimeSpan ttl,
        CancellationToken ct = default)
    {
        if (IsConfigured && _s3Client is not null)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = objectKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.Add(ttl)
            };

            return Task.FromResult(_s3Client.GetPreSignedURL(request));
        }

        // Return path directly
        if (objectKey.StartsWith("http://") || objectKey.StartsWith("https://"))
        {
            return Task.FromResult(objectKey);
        }

        return Task.FromResult($"/uploads/{objectKey.TrimStart('/')}");
    }

    public async Task<bool> DeleteObjectAsync(string objectKey, CancellationToken ct = default)
    {
        if (IsConfigured && _s3Client is not null)
        {
            try
            {
                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = objectKey
                }, ct);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete S3 object {ObjectKey}", objectKey);
                return false;
            }
        }
        return true;
    }

    public async Task<bool> DoesObjectExistAsync(string objectKey, CancellationToken ct = default)
    {
        if (IsConfigured && _s3Client is not null)
        {
            try
            {
                await _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
                {
                    BucketName = BucketName,
                    Key = objectKey
                }, ct);
                return true;
            }
            catch
            {
                return false;
            }
        }
        return true;
    }
}
