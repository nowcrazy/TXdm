using System;
namespace xdm_model.DTO
{
    public class MenuItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Hidden { get; set; }
        public string Redirect { get; set; }
        public string Component { get; set; }
        public bool AlwaysShow { get; set; }
        public Meta Meta { get; set; }
        public List<Child> Children { get; set; }
    }

    public class Meta
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool NoCache { get; set; }
        public object Link { get; set; }
    }

    public class Child
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool Hidden { get; set; }
        public string Component { get; set; }
        public Meta Meta { get; set; }
        public string Redirect { get; set; }
        public bool AlwaysShow { get; set; }
        public List<Child> Children { get; set; }
    }
}

