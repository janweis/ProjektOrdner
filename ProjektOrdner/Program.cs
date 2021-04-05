using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainProgram program = new MainProgram();
            Task programTask = program.StartProgramAsync();

            Application.Run();
        }
    }
}
