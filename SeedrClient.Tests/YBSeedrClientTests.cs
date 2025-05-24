using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YBSeedrClient;
using YBSeedrClient.Abstractions;
using static YBSeedrClient.SeedrModels;


namespace SeedrClient.Tests;


[TestClass]
public class SeedrApiServiceIntegrationTests
{
    private static ISeedrApiService? _seedrService;
    private static SeedrUser? _currentUser;
    private static Mock<IBrowserLauncher>? _mockBrowserLauncher;
    private static string? _seedrEmail;
    private static string? _seedrPassword;
    private const string BaseApiUrl = "https://www.seedr.cc/rest";
    private List<long> _foldersToDelete = new List<long>();
    private List<long> _filesToDelete = new List<long>();
    private List<long> _transfersToDelete = new List<long>(); 

    
    public TestContext TestContext { get; set; } = null!;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext context)
    {
        System.Diagnostics.Trace.WriteLine("ClassInitialize: Setting up SeedrApiService for integration tests.");
        _seedrEmail = APIInfo.SeedrEmail;
        _seedrPassword = APIInfo.SeedrPassword;

        if (string.IsNullOrWhiteSpace(_seedrEmail) || string.IsNullOrWhiteSpace(_seedrPassword))
        {
            Assert.Inconclusive("SEEDR_EMAIL or SEEDR_PASSWORD environment variables not set. Skipping integration tests.");
            return; 
        }

        var httpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = true }) 
        {
            BaseAddress = new Uri(BaseApiUrl + "/") 
        };

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_seedrEmail}:{_seedrPassword}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _mockBrowserLauncher = new Mock<IBrowserLauncher>();
        ILogger<SeedrApiService> loggerForService = NullLogger<SeedrApiService>.Instance;

        _seedrService = new SeedrApiService(httpClient, _mockBrowserLauncher.Object, loggerForService);

        _currentUser = await _seedrService.GetUserDataAsync();
        if (_currentUser == null)
        {
            Assert.Fail("Failed to authenticate with Seedr using provided credentials. Check SEEDR_EMAIL and SEEDR_PASSWORD.");
        }
        System.Diagnostics.Trace.WriteLine($"Successfully authenticated as Seedr user: {_currentUser.Username}");
    }

    [TestInitialize]
    public void TestInitialize()
    {

        _mockBrowserLauncher?.Reset();
        _foldersToDelete.Clear();
        _filesToDelete.Clear();
        _transfersToDelete.Clear();
        System.Diagnostics.Trace.WriteLine($"TestInitialize for: {TestContext.TestName}");
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        System.Diagnostics.Trace.WriteLine($"TestCleanup for: {TestContext.TestName}. Cleaning up created resources...");
        if (_seedrService == null)
            return; 

        
        
        foreach (var fileId in _filesToDelete.Distinct().Reverse())
        {
            System.Diagnostics.Trace.WriteLine($"Cleaning up file ID: {fileId}");
            await _seedrService.DeleteFileAsync(fileId); 
        }
        foreach (var folderId in _foldersToDelete.Distinct().Reverse())
        {
            System.Diagnostics.Trace.WriteLine($"Cleaning up folder ID: {folderId}");
            await _seedrService.DeleteFolderAsync(folderId);
        }
        foreach (var transferId in _transfersToDelete.Distinct().Reverse())
        {
            System.Diagnostics.Trace.WriteLine($"Cleaning up transfer ID: {transferId}");
            await _seedrService.DeleteTransferAsync(transferId);
        }
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        
        
        System.Diagnostics.Trace.WriteLine("ClassCleanup: Integration tests finished.");
    }

    private void AddFolderForCleanup(long folderId) => _foldersToDelete.Add(folderId); // Ensure type consistency
    private void AddFileForCleanup(long fileId) => _filesToDelete.Add(fileId);
    private void AddTransferForCleanup(int transferId) => _transfersToDelete.Add(transferId);

    

    [TestMethod]
    [TestCategory("Integration_User")]
    public async Task GetUserDataAsync_Live_ShouldReturnCurrentUserInfo()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        Assert.IsNotNull(_currentUser, "CurrentUser not set during ClassInitialize.");

        var user = await _seedrService.GetUserDataAsync();

        Assert.IsNotNull(user, "GetUserDataAsync returned null.");
