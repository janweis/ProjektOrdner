using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ProjektOrdner.App
{
    public class LogProcessor
    {
        // // // // // // // // // // // // // // // // // // // // //
        // Variables
        // 

        private string FilePath { get; set; }
        private StreamWriter Writer { get; set; }


        // // // // // // // // // // // // // // // // // // // // //
        // Constructors
        // 

        public LogProcessor(string filePath)
        {
            FilePath = filePath;

            if (null == Writer)
                OpenFile();
        }


        // // // // // // // // // // // // // // // // // // // // //
        // Public Functions
        // 

        /// <summary>
        /// 
        /// Initalisiert das Log
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
        /// Schreibt einen Eintrag in das Log
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
        /// Schließt das Log
        /// 
        /// </summary>
        public void Close()
        {
            Writer.Close();
        }


        /// <summary>
        /// 
        /// Öffnet das Log mit dem jeweils verbunden Programm
        /// 
        /// </summary>
        public void ShowLog()
        {
            Process process = new Process();
            process.StartInfo.FileName = FilePath;
            process.Start();
        }

    }
}
