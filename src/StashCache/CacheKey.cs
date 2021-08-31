using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StashCache
{
    public readonly struct CacheKey : IEquatable<CacheKey>
    {
        private const char Delimiter = ':';
        private readonly Type _ownerType;
        private readonly string _memberInfo;
        private readonly IEnumerable<string> _segments;
        private readonly string _segmentString;

        public CacheKey(Type ownerType, string memberInfo, IEnumerable<string> segments)
        {
            _ownerType = ownerType.NotNull(nameof(ownerType));
            _memberInfo = memberInfo.NotEmpty(nameof(memberInfo));
            _segments = segments;
            _segmentString = null;

            if (segments != null && segments.Any())
            {
                var sb = new StringBuilder();

                foreach (var segment in segments.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    sb.Append($"{Delimiter}{segment}");
                }

                _segmentString = sb.ToString();
            }
        }

        public override bool Equals(object obj) => obj is CacheKey other && Equals(other);

        public bool Equals(CacheKey other)
        {
            return _ownerType.FullName == other._ownerType.FullName
                && _memberInfo == other._memberInfo
                && _segmentString == other._segmentString;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
        {
            return $"{_ownerType.FullName}.{_memberInfo}{_segmentString ?? string.Empty}";
        }
    }
}
