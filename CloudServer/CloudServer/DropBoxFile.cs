using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer
{
   public class DropBoxFile:DropBoxElement
    {
        public override Metadata Metadata { get { return FileMetadata; } }
        public FileMetadata FileMetadata { get; private set; }

        public DropBoxFile(FileMetadata metadata)
        {
            FileMetadata = metadata;
            Name = metadata.Name;
            FullName = metadata.PathDisplay;
        }
    }
}
