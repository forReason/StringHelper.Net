using System.Text;

namespace StringHelper.Net
{
    /// <summary>
    /// Class containing methods for reading and converting encoding in text files. 
    /// Includes a method for reading a specified number of lines from a file and 
    /// a method for converting a string representation of an encoding to its corresponding System.Text.Encoding object.
    /// </summary>
    public class TextFileFunctions
    {
        /// <summary>
        /// This function loads and returns the first x lines from a file
        /// </summary>
        /// <param name="path">The path to the file to read</param>
        /// <param name="amountOfLines">the number of lines which should be read</param>
        /// <param name="encoding">optional value, which specifies the format in which the text file is saved</param>
        /// <returns></returns>
        public List<string> GetFirstXLines(string path, int amountOfLines, Encoding encoding = null, int skip = 0)
        {
            // if no encoding is set, try the default encoding (File Format)
            if (encoding == null) encoding = Encoding.Default;
            // create list which will be filled with the lines and returned later
            List<string> lines = new List<string>();
            // wrap streamreader around so it gets closed+disposed properly later
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                // skip x lines
                for (int i = 0; i < skip; i++)
                {
                    // read the next line
                    string line = reader.ReadLine();
                    // if the line is null, we are at the end of file, break out of the loop
                    if (line == null) break;
                }
                // loop x times to get the first x lines
                for (int i = 0; i < amountOfLines; i++)
                {
                    // read the next line
                    string line = reader.ReadLine();
                    // if the line is null, we are at the end of file, break out of the loop
                    if (line == null) break;
                    // if the line was not null, add it to the lines list
                    lines.Add(line);
                }
            }
            // return the lines
            return lines;
        }
        /// <summary>
        /// Converts a string representation of an encoding to its corresponding System.Text.Encoding object.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Encoding ConvertEncoding(string input)
        {
            Encoding selectedEncoding = Encoding.Default;
            if (input == "Default") selectedEncoding = Encoding.Default;
            if (input == "UTF8") selectedEncoding = Encoding.UTF8;
            if (input == "ASCII") selectedEncoding = Encoding.ASCII;
            if (input == "Unicode") selectedEncoding = Encoding.Unicode;
            if (input == "UTF7") selectedEncoding = Encoding.UTF7;
            if (input == "UTF32") selectedEncoding = Encoding.UTF32;
            return selectedEncoding;
        }
    }
}
