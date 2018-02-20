using System;

namespace NugetAnalyzer.Core.Models
{
    public struct Version
    {
        public Version(short[] values, string releaseBeta)
        {
            ReleaseBeta = releaseBeta;
            Major = values[0];
            Minor = values[1];
            Patch = values.Length >= 3 ? values[2] : default;
            Build = values.Length == 4 ? values[3] : default;
        }


        public short Major { get; }
        public short Minor { get; }
        public short Patch { get; }
        public short? Build { get; }
        public string ReleaseBeta { get; }

        public override string ToString() => $"{Major}.{Minor}.{Patch}.{Build}";

        public static implicit operator Version(string version)
        {
            var strings = version.Split('-');
            var releaseBeta = strings.Length == 2 ? strings[1] : "";
            return new Version(Array.ConvertAll(strings[0].Split('.'), short.Parse), releaseBeta);
        }

        public bool Equals(Version other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch && Build == other.Build;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Version && Equals((Version) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Major.GetHashCode();
                hashCode = (hashCode * 397) ^ Minor.GetHashCode();
                hashCode = (hashCode * 397) ^ Patch.GetHashCode();
                hashCode = (hashCode * 397) ^ Build.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator >(Version versionA, Version versionB)
        {
            return long.Parse($"{versionA.Major}{versionA.Minor}{versionA.Patch}{versionA.Build}") >
                   long.Parse($"{versionB.Major}{versionB.Minor}{versionB.Patch}{versionB.Build}");
        }

        public static bool operator <(Version versionA, Version versionB)
        {
            return long.Parse($"{versionA.Major}{versionA.Minor}{versionA.Patch}{versionA.Build}") <
                   long.Parse($"{versionB.Major}{versionB.Minor}{versionB.Patch}{versionB.Build}");
        }
    }
}