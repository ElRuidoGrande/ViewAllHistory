// Guids.cs
// MUST match guids.h
using System;

namespace TFSExtensions.ViewAllHistory
{
    static class GuidList
    {
        public const string guidViewAllHistoryPkgString = "512e2dd1-414e-412a-a2e8-b21ab4fb69f2";
        public const string guidViewAllHistoryCmdSet_SolutionExplorerString = "26cd0820-4db6-46a9-b26a-05f08fdc84f9";

        public const string guidViewAllHistoryCmdSetString = "25cd0820-4db6-46a9-b26a-05f08fdc84f9";

        public static readonly Guid guidViewAllHistoryCmdSet = new Guid(guidViewAllHistoryCmdSetString);

        public static readonly Guid guidViewAllHistoryCmdSet_SolutionExplorer = new Guid(guidViewAllHistoryCmdSet_SolutionExplorerString);
    };
}