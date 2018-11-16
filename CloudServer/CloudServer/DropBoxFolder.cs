using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer
{
  public class DropBoxFolder:DropBoxElement
    {
        public override Metadata Metadata { get { return FolderMetadata; }}
        public FolderMetadata FolderMetadata { get; private set; }
        public List<DropBoxElement> Elements { get; }

        public DropBoxFolder(String name)
        {
            Name = name;
            FullName = FullName;
            Elements = new List<DropBoxElement>();
        }

        public DropBoxFolder(FolderMetadata metadata)
        {
            FolderMetadata = metadata;
            Name = metadata.Name;
            FullName = metadata.PathDisplay;
            Elements = new List<DropBoxElement>();
        }
    }
}