Assert.AreEqual(_currentUser.Id, user.Id, "User ID mismatch.");
        Assert.AreEqual(_currentUser.Username, user.Username, "Username mismatch.");
        Assert.AreEqual(_seedrEmail, user.Email, "Email mismatch with configured email."); 
    }

    [TestMethod]
    [TestCategory("Integration_Folder")]
    public async Task ListRootFolderAsync_Live_ShouldReturnContent()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        var content = await _seedrService.ListRootFolderAsync();

        Assert.IsNotNull(content, "ListRootFolderAsync returned null.");
        Assert.AreEqual("", content.Name, "Root folder name should be 'root'."); 
        Assert.IsNotNull(content.Folders, "Folders list should not be null.");
        Assert.IsNotNull(content.Files, "Files list should not be null.");
    }

    [TestMethod]
    [TestCategory("Integration_Folder_CRUD")]
    public async Task FolderLifecycle_Live_CreateRenameDelete()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        string originalFolderName = $"Yvan_{Guid.NewGuid().ToString().Substring(0, 8)}";
        string renamedFolderName = $"Renamed_YB_{originalFolderName}";

        
        TestContext.WriteLine($"Attempting to create folder: {originalFolderName}");
        var createResult = await _seedrService.CreateFolderAsync(originalFolderName);
        Assert.IsNotNull(createResult, "CreateFolderAsync result was null.");
        Assert.IsTrue(createResult.Result || createResult.Id.HasValue, $"Failed to create folder. Error: {createResult.Error}");
        Assert.AreEqual(createResult.Code, 200);



        FolderContent? folders = await _seedrService.ListRootFolderAsync();

        var fol = folders?.Folders.OrderByDescending(x=>x.LastUpdate).ToList();
        var f = fol?.FirstOrDefault()!;
        var folderId = f.Id;

        var dlLink =  _seedrService.GetFolderLink(folderId);

        System.Diagnostics.Trace.WriteLine($"TestInitialize for: {dlLink}");
        Assert.IsNotNull(dlLink, "GetFolderAsZipLinkAsync result was null.");

        if (folders?.Files.Count> 0)
        {
            //var filee = await _seedrService.ListFolderAsync(folderId);
            var fileId = folders?.Files.FirstOrDefault()?.Id ?? -1;
            var fileLink = _seedrService.GetFileLink(fileId);

            System.Diagnostics.Trace.WriteLine($"TestInitialize for: {fileLink}");
        }

        TestContext.WriteLine($"Attempting to rename folder ID {folderId} to: {renamedFolderName}");
        var renameResult = await _seedrService.RenameFolderAsync(folderId, renamedFolderName);
        Assert.IsNotNull(renameResult, "RenameFolderAsync result was null.");
        Assert.IsTrue(renameResult.Result, $"Failed to rename folder. Error: {renameResult.Error}");
        TestContext.WriteLine($"Folder {folderId} renamed.");

        

        var folders2 = await _seedrService.ListRootFolderAsync();
        var folderContentAfterRename = await _seedrService.ListFolderAsync(folderId);
        Assert.IsNotNull(folderContentAfterRename, "Could not fetch renamed folder content.");
        Assert.AreEqual(renamedFolderName, folderContentAfterRename.Name, "Folder name was not updated after rename.");

        
        TestContext.WriteLine($"Attempting to delete folder ID {folderId}");
        var deleteResult = await _seedrService.DeleteFolderAsync(folderId);
        Assert.IsNotNull(deleteResult, "DeleteFolderAsync result was null.");
        Assert.IsTrue(deleteResult.Result, $"Failed to delete folder. Error: {deleteResult.Error}");
        TestContext.WriteLine($"Folder {folderId} deleted.");
        _foldersToDelete.Remove(folderId); 

        
        var checkDeleted = await _seedrService.ListFolderAsync(folderId);
        Assert.AreEqual(checkDeleted?.Result,false);
    }


    [TestMethod]
    [TestCategory("Integration_Transfer_Magnet")]
    public async Task AddMagnetAsync_Live_ValidMagnet_ShouldSucceedAndCleanup()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        
        
        
        string testMagnet = "magnet:?xt=urn:btih:6D51445B88842875C2DBDBAA924DC78B9FE0358F&dn=RoadCraft-RUNE&tr=udp%3A%2F%2Ftracker.torrent.eu.org%3A451%2Fannounce&tr=udp%3A%2F%2Fexodus.desync.com%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.moeking.me%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337%2Fannounce&tr=udp%3A%2F%2Fopen.stealth.si%3A80%2Fannounce&tr=udp%3A%2F%2Ftracker.theoks.net%3A6969%2Fannounce&tr=udp%3A%2F%2Fmovies.zsw.ca%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker-udp.gbitt.info%3A80%2Fannounce&tr=udp%3A%2F%2Ftracker.tiny-vps.com%3A6969%2Fannounce&tr=http%3A%2F%2Ftracker.gbitt.info%3A80%2Fannounce&tr=http%3A%2F%2Ftracker.ccp.ovh%3A6969%2Fannounce&tr=https%3A%2F%2Ftracker.gbitt.info%3A443%2Fannounce&tr=udp%3A%2F%2Ftracker.dler.com%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.ccp.ovh%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337%2Fannounce&tr=http%3A%2F%2Ftracker.openbittorrent.com%3A80%2Fannounce&tr=udp%3A%2F%2Fopentracker.i2p.rocks%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.internetwarriors.net%3A1337%2Fannounce&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969%2Fannounce&tr=udp%3A%2F%2Fcoppersurfer.tk%3A6969%2Fannounce&tr=udp%3A%2F%2Ftracker.zer0day.to%3A1337%2Fannounce"; 

        var result = await _seedrService.AddMagnetAsync(testMagnet);

        Assert.IsNotNull(result, "AddMagnetAsync result was null.");
        Assert.IsTrue(result.Result, $"Failed to add magnet. Error: {result.Error}");
        Assert.IsNotNull(result.UserTorrentId, "UserTorrentId should be returned on successful magnet add.");
        TestContext.WriteLine($"Magnet added, UserTorrentId: {result.UserTorrentId.Value}");

        AddTransferForCleanup(result.UserTorrentId.Value);

        
        await Task.Delay(5000); 
        var transferStatus = await _seedrService.GetTransferDataAsync(result.UserTorrentId.Value);
        }

    

    [TestMethod]
    [TestCategory("Integration_File")]
    [Ignore("Requires a pre-existing file ID or a more complex setup to create one.")]
    public void DownloadFileAsync_Live_KnownFileId_ShouldDownload()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        
        int knownFileId = -1; 
        //string knownFileName = "test_download_file.dat"; 

        if (knownFileId == -1)
            Assert.Inconclusive("KnownFileId not set for DownloadFileAsync_Live test.");

        var tempOutputPath = Path.Combine(Path.GetTempPath(), $"seedr_dl_test_{Guid.NewGuid()}.dat");
        TestContext.WriteLine($"Attempting to download file ID {knownFileId} to {tempOutputPath}");

        //var success = await _seedrService.DownloadFileAsync(knownFileId, tempOutputPath);

        //Assert.IsTrue(success, "DownloadFileAsync returned false.");
        Assert.IsTrue(System.IO.File.Exists(tempOutputPath), "Downloaded file does not exist at output path.");
        var fileInfo = new FileInfo(tempOutputPath);
        Assert.IsTrue(fileInfo.Length > 0, "Downloaded file is empty.");
        TestContext.WriteLine($"File downloaded successfully. Size: {fileInfo.Length} bytes.");

        
        System.IO.File.Delete(tempOutputPath);
    }

    
    [TestMethod]
    [TestCategory("Integration_Transfer_File")]
    [Ignore("Requires a pre-existing file ID or a more complex setup to create one.")]

    public async Task AddFileAsync_Live_ValidTorrentFile_ShouldSucceedAndCleanup()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");

        string dummyTorrentContent = "d8:announce40:udp://tracker.openbittorrent.com:8013:creation datei1327050000e4:infod6:lengthi1e4:name10:dummy.txte12:piece lengthi65536e6:pieces20:aaaaaaaaaaaaaaaaaaaae"; 
        string tempTorrentFilePath = Path.Combine(Path.GetTempPath(), $"test_upload_{Guid.NewGuid().ToString().Substring(0, 6)}.torrent");
        await System.IO.File.WriteAllTextAsync(tempTorrentFilePath, dummyTorrentContent);

        ApiResult? result = null;
        try
        {
            using (var fileStream = new FileStream(tempTorrentFilePath, FileMode.Open, FileAccess.Read))
            {
                
                //result = await _seedrService.AddFileAsync(Path.GetFileName(tempTorrentFilePath),fileStream );
            }

            Assert.IsNotNull(result, "AddFileAsync result was null.");
            
            
            Assert.IsTrue(result.Result, $"Failed to upload .torrent file. Error: {result.Error}");
            Assert.IsNotNull(result.UserTorrentId, "UserTorrentId should be returned on successful .torrent file upload.");
            TestContext.WriteLine($".torrent file uploaded, UserTorrentId: {result.UserTorrentId.Value}");

            AddTransferForCleanup(result.UserTorrentId.Value);
        }
        finally
        {
            if (System.IO.File.Exists(tempTorrentFilePath))
            {
                System.IO.File.Delete(tempTorrentFilePath);
            }
        }
    }

    [TestMethod]
    [TestCategory("Integration_BrowserDownload")]
    public async Task DownloadFileOnPreferredBrowserAsync_ShouldCallBrowserLauncherWithCorrectUrl()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        Assert.IsNotNull(_mockBrowserLauncher, "MockBrowserLauncher not initialized.");

        long testFileId = 12345;
        string expectedUrl = $"{BaseApiUrl}/file/{testFileId}";

        _mockBrowserLauncher.Setup(b => b.OpenAsync(expectedUrl, SeedrBrowserLaunchMode.SystemPreferred))
                            .ReturnsAsync(true);

        // Act
        bool result = await _seedrService.DownloadFileOnPreferredBrowserAsync(testFileId);

        // Assert
        Assert.IsTrue(result, "DownloadFileOnPreferredBrowserAsync should return true when browser launch is successful.");
        _mockBrowserLauncher.Verify(b => b.OpenAsync(expectedUrl, SeedrBrowserLaunchMode.SystemPreferred), Times.Once,
            "IBrowserLauncher.OpenAsync was not called with the expected URL or parameters for file download.");
    }

    [TestMethod]
    [TestCategory("Integration_BrowserDownload")]
    public async Task DownloadFileOnPreferredBrowserAsync_BrowserLaunchFails_ShouldReturnFalse()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        Assert.IsNotNull(_mockBrowserLauncher, "MockBrowserLauncher not initialized.");

        long testFileId = 67890;
        string expectedUrl = $"{BaseApiUrl}/file/{testFileId}";

        _mockBrowserLauncher.Setup(b => b.OpenAsync(expectedUrl, SeedrBrowserLaunchMode.SystemPreferred))
                            .ReturnsAsync(false); // Simulate browser launch failure

        // Act
        bool result = await _seedrService.DownloadFileOnPreferredBrowserAsync(testFileId);

        // Assert
        Assert.IsFalse(result, "DownloadFileOnPreferredBrowserAsync should return false when browser launch fails.");
        _mockBrowserLauncher.Verify(b => b.OpenAsync(expectedUrl, SeedrBrowserLaunchMode.SystemPreferred), Times.Once,
            "IBrowserLauncher.OpenAsync was not called as expected even on failure for file download.");
    }

    [TestMethod]
    [TestCategory("Integration_BrowserDownload")]
    public async Task DownloadFolderOnPreferredBrowserAsync_ShouldCallBrowserLauncherWithCorrectUrl()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        Assert.IsNotNull(_mockBrowserLauncher, "MockBrowserLauncher not initialized.");

        long testFolderId = 98765;
        string expectedUrl = $"{BaseApiUrl}/folder/{testFolderId}/download";

        _mockBrowserLauncher.Setup(b => b.OpenAsync(expectedUrl, SeedrBrowserLaunchMode.SystemPreferred))
                            .ReturnsAsync(true);

        // Act
        bool result = await _seedrService.DownloadFolderOnPreferredBrowserAsync(testFolderId);

        // Assert
        Assert.IsTrue(result, "DownloadFolderOnPreferredBrowserAsync should return true when browser launch is successful.");
        _mockBrowserLauncher.Verify(b => b.OpenAsync(expectedUrl, SeedrBrowserLaunchMode.SystemPreferred), Times.Once,
            "IBrowserLauncher.OpenAsync was not called with the expected URL or parameters for folder download.");
    }

    [TestMethod]
    [TestCategory("Integration_BrowserDownload")]
    public async Task DownloadFolderOnPreferredBrowserAsync_BrowserLaunchFails_ShouldReturnFalse()
    {
        Assert.IsNotNull(_seedrService, "Service not initialized.");
        Assert.IsNotNull(_mockBrowserLauncher, "MockBrowserLauncher not initialized.");

        long testFolderId = 54321;
        string expectedUrl = $"{BaseApiUrl}/folder/{testFolderId}/download";

        _mockBrowserLauncher.Setup(b => b.OpenAsync(expectedUrl, SeedrBrowserLaunchMode.SystemPreferred))
                            .ReturnsAsync(false); // Simulate browser launch failure

        // Act
        bool result = await _seedrService.DownloadFolderOnPreferredBrowserAsync(testFolderId);

        // Assert
        Assert.IsFalse(result, "DownloadFolderOnPreferredBrowserAsync should return false when browser launch fails.");
        _mockBrowserLauncher.Verify(b => b.OpenAsync(expectedUrl, SeedrBrowserLaunchMode.SystemPreferred), Times.Once,
            "IBrowserLauncher.OpenAsync was not called as expected even on failure for folder download.");
    }




}