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
        private readonly string _segmentsString;

        public CacheKey(Type ownerType, string memberInfo, IEnumerable<string> segments)
        {
            _ownerType = ownerType.NotNull(nameof(ownerType));
            _memberInfo = memberInfo.NotEmpty(nameof(memberInfo));
            _segments = segments;
            _segmentsString = null;

            if (_segments != null && _segments.Any())
            {
                var sb = new StringBuilder();
                foreach (var item in _segments)
                {
                    sb.Append($"{Delimiter}{item}");
                }

                _segmentsString = sb.ToString();
            }
        }

        public override bool Equals(object obj) => obj is CacheKey other && Equals(other);

        public bool Equals(CacheKey other)
        {
            return _ownerType.FullName == other._ownerType.FullName
                && _memberInfo == other._memberInfo
                && _segmentsString == other._segmentsString;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{_ownerType.FullName}.{_memberInfo}{(_segmentsString ?? string.Empty)}";
        }
    }
}
