using System;
namespace xdm_model.DTO
{


    public class MenuItem
    {
        public int menu_id { get; set; }
        public int parent_id { get; set; }
        public string menu_name { get; set; }
        public string path { get; set; }
        public string perms { get; set; }
        public string icon { get; set; }
        public int order_num { get; set; }
        public List<MenuItem> Children { get; set; } = new List<MenuItem>();
    }

    public class MenuMeta
    {
        public string title { get; set; }
        public string icon { get; set; }
        public bool noCache { get; set; }
        public string link { get; set; }
    }

    public class TreeMenuItem
    {
        public string name { get; set; }
        public string path { get; set; }
        public bool hidden { get; set; }
        public string redirect { get; set; }
        public string component { get; set; }
        public bool alwaysShow { get; set; }
        public MenuMeta meta { get; set; }
        public List<TreeMenuItem> Children { get; set; } = new List<TreeMenuItem>();
    }
}

