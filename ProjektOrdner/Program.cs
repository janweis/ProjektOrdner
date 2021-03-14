using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektOrdner
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainProgram program = new MainProgram();
            Task programTask = program.StartAsync();

            Application.Run();
        }
    }
}
