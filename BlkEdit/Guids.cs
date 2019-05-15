// Guids.cs
// MUST match guids.h
using System;

namespace MR.BlkEdit
{
    static class GuidList
    {
        public const string guidBlkEditPkgString = "609bccdc-2740-4d3a-872f-808465c96700";
        public const string guidBlkEditCmdSetString = "a21bf088-6902-4d40-a23b-ef38a4ee07d8";
        public const string guidlogResultCmdSetString = "cb11d021-d81d-438e-a5bf-2c704f57420e";

        public static readonly Guid guidBlkEditCmdSet = new Guid(guidBlkEditCmdSetString);
    };
}