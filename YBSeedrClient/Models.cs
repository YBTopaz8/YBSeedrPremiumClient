namespace YBSeedrClient.Models;

/// <summary>
/// The api result.
/// </summary>
public class ApiResult
{
    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }
    /// <summary>
    /// Gets or sets  a value indicating whether to result.
    /// </summary>
    [JsonPropertyName("result")]
    public bool Result { get; set; }

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the user torrent id.
    /// </summary>
    [JsonPropertyName("user_torrent_id")]
    public int? UserTorrentId { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }


    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    /// <summary>
    /// Gets or sets the account.
    /// </summary>
    [JsonPropertyName("account")]
    public SeedrUser? Account { get; set; }
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the url.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// The seedr user.
/// </summary>
public class SeedrUser
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [JsonPropertyName("user_id")]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the premium expire.
    /// </summary>
    [JsonPropertyName("next_payment_due")]
    public string? PremiumExpire { get; set; }

    /// <summary>
    /// Gets or sets the storage total.
    /// </summary>
    [JsonPropertyName("space_max")]
    public long StorageTotal { get; set; }

    /// <summary>
    /// Gets or sets the storage used.
    /// </summary>
    [JsonPropertyName("space_used")]
    public long StorageUsed { get; set; }



    /// <summary>
    /// Gets or sets the torrents storage limit.
    /// </summary>
    [JsonPropertyName("torrents_storage_limit")]
    public int TorrentsStorageLimit { get; set; }


    /// <summary>
    /// Gets or sets the premium.
    /// </summary>
    [JsonPropertyName("premium")]
    public int Premium { get; set; }

    /// <summary>
    /// Gets or sets the package id.
    /// </summary>
    [JsonPropertyName("package_id")]
    public long PackageId { get; set; }

    /// <summary>
    /// Gets or sets the package name.
    /// </summary>
    [JsonPropertyName("package_name")]
    public string? PackageName { get; set; }

    /// <summary>
    /// Gets or sets the bandwidth used.
    /// </summary>
    [JsonPropertyName("bandwidth_used")]
    public long BandwidthUsed { get; set; }
    /// <summary>
    /// Gets or sets the billing plan.
    /// </summary>
    [JsonPropertyName("billing_plan")]
    public BillingPlanInfo? BillingPlan { get; set; }
    /// <summary>
    /// Gets or sets the cancelled.
    /// </summary>
    [JsonPropertyName("cancelled")]
    public int Cancelled { get; set; }
}
/// <summary>
/// The billing plan info.
/// </summary>
public class BillingPlanInfo
{
    /// <summary>
    /// Gets or sets the period.
    /// </summary>
    [JsonPropertyName("period")]
    public string? Period { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore]
    public int Id { get; set; }
}
/// <summary>
/// The seedr folder item.
/// </summary>
public class SeedrFolderItem
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the last update.
    /// </summary>
    [JsonPropertyName("last_update")]
    public string LastUpdate { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets the files count.
    /// </summary>
    [JsonPropertyName("files_count")]
    public int? FilesCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether play video.
    /// </summary>
    [JsonPropertyName("play_video")]
    public bool? PlayVideo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether play audio.
    /// </summary>
    [JsonPropertyName("play_audio")]
    public bool? PlayAudio { get; set; }
}

/// <summary>
/// The seedr file item.
/// </summary>
public class SeedrFileItem
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the folder id.
    /// </summary>
    [JsonPropertyName("folder_id")]
    public long FolderId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the download url.
    /// </summary>
    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stream url.
    /// </summary>
    [JsonPropertyName("stream_url")]
    public string StreamUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the video converted status.
    /// </summary>
    [JsonPropertyName("video_converted_status")]
    public string VideoConvertedStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the torrent hash.
    /// </summary>
    [JsonPropertyName("torrent_hash")]
    public string? TorrentHash { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether deleted.
    /// </summary>
    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the created at.
    /// </summary>
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public string Status { get; set; } = "Completed";
}

/// <summary>
/// The folder content.
/// </summary>
public class FolderContent
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    [JsonPropertyName("code")]
    public int? Code { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public long? ParentId { get; set; }

    /// <summary>
    /// Gets or sets  a value indicating whether to result.
    /// </summary>
    [JsonPropertyName("result")]
    public bool? Result { get; set; }

    /// <summary>
    /// Gets or sets the folders.
    /// </summary>
    [JsonPropertyName("folders")]
    public List<SeedrFolderItem> Folders { get; set; } = new List<SeedrFolderItem>();

    /// <summary>
    /// Gets or sets the files.
    /// </summary>
    [JsonPropertyName("files")]
    public List<SeedrFileItem> Files { get; set; } = new List<SeedrFileItem>();

    /// <summary>
    /// Gets or sets the space max.
    /// </summary>
    [JsonPropertyName("space_max")]
    public long SpaceMax { get; set; }

    /// <summary>
    /// Gets or sets the space used.
    /// </summary>
    [JsonPropertyName("space_used")]
    public long SpaceUsed { get; set; }

    /// <summary>
    /// Gets or sets the torrents.
    /// </summary>
    [JsonPropertyName("torrents")]
    public List<SeedrTransfer> Torrents { get; set; } = new List<SeedrTransfer>();
}

/// <summary>
/// The seedr transfer.
/// </summary>
public class SeedrTransfer
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the user torrent id.
    /// </summary>
    [JsonPropertyName("user_torrent_id")]
    public long? UserTorrentId { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the progress.
    /// </summary>
    [JsonPropertyName("progress")]
    public object? Progress { get; set; }

    /// <summary>
    /// Gets or sets the torrent quality.
    /// </summary>
    [JsonPropertyName("torrent_quality")]
    public int TorrentQuality { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the hash.
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; set; }

    /// <summary>
    /// Gets or sets the parent folder id.
    /// </summary>
    [JsonPropertyName("parent_folder_id")]
    public string? ParentFolderId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the download rate.
    /// </summary>
    [JsonPropertyName("download_rate")]
    public long DownloadRate { get; set; }

    /// <summary>
    /// Gets or sets the upload rate.
    /// </summary>
    [JsonPropertyName("upload_rate")]
    public long UploadRate { get; set; }

    /// <summary>
    /// Gets or sets the created.
    /// </summary>
    [JsonPropertyName("created")]
    public string Created { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated.
    /// </summary>
    [JsonPropertyName("updated")]
    public string Updated { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the folder created id.
    /// </summary>
    [JsonPropertyName("folder_created_id")]
    public long? FolderCreatedId { get; set; }

    /// <summary>
    /// Gets or sets the files.
    /// </summary>
    [JsonPropertyName("files")]
    public List<TransferFiles> Files { get; set; } = new List<TransferFiles>();
}
/// <summary>
/// The transfer files.
/// </summary>
public class TransferFiles
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("files")]
    public string? Name { get; set; }
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    [JsonPropertyName("size")]
    public long Size { get; set; }
}
/// <summary>
/// The api result.
/// </summary>
public class Models
{
    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }
    /// <summary>
    /// Gets or sets  a value indicating whether to result.
    /// </summary>
    [JsonPropertyName("result")]
    public bool Result { get; set; }

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the user torrent id.
    /// </summary>
    [JsonPropertyName("user_torrent_id")]
    public int? UserTorrentId { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }


    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    /// <summary>
    /// Gets or sets the account.
    /// </summary>
    [JsonPropertyName("account")]
    public SeedrUser? Account { get; set; }
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the url.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
/// <summary>
/// The download link result.
/// </summary>
public class DownloadLinkResult
{
    /// <summary>
    /// Gets or sets the url.
    /// </summary>
    public string? Url { get; set; }
    /// <summary>
    /// Gets or sets  a value indicating whether to success.
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string? ErrorMessage { get; set; }
    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    public HttpStatusCode? StatusCode { get; set; }
}