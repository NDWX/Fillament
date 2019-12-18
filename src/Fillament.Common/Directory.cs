
namespace Fillament
{
    public class Directory
    {
        public string Path { get; set; }
        
        public FilePermissions FilePermissions { get; set; }
        
        public FolderPermissions FolderPermissions { get; set; }
        
        public bool AutoCreate { get; set; }
    }
}