using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ridwan.Service.Util
{
    public static class ResponseCodes
    {
        public const string Success = "10200";
        public const string InvalidRequest = "10400";
        public const string UnexpectedError = "10500";
        public const string Conflict = "10409";
        public const string Forbidden = "10403";
        public const string NotFound = "10404";
        public const string Unauthorized = "10401";
        public const string UpgradeRequired = "10426";
        public const string UnavailableForLegalReasons = "10451";
    }
}
