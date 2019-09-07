using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.IO;

namespace blockget
{
    
    //https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=netframework-4.8
    public class myWatcher
    {
        Filesys fs;
        FileSystemWatcher watcher;

        internal myWatcher(Filesys filesys) {
            fs = filesys;
            watcher = new FileSystemWatcher();
            initWatcher();
        }
        //[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        //private 
        //public static void Run()
        void initWatcher()
        {
            // Create a new FileSystemWatcher and set its properties.
            //using (FileSystemWatcher watcher = new FileSystemWatcher())
            //{
                //Will have to recover this from installation params
                watcher.Path = Globals.BLOCKGET_FILE_FOLDER;

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;
                // Watch all apart from .aes and .tmp => Filter only include
                //watcher.Filter = !"*.aes";

                // Add event handlers.
                watcher.Changed += OnChanged; 
                watcher.Created += OnCreated;
                watcher.Deleted += OnDeleted;
                watcher.Renamed += OnRenamed; 
                // Begin watching.
                watcher.EnableRaisingEvents = true;
            //}
        }

        // Define the event handlers.
        //private static void OnChanged(object source, FileSystemEventArgs e)
        private void OnChanged(object source, FileSystemEventArgs e) 
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            //Filesys.UpdateEvent(e.FullPath);
            //Filesys fs = new Filesys();
            fs.UpdateEvent(e.FullPath);
            //Form1.UpdateEvent(e.FullPath);
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
            fs.UpdateEvent(e.FullPath);
        }


        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            fs.DeleteEvent(e.FullPath);
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            fs.CreateEvent(e.FullPath);
        }

    }
    
}
