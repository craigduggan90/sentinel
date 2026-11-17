using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Data;

/// <summary>Constants used throughout the project.</summary>
[ExcludeFromCodeCoverage]
internal static class Constants
{
    /// <summary>The default number of items to retrieve per page of data.</summary>
    public const int DefaultPageSize = 25;

    /// <summary>The configuration key of the read-only connection string.</summary>
    public const string ReaderConnectionStringName = "Reader";

    /// <summary>The configuration key of the read-write connection string.</summary>
    public const string WriterConnectionStringName = "Writer";
}