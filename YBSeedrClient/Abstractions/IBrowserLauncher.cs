using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YBSeedrClient.Abstractions;
/// <summary>
/// The seedrs browser launch modes.
/// </summary>
public enum SeedrBrowserLaunchMode
{
    SystemPreferred = 0,
    Private = 1
}

/// <summary>
/// The browser launcher interface.
/// </summary>
public interface IBrowserLauncher
{
    /// <summary>
    /// Open and return a task of type bool asynchronously.
    /// </summary>
    /// <param name="uri">The uri.</param>
    /// <param name="launchMode">The launch mode.</param>
    /// <returns><![CDATA[Task<bool>]]></returns>
    Task<bool> OpenAsync(string uri, SeedrBrowserLaunchMode launchMode);
}