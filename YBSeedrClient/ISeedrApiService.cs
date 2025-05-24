using YBSeedrClient.Models;

namespace YBSeedrClient;
/// <summary>
/// Interface for interacting with the Seedr API service.
/// </summary>
public interface ISeedrApiService
{
    /// <summary>
    /// Lists the contents of the root folder.
    /// </summary>
    /// <returns>A <see cref="FolderContent"/> object representing the root folder's contents.</returns>
    Task<FolderContent?> ListRootFolderAsync();

    /// <summary>
    /// Lists the contents of a specific folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to list.</param>
    /// <returns>A <see cref="FolderContent"/> object representing the folder's contents.</returns>
    Task<FolderContent?> ListFolderAsync(long folderId);

    /// <summary>
    /// Creates a new folder at the specified path.
    /// </summary>
    /// <param name="fullPath">The full path where the folder should be created.</param>
    /// <returns>An <see cref="ApiResult"/> indicating the result of the operation.</returns>
    Task<ApiResult?> CreateFolderAsync(string fullPath);

    /// <summary>
    /// Renames an existing folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to rename.</param>
    /// <param name="newName">The new name for the folder.</param>
    /// <returns>An <see cref="ApiResult"/> indicating the result of the operation.</returns>
    Task<ApiResult?> RenameFolderAsync(long folderId, string newName);

    /// <summary>
    /// Deletes a folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder to delete.</param>
    /// <returns>An <see cref="ApiResult"/> indicating the result of the operation.</returns>
    Task<ApiResult?> DeleteFolderAsync(long folderId);

    /// <summary>
    /// Retrieves the HLS URL for a file.
    /// </summary>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>An <see cref="ApiResult"/> containing the HLS URL.</returns>
    Task<ApiResult?> GetFileHlsUrlAsync(long fileId);

    /// <summary>
    /// Renames a file.
    /// </summary>
    /// <param name="fileId">The ID of the file to rename.</param>
    /// <param name="newName">The new name for the file.</param>
    /// <returns>An <see cref="ApiResult"/> indicating the result of the operation.</returns>
    Task<ApiResult?> RenameFileAsync(long fileId, string newName);

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="fileId">The ID of the file to delete.</param>
    /// <returns>An <see cref="ApiResult"/> indicating the result of the operation.</returns>
    Task<ApiResult?> DeleteFileAsync(long fileId);

    /// <summary>
    /// Adds a magnet link to the Seedr service.
    /// </summary>
    /// <param name="magnetLink">The magnet link to add.</param>
    /// <returns>An <see cref="ApiResult"/> indicating the result of the operation.</returns>
    Task<ApiResult?> AddMagnetAsync(string magnetLink);

    /// <summary>
    /// Retrieves transfer data for a specific transfer.
    /// </summary>
    /// <param name="transferId">The ID of the transfer.</param>
    /// <returns>A <see cref="SeedrTransfer"/> object containing the transfer data.</returns>
    Task<SeedrTransfer?> GetTransferDataAsync(long transferId);

    /// <summary>
    /// Deletes a transfer.
    /// </summary>
    /// <param name="transferId">The ID of the transfer to delete.</param>
    /// <returns>An <see cref="ApiResult"/> indicating the result of the operation.</returns>
    Task<ApiResult?> DeleteTransferAsync(long transferId);

    /// <summary>
    /// Retrieves user data for the current account.
    /// </summary>
    /// <returns>A <see cref="SeedrUser"/> object containing the user data.</returns>
    Task<SeedrUser?> GetUserDataAsync();

    /// <summary>
    /// Generates a link to a specific folder.
    /// </summary>
    /// <param name="folderId">The ID of the folder.</param>
    /// <returns>A string containing the folder link.</returns>
    string GetFolderLink(long folderId);

    /// <summary>
    /// Generates a link to a specific file.
    /// </summary>
    /// <param name="fileId">The ID of the file.</param>
    /// <returns>A string containing the file link.</returns>
    string GetFileLink(long fileId);

    /// <summary>
    /// Downloads a file using the preferred browser.
    /// </summary>
    /// <param name="fileId">The ID of the file to download.</param>
    /// <returns>A boolean indicating whether the download was successful.</returns>
    Task<bool> DownloadFileOnPreferredBrowserAsync(long fileId);

    /// <summary>
    /// Downloads a folder using the preferred browser.
    /// </summary>
    /// <param name="folderId">The ID of the folder to download.</param>
    /// <returns>A boolean indicating whether the download was successful.</returns>
    Task<bool> DownloadFolderOnPreferredBrowserAsync(long folderId);
}
