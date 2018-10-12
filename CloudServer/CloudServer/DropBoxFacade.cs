using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudServer
{
    class DropBoxFacade
    {
        private String token;
        private Dictionary<String, DropBoxFolder> folders;
        public List<DropBoxFolder> Folders
        {
            get
            {
                return folders.Values.ToList();
            }
        }

        public DropBoxFacade(String token)
        {
            this.token = token;
            folders = new Dictionary<string, DropBoxFolder>();
        }

        public Task LoadAll()
        {
            return Task.Run(async () =>
            {
                using (DropboxClient client = new DropboxClient(token))
                {
                    await LoadFor(String.Empty, client);
                }
            });
        }

        public async Task LoadFor(String folder, DropboxClient client)
        {
            if (client == null) throw new NullReferenceException(nameof(client));

            var elements = await client.Files.ListFolderAsync(folder);

            if (!folders.ContainsKey(folder))
            {

                try
                {

                    if (folder != "")
                    {
                        FolderMetadata folderMetadata = (await client.Files.GetMetadataAsync(new GetMetadataArg(folder))).AsFolder;
                        folders.Add(folder, new DropBoxFolder(folderMetadata));
                    }
                    else
                    {
                        folders.Add(folder, new DropBoxFolder(folder));
                    }


                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    return;
                }
            }

            foreach (var element in elements.Entries)
            {
                if (element.IsFile)
                {
                    folders[folder].Elements.Add(new DropBoxFile(element.AsFile));
                }
                else
                {
                    await LoadFor(element.PathDisplay, client);
                    folders[folder].Elements.Add(folders[element.PathDisplay]);
                }
            }
        }

     
    }
}
