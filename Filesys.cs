using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockget
{
    class Filesys
    {   
        /// <summary>
        /// React to the watcher event - file deleted
        /// </summary>
        /// <param name="filepath"></param>
        public static void DeleteEvent(string filepath) {
            Console.WriteLine("We will have to remove {0} from the File System", filepath);
        }
        /// <summary>
        /// React to the watcher event - file created
        /// </summary>
        /// <param name="filepath"></param>
        public static void CreateEvent(string filepath) {
            Console.WriteLine("We will have to add {0} on the File System", filepath);
        }
    }
}
