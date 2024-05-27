using System;
namespace xdm_model.Models
{
    public class sys_file
    {
        public sys_file()
        {
        }
        public int id { get; set; }
        public string? no { get; set; }
        public string? fullpath { get; set; }
        public string? path { get; set; }
        public string? filename { get; set; }
        public string? filenewname { get; set; }
        public string? fileext { get; set; }
        public byte[] content { get; set; }
    }
}

