namespace Report_Update_Automation
{
    public record SystemEntityVersion
    {
        public string Name { get; }
        public int Version { get; }

        public SystemEntityVersion(string name, int version)
        {
            Name = name;
            Version = version;
        }
    }
}