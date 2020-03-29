namespace Heroes.StormReplayParser
{
    internal struct ReplayAttribute
    {
        public ReplayAttributeEventType AttributeType { get; set; }
        public int PlayerId { get; set; }
        public string Value { get; set; }

        public override string? ToString()
        {
            return $"{nameof(PlayerId)}: {PlayerId}, {nameof(AttributeType)}: {AttributeType}, {nameof(Value)}: {Value}";
        }
    }
}
