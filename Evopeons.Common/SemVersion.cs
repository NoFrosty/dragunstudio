using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Evopeons.Common
{
    public class SemVersion
    {
        static readonly Regex parseEx =
            new Regex(@"^(?<major>\d+)" +
                @"(\.(?<minor>\d+))?" +
                @"(\.(?<patch>\d+))?" +
                @"(\-(?<pre>[0-9A-Za-z\-\.]+))?" +
                @"(\+(?<build>[0-9A-Za-z\-\.]+))?$",
                RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture);


        public SemVersion() : this(0)
        {
        }

        public SemVersion(int major, int minor = 0, int patch = 0, string prerelease = "", string build = "")
        {
            Major = major;
            Minor = minor;
            Patch = patch;

            Prerelease = prerelease ?? "";
            Build = build ?? "";
        }

        public static SemVersion Parse(string version)
        {
            var match = parseEx.Match(version);
            if (!match.Success)
                throw new ArgumentException("Invalid version.", "version");

            var major = int.Parse(match.Groups["major"].Value, CultureInfo.InvariantCulture);

            var minorMatch = match.Groups["minor"];
            int minor = 0;
            if (minorMatch.Success)
            {
                minor = int.Parse(minorMatch.Value, CultureInfo.InvariantCulture);
            }

            var patchMatch = match.Groups["patch"];
            int patch = 0;
            if (patchMatch.Success)
            {
                patch = int.Parse(patchMatch.Value, CultureInfo.InvariantCulture);
            }

            var prerelease = match.Groups["pre"].Value;
            var build = match.Groups["build"].Value;

            return new SemVersion(major, minor, patch, prerelease, build);
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string Prerelease { get; set; }
        public string Build { get; set; }


        public override string ToString()
        {
            var version = "" + Major + "." + Minor + "." + Patch;
            if (!String.IsNullOrEmpty(Prerelease))
                version += "-" + Prerelease;
            if (!String.IsNullOrEmpty(Build))
                version += "+" + Build;
            return version;
        }


        public override bool Equals(object obj)
        {
            if (obj is SemVersion other)
            {
                return
                    Major == other.Major &&
                    Minor == other.Minor &&
                    Patch == other.Patch &&
                    string.Equals(Prerelease, other.Prerelease, StringComparison.Ordinal) &&
                    string.Equals(Build, other.Build, StringComparison.Ordinal);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.Major.GetHashCode();
                result = result * 31 + Minor.GetHashCode();
                result = result * 31 + Patch.GetHashCode();
                result = result * 31 + Prerelease.GetHashCode();
                result = result * 31 + Build.GetHashCode();
                return result;
            }
        }
    }
}
