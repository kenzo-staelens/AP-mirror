using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace Datalaag {
    public class FileManager {

        public static string Load(string filename) {
            string text;
            using (StreamReader inputFile = new StreamReader(filename)) {
                text = inputFile.ReadToEnd();
            }
            return text;
        }
    }
}