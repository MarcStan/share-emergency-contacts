using System.IO;
using System.Text;
using System.Xml.Linq;

namespace VersionPatcher
{
    public static class XDocumentExtensions
    {

        public static void SaveAsUtf8(this XDocument doc, string path)
        {
            // .Net core uses UTF-16 by default and that doesn't work well with either of the files we need to save
            using (var file = File.OpenWrite(path))
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                // if we don't set it to 0 before and we write less content than was in the file before
                // we end up with our content + whatever we didn't overwrite
                // this is due to shitty OpenWrite not allowing any specifications (Clear|Overwrite|..)
                // manually resetting it to 0 first fixes it
                file.SetLength(0);
                doc.Save(writer, SaveOptions.None);
            }
        }
    }
}