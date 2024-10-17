using System.Diagnostics;

namespace testISO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string isoPath = @"C:\Users\CaYatur\Desktop\CaYaOS.iso";

            // Boot Manager üzerinden ISO'yu ekleme komutlarý
            string command = $"/c bcdedit /create {Guid.NewGuid()} /d \"ISO Boot\" /application osloader";
            string addBootCommand = $"/c bcdedit /set {Guid.NewGuid()} device partition=C:";
            string setPathCommand = $"/c bcdedit /set {Guid.NewGuid()} path \"{isoPath}\"";
            string setBootSequence = $"/c bcdedit /displayorder {Guid.NewGuid()} /addlast";

            RunCommand(command);
            RunCommand(addBootCommand);
            RunCommand(setPathCommand);
            RunCommand(setBootSequence);
        }

        static void RunCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine(output);
            }
        }

    }
}
