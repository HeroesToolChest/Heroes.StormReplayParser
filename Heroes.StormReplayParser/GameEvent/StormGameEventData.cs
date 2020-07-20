using System;
using System.Collections.Generic;
using System.Linq;

namespace Heroes.StormReplayParser.GameEvent
{
    public class StormGameEventData
    {
        private readonly bool? _boolean = null;
        private readonly string? _blob = null;
        private readonly int? _int32 = null;
        private readonly uint? _uInt32 = null;
        private readonly ulong? _uInt64 = null;
        private readonly StormGameEventData[]? _array = null;
        private readonly bool[]? _bitArray = null;

        public StormGameEventData(bool value)
        {
            DataType = StormGameEventDataType.Bool;
            _boolean = value;
        }

        public StormGameEventData(int value)
        {
            DataType = StormGameEventDataType.Int32;
            _int32 = value;
        }

        public StormGameEventData(uint value)
        {
            DataType = StormGameEventDataType.UInt32;
            _uInt32 = value;
        }

        public StormGameEventData(ulong value)
        {
            DataType = StormGameEventDataType.UInt64;
            _uInt64 = value;
        }

        public StormGameEventData(string? value)
        {
            DataType = StormGameEventDataType.Blob;
            _blob = value;
        }

        public StormGameEventData(Dictionary<int, StormGameEventData>? structureByIndex)
        {
            DataType = StormGameEventDataType.Structure;
            StructureByIndex = structureByIndex;
        }

        public StormGameEventData(StormGameEventData[]? array)
        {
            DataType = StormGameEventDataType.Array;
            _array = array;
        }

        public StormGameEventData(bool[] bitArray)
        {
            DataType = StormGameEventDataType.BitArray;
            _bitArray = bitArray;
        }

        public StormGameEventDataType DataType { get; }

        public Dictionary<int, StormGameEventData>? StructureByIndex { get; } = null;

        public bool? GetBoolean()
        {
            if (DataType != StormGameEventDataType.Bool)
                throw new InvalidOperationException($"Invalid call, current data type is {DataType}");

            return _boolean;
        }

        public int? GetInt32()
        {
            if (DataType != StormGameEventDataType.Int32)
                throw new InvalidOperationException($"Invalid call, current data type is {DataType}");

            return _int32;
        }

        public uint? GetUInt32()
        {
            if (DataType != StormGameEventDataType.UInt32)
                throw new InvalidOperationException($"Invalid call, current data type is {DataType}");

            return _uInt32;
        }

        public ulong? GetUInt64()
        {
            if (DataType != StormGameEventDataType.UInt64)
                throw new InvalidOperationException($"Invalid call, current data type is {DataType}");

            return _uInt64;
        }

        public string? GetString()
        {
            if (DataType != StormGameEventDataType.Blob)
                throw new InvalidOperationException($"Invalid call, current data type is {DataType}");

            return _blob;
        }

        public StormGameEventData[]? GetArrary()
        {
            if (DataType != StormGameEventDataType.Array)
                throw new InvalidOperationException($"Invalid call, current data type is {DataType}");

            return _array;
        }

        public bool[] GetBitArray()
        {
            if (DataType != StormGameEventDataType.BitArray)
                throw new InvalidOperationException($"Invalid call, current data type is {DataType}");

            return _bitArray;
        }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return DataType switch
            {
                StormGameEventDataType.Bool => _boolean?.ToString(),
                StormGameEventDataType.Int32 => _int32?.ToString(),
                StormGameEventDataType.UInt32 => _uInt32?.ToString(),
                StormGameEventDataType.UInt64 => _uInt64?.ToString(),
                StormGameEventDataType.Blob => _blob != null ? @$"""{_blob}""" : null,
                StormGameEventDataType.Array => _array != null ? $"[{string.Join(", ", _array.Select(i => i?.ToString()))}]" : null,
                StormGameEventDataType.Structure => StructureByIndex != null ? $"{{{string.Join(", ", StructureByIndex.Values.Select(i => i?.ToString()))}}}" : null,
                StormGameEventDataType.BitArray => _bitArray != null ? $"[{string.Join(", ", _bitArray.Select(i => i.ToString()))}]" : null,

                _ => string.Empty,
            };
        }
    }
}
