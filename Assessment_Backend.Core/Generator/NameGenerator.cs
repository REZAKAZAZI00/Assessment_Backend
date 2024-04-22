using System;
using System.Collections.Generic;

namespace Assessment_Backend.Core.Generator
{
    public class NameGenerator
    {
        public static string GenerateName(int len=30)
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, len);
        }
        public static string GenerateShareLink(int len=8)
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, len);
        }
        public static string GenerateUniqCode(int len = 30)
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, len);
        }

        public static string GenerateNameForImage(int len=20)
        {
            return Guid.NewGuid().ToString().Replace("-", "")[..len];
        }
    }
}
