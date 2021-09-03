using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace StashCache
{
    /// <summary>
    /// A key that uniquely identifies a local in-memory cache value.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly struct CacheKey : IEquatable<CacheKey>
    {
        private const char Delimiter = ':';
        private readonly Type _ownerType;
        private readonly string _memberInfo;
        private readonly IEnumerable<string> _segments;
        private readonly string _segmentString;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKey"/>.
        /// </summary>
        /// <param name="ownerType">The class from which the cache key is generated.</param>
        /// <param name="memberInfo">The name of the method in <paramref name="ownerType"/> that needs caching.
        ///     Method name is determined from <see cref="CallerMemberNameAttribute"/>.
        /// </param>
        /// <param name="segments">The optional collection of segments that will be appended to the key.</param>
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
                && string.Compare(_segmentString, other._segmentString, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(_ownerType.FullName);
            hash.Add(_memberInfo);
            hash.Add(_segmentString?.ToLower());

            return hash.ToHashCode();
        }

        public override string ToString()
        {
            return $"{_ownerType.FullName}.{_memberInfo}{_segmentString ?? string.Empty}";
        }
    }
}
