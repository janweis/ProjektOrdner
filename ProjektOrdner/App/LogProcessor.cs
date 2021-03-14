using System.IO;
using System.Text;

namespace ProjektOrdnerLib.Processors
{
    public class LogProcessor
    {
        private string FilePath { get; set; }
        private StreamWriter Writer { get; set; }


        public LogProcessor(string filePath)
        {
            FilePath = filePath;

            if (null == Writer)
                OpenFile();
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        private void OpenFile()
        {
            Writer = new StreamWriter(FilePath, true, Encoding.UTF8)
            {
                AutoFlush = true
            };
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public void Write(string message)
        {
            if (null == Writer)
                OpenFile();

            Writer.WriteLine(message);
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        public void Close()
        {
            Writer.Close();
        }
    }
}
