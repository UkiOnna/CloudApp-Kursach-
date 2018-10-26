namespace CloudServer
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class DropBoxContext : DbContext
    {
        public DropBoxContext()
            : base("name=DropBoxContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FileInfo> FilesInfo { get; set; }
    }

}