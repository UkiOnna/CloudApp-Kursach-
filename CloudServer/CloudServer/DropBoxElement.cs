using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer
{
    public abstract class DropBoxElement
    {
        public String Name { get; protected set; }
        public String FullName { get; protected set; }
        public abstract Metadata Metadata { get; }

        public Boolean IsFile => Metadata.IsFile;
        public Boolean IsFolder => Metadata.IsFolder;
    }
}
