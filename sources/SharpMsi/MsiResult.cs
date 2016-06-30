using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMsi
{
    public enum MsiResult : int
    {
        Success = 0,
        InvalidHandle = 6,
        InvalidParameter = 87,
        OpenFailed = 110,
        MoreData = 234,
        NoMoreItems = 259,
        InstallUserExit = 1602,
        InstallFailure = 1603,
        UnknownProduct = 1605,
        UnknownProperty = 1608,
        InvalidHandleState = 1609,
        BadConfiguration = 1610,
        InstallSourceAbsent = 1612,
        BadQuerySyntax = 1615,
        InstallInProgress = 1618,
        FunctionFailed = 1627,
        InvalidTable = 1628,
        CreateFailed = 1631
    }
}
