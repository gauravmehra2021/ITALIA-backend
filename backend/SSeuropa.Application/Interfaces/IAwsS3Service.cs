using SSeuropa.Application.DTOs;

namespace SSeuropa.Application.Interfaces;

public interface IAwsS3Service
{
    string BucketName { get; }
    string Region { get; }
    bool IsConfigured { get; }

    Task<PresignedUploadResponseDto> GeneratePresignedUploadUrlAsync(
        string objectKey,
        string mimeType,
        long fileSize,
        TimeSpan ttl,
        CancellationToken ct = default);

    Task<InitiateMultipartResponseDto> InitiateMultipartUploadAsync(
        string objectKey,
        string mimeType,
        long fileSize,
        CancellationToken ct = default);

    Task<string> GeneratePresignedPartUploadUrlAsync(
        string objectKey,
        string uploadId,
        int partNumber,
        TimeSpan ttl,
        CancellationToken ct = default);

    Task<CompleteMultipartResponseDto> CompleteMultipartUploadAsync(
        string objectKey,
        string uploadId,
        IReadOnlyList<MultipartPartDto> parts,
        CancellationToken ct = default);

    Task AbortMultipartUploadAsync(
        string objectKey,
        string uploadId,
        CancellationToken ct = default);

    Task<string> GeneratePresignedDownloadUrlAsync(
        string objectKey,
        TimeSpan ttl,
        CancellationToken ct = default);

    Task<bool> DeleteObjectAsync(
        string objectKey,
        CancellationToken ct = default);

    Task<bool> DoesObjectExistAsync(
        string objectKey,
        CancellationToken ct = default);
}
