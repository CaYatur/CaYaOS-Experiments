using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using System;
using System.Linq;
using System.Threading;
using Sys = Cosmos.System;
using static CaYaOS.Kernel;
using System.Collections.Generic;

namespace CaYaOS
{
    public class Kernel : Sys.Kernel
    {


        static string logo = @"
   _____   __     __    ____   _____ 
  / ____|  \ \   / /   / __ \ / ____|
 | |     __ \ \_/ /_ _| |  | | (___  
 | |    / _ \   / _ | |  | |\___ \ 
 | |___| (_| || | (_| | |__| |____) |
  \_____\__,_||_|\__,_|\____/|_____/ 
";

        private static void CrashScreen(string errorMessage)
        {
            // Ekranı temizle ve renkleri ayarla
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;

            // Logoyu yazdır
            Console.WriteLine(new string('=', 40));
            Console.WriteLine(logo);
            Console.WriteLine(new string('=', 40));

            // Hata mesajı başlığı
            Console.WriteLine("\n     === CRITICAL ERROR DETECTED ===\n");

            // Hata mesajını kırmızı ile vurgula
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Error Details: {errorMessage}");
            Console.ForegroundColor = ConsoleColor.White;

            // Sistem kapanış bildirisi ve ilerleme çubuğu
            Console.WriteLine("\nThe system will reboot in:\n");

            // Geri sayım ve ilerleme çubuğu
            for (int i = 10; i > 0; i--)
            {
                Console.Write($"[{new string('#', 10 - i)}{new string('.', i)}] {i} seconds remaining...");
                Thread.Sleep(1000); // Her saniyede bir bekle
                Console.Write("\r"); // Aynı satırı tekrar yaz
            }

            // Sistemi yeniden başlat
            Console.WriteLine("\n\nRebooting...");
            Thread.Sleep(2000);
            //Console.Clear();
            Sys.Power.Reboot(); // Gerçek reboot komutunuz burada yer alacak
        }


        private bool hasRun = false;  // İşletim sistemi bir kere çalıştı mı kontrol

        protected override void BeforeRun()
        {
            VFSManager.RegisterVFS(new CosmosVFS());
            //Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.Clear();
            ShowBootScreen();
            //Thread.Sleep(5000);  // 5 saniye bekleme süresi
        }

        protected override void Run()
        {
            // Beklenmedik bir hata meydana geldiğinde CrashScreen çağrılacak
            try
            {
                // Normal çalışacak kodlar burada olacak





                if (!hasRun)
                {
                    Console.Clear();
                    Console.WriteLine(ReplaceTurkishCharacters("Hoş geldiniz! CaYaOS başarıyla başlatıldı."));
                    //Console.WriteLine(ReplaceTurkishCharacters("Türkçe karakterler destekleniyor: çÇşŞıİğĞöÖüÜ"));
                    hasRun = true;
                }

                Console.WriteLine(ReplaceTurkishCharacters("1: Komut satırı"));
                Console.WriteLine(ReplaceTurkishCharacters("2: Sistem bilgileri"));
                Console.WriteLine(ReplaceTurkishCharacters("3: Dosya işlemleri"));
                Console.WriteLine(ReplaceTurkishCharacters("4: Sayı tahmin oyunu"));
                Console.WriteLine(ReplaceTurkishCharacters("5: Yılan oyunu"));
                Console.WriteLine(ReplaceTurkishCharacters("6: Tarih ve saat göster"));
                Console.WriteLine(ReplaceTurkishCharacters("7: Kronometre"));
                Console.WriteLine(ReplaceTurkishCharacters("8: Klasör oluştur"));
                Console.WriteLine(ReplaceTurkishCharacters("9: Hesap makinesi"));
                Console.WriteLine(ReplaceTurkishCharacters("10: Terminal renklerini değiştir"));
                Console.WriteLine(ReplaceTurkishCharacters("11: Sistemi kapat"));
                Console.Write("> " + ReplaceTurkishCharacters("Seçiminiz: "));

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowCommandLineInterface();
                        break;
                    case "2":
                        ShowSystemInfo();
                        break;
                    case "3":
                        FileOperations();
                        break;
                    case "4":
                        PlayNumberGuessingGame();
                        break;
                    case "5":
                        PlaySnakeGame();
                        break;
                    case "6":
                        ShowDateTime();
                        break;
                    case "7":
                        Stopwatch.RunStopwatch();
                        break;
                    case "8":
                        CreateFolder();
                        break;
                    case "9":
                        Calculator();
                        break;
                    case "10":
                        ChangeTerminalColors();
                        break;
                    case "11":
                        ExitSystem();
                        break;
                    default:
                        Console.WriteLine(ReplaceTurkishCharacters("Geçersiz seçim!"));
                        break;
                }






            }
            catch (Exception ex)
            {
                // Hata durumunda CrashScreen çağrılıyor
                CrashScreen(ex.Message);
            }

        }

        private void ShowBootScreen()
        {
            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;

            string logo = @"
               _____   __     __    ____   _____ 
              / ____|  \ \   / /   / __ \ / ____|
             | |     __ \ \_/ /_ _| |  | | (___  
             | |    / _ \   / _ | |  | |\___ \ 
             | |___| (_| || | (_| | |__| |____) |
              \_____\__,_||_|\__,_|\____/|_____/ 
                                     ";

            string loading = ReplaceTurkishCharacters("Yükleniyor...");

            int logoX = (screenWidth / 2) - (logo.Length / 2 / 7); // logoyu ortala
            int logoY = screenHeight / 3;

            Console.SetCursorPosition(logoX, logoY);
            Console.WriteLine(logo);

            int loadingX = (screenWidth / 2) - (loading.Length / 2);
            int loadingY = logoY + 8;
            Console.SetCursorPosition(loadingX, loadingY);
            Console.Write(loading);

            string[] loadingSymbols = { "|", "/", "-", "\\" };
            for (int i = 0; i < 20; i++)
            {
                Console.SetCursorPosition(loadingX + loading.Length, loadingY);
                Console.Write(loadingSymbols[i % 4]);
                Thread.Sleep(250);
            }
        }

        private void ShowCommandLineInterface()
        {
            Console.WriteLine(ReplaceTurkishCharacters("CaYaOS Komut Satırı"));

            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine();

                // Eğer komut "menu" ise döngüden çık
                if (command.ToLower() == "menu")
                {
                    Console.WriteLine(ReplaceTurkishCharacters("Ana menüye dönülüyor..."));
                    break; // Döngüden çık
                }

                // Diğer komutları işleme al
                HandleCommand(command);
            }
        }


        private void HandleCommand(string command)
        {
            string[] commandParts = command.Split(' ');
            string cmd = commandParts[0].ToLower();
            string[] args = commandParts.Skip(1).ToArray();

            switch (cmd)
            {
                case "help":
                    Console.WriteLine(ReplaceTurkishCharacters("Mevcut komutlar: help, time, exit, menu, dir, cd, start, mkdir, createfile, copy, move, ren, del"));
                    break;
                case "time":
                    Console.WriteLine(ReplaceTurkishCharacters("Şu anki saat: ") + DateTime.Now);
                    break;
                case "dir":
                    DirCommand();
                    break;
                case "cd":
                    CdCommand(args);
                    break;
                case "start":
                    OpenCommand(args);
                    break;
                case "mkdir":
                    MkdirCommand(args);
                    break;
                case "createfile":
                    CreateFileCommand(args);
                    break;
                case "copy":
                    CopyCommand(args);
                    break;
                case "move":
                    Console.WriteLine(ReplaceTurkishCharacters("Bu komut şuanda hizmette değildir."));
                    //MoveCommand(args);
                    break;
                case "ren":
                    //RenameCommand(args);
                    Console.WriteLine(ReplaceTurkishCharacters("Bu komut şuanda hizmette değildir."));
                    break;
                case "del":
                    DeleteCommand(args);
                    break;
                case "exit":
                    Console.WriteLine(ReplaceTurkishCharacters("Çıkılıyor..."));
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine(ReplaceTurkishCharacters("Bilinmeyen komut. 'help' yazabilirsiniz."));
                    break;
            }
        }
        private string currentDirectory = @"0:\";

        private void DirCommand()
        {
            try
            {
                // Klasörleri ve dosyaları al
                string[] files = System.IO.Directory.GetFiles(currentDirectory);
                string[] directories = System.IO.Directory.GetDirectories(currentDirectory);

                Console.WriteLine(ReplaceTurkishCharacters("Klasörler:"));
                foreach (string directory in directories)
                {
                    Console.WriteLine(System.IO.Path.GetFileName(directory));
                }

                Console.WriteLine(ReplaceTurkishCharacters("Dosyalar:"));
                foreach (string file in files)
                {
                    Console.WriteLine(System.IO.Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Hata: " + ex.Message));
            }
        }

        private void CdCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Kullanım: cd <klasör_adı>"));
                return;
            }

            // Eğer ".." kullanılırsa bir üst dizine çık
            if (args[0] == "..")
            {
                // Mevcut dizinden üst dizine çıkmak için yol ayarlayın
                string parentDirectory = System.IO.Directory.GetParent(currentDirectory)?.FullName;

                if (!string.IsNullOrEmpty(parentDirectory))
                {
                    currentDirectory = parentDirectory;
                    Console.WriteLine(ReplaceTurkishCharacters("Dizin değiştirildi: " + currentDirectory));
                }
                else
                {
                    Console.WriteLine(ReplaceTurkishCharacters("Üst dizin bulunamadı."));
                }
                return;
            }

            // Klasör adını boşluklara göre ayır ve tam yolu oluştur
            string directoryName = string.Join(" ", args);
            string newPath = System.IO.Path.Combine(currentDirectory, directoryName.Trim());

            if (System.IO.Directory.Exists(newPath))
            {
                currentDirectory = newPath;
                Console.WriteLine(ReplaceTurkishCharacters("Dizin değiştirildi: " + currentDirectory));
            }
            else
            {
                Console.WriteLine(ReplaceTurkishCharacters("Klasör bulunamadı: " + newPath));
            }
        }


        private void OpenCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Kullanım: start <dosya_adı>"));
                return;
            }

            // Argümanları birleştirerek tam dosya adını elde et
            string fileName = string.Join(" ", args).Trim('"'); // Çift tırnakları kaldır
            string filePath = System.IO.Path.Combine(currentDirectory, fileName);

            if (System.IO.File.Exists(filePath))
            {
                // Dosya içeriğini okuyun ve gösterin
                string content = System.IO.File.ReadAllText(filePath);
                Console.WriteLine(ReplaceTurkishCharacters("Dosya içeriği:"));
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine(ReplaceTurkishCharacters("Dosya bulunamadı: " + filePath));
            }
        }




        private void MkdirCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Kullanım: mkdir <klasör_adı>"));
                return;
            }

            string originalFolderName = args[0];
            string newFolderPath = System.IO.Path.Combine(currentDirectory, originalFolderName);

            // Aynı ada sahip klasör varsa, numara ekle
            int counter = 1;
            while (System.IO.Directory.Exists(newFolderPath))
            {
                newFolderPath = System.IO.Path.Combine(currentDirectory, $"{originalFolderName} ({counter})");
                counter++;
            }

            try
            {
                System.IO.Directory.CreateDirectory(newFolderPath);
                Console.WriteLine(ReplaceTurkishCharacters("Klasör oluşturuldu: " + newFolderPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Hata: " + ex.Message));
            }
        }


        private void CreateFileCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Kullanım: createfile <dosya_adı>"));
                return;
            }

            string originalFileName = args[0];
            string filePath = System.IO.Path.Combine(currentDirectory, originalFileName);

            // Aynı ada sahip dosya varsa, numara ekle
            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(originalFileName);
                string extension = System.IO.Path.GetExtension(originalFileName);
                filePath = System.IO.Path.Combine(currentDirectory, $"{fileNameWithoutExtension} ({counter}){extension}");
                counter++;
            }

            Console.WriteLine(ReplaceTurkishCharacters("Dosya içeriği giriniz:"));
            string content = Console.ReadLine();

            try
            {
                System.IO.File.WriteAllText(filePath, content);
                Console.WriteLine(ReplaceTurkishCharacters("Dosya oluşturuldu: " + filePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Hata: " + ex.Message));
            }
        }


        private void CopyCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Kullanım: copy <kaynak_dosya> <hedef_dosya>"));
                return;
            }

            // Kaynak ve hedef dosya adlarını birleştir
            string sourceFileName = string.Join(" ", args, 0, args.Length - 1).Trim('"'); // Çift tırnakları kaldır
            string destinationFileName = args[^1].Trim('"'); // Son elemanı al

            string sourcePath = System.IO.Path.Combine(currentDirectory, sourceFileName);
            string destinationPath = System.IO.Path.Combine(currentDirectory, destinationFileName);

            try
            {
                if (System.IO.File.Exists(sourcePath))
                {
                    System.IO.File.Copy(sourcePath, destinationPath, true); // true: var olan dosyanın üzerine yaz
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya kopyalandı: " + destinationPath));
                }
                else
                {
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya bulunamadı: " + sourcePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Hata: " + ex.Message));
            }
        }


        private void MoveCommand(string[] args)
        {
            // Argümanları kontrol et ve uygun şekilde birleştir
            if (args.Length < 2)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Kullanım: move <kaynak_dosya> <hedef_dosya>"));
                return;
            }

            // Kaynak ve hedef dosya adlarını birleştir
            string sourceFileName = string.Join(" ", args, 0, args.Length - 1); // Son argümanı hariç tut
            string destinationFileName = args[^1]; // Son argüman hedef dosya

            string sourcePath = System.IO.Path.Combine(currentDirectory, sourceFileName.Trim('"')); // Çift tırnakları kaldır
            string destinationPath = System.IO.Path.Combine(currentDirectory, destinationFileName.Trim('"')); // Çift tırnakları kaldır

            try
            {
                if (System.IO.File.Exists(sourcePath))
                {
                    System.IO.File.Move(sourcePath, destinationPath); // Dosyayı taşımak için Move kullan
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya taşındı: " + destinationPath));
                }
                else if (System.IO.Directory.Exists(sourcePath))
                {
                    System.IO.Directory.Move(sourcePath, destinationPath); // Klasörü taşımak için Move kullan
                    Console.WriteLine(ReplaceTurkishCharacters("Klasör taşındı: " + destinationPath));
                }
                else
                {
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya veya klasör bulunamadı: " + sourcePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Hata: " + ex.Message));
            }
        }


        private void RenameCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Kullanım: ren <mevcut_dosya_adı> <yeni_dosya_adı>"));
                return;
            }

            string oldPath = System.IO.Path.Combine(currentDirectory, args[0]);
            string newPath = System.IO.Path.Combine(currentDirectory, args[1]);

            try
            {
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Move(oldPath, newPath);
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya yeniden adlandırıldı: " + newPath));
                }
                else
                {
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya bulunamadı: " + oldPath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Hata: " + ex.Message));
            }
        }

        private void DeleteCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Kullanım: del <dosya_adı>"));
                return;
            }

            // Argümanları birleştirerek tam dosya adını elde et
            string fileName = string.Join(" ", args).Trim('"'); // Çift tırnakları kaldır
            string filePath = System.IO.Path.Combine(currentDirectory, fileName);

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya silindi: " + filePath));
                }
                else if (System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.Delete(filePath, true); // true: içeriğiyle birlikte sil
                    Console.WriteLine(ReplaceTurkishCharacters("Klasör silindi: " + filePath));
                }
                else
                {
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya veya klasör bulunamadı: " + filePath));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ReplaceTurkishCharacters("Hata: " + ex.Message));
            }
        }






        private void ShowSystemInfo()
        {
            Console.WriteLine(ReplaceTurkishCharacters("Sistem Bilgileri:"));
            Console.WriteLine(ReplaceTurkishCharacters("CPU: ") + Cosmos.Core.CPU.GetCPUBrandString());
            Console.WriteLine(ReplaceTurkishCharacters("Toplam RAM: ") + Cosmos.Core.CPU.GetAmountOfRAM() + " MB");
        }

        private void FileOperations()
        {
            Console.WriteLine(ReplaceTurkishCharacters("1: Dosya oluştur"));
            Console.WriteLine(ReplaceTurkishCharacters("2: Dosya oku"));
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write(ReplaceTurkishCharacters("Dosya adı giriniz: "));
                string fileName = Console.ReadLine();
                Console.Write(ReplaceTurkishCharacters("İçerik giriniz: "));
                string content = Console.ReadLine();
                System.IO.File.WriteAllText(@"0:\" + fileName, content);  // 0: disk sürücüsü (varsayılan)
                Console.WriteLine(ReplaceTurkishCharacters(fileName + " başarıyla oluşturuldu."));
            }
            else if (choice == "2")
            {
                Console.Write(ReplaceTurkishCharacters("Dosya adı giriniz: "));
                string fileName = Console.ReadLine();
                if (System.IO.File.Exists(@"0:\" + fileName))
                {
                    string content = System.IO.File.ReadAllText(@"0:\" + fileName);
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya içeriği: "));
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine(ReplaceTurkishCharacters("Dosya bulunamadı."));
                }
            }
        }

        private void PlayNumberGuessingGame()
        {
            Random rand = new Random();
            int numberToGuess = rand.Next(1, 101);
            int attempts = 0;
            int guess = 0;

            Console.WriteLine(ReplaceTurkishCharacters("1 ile 100 arasında bir sayı tuttum. Tahmin etmeye çalış!"));
            while (guess != numberToGuess)
            {
                Console.Write(ReplaceTurkishCharacters("Tahmininiz: "));
                guess = int.Parse(Console.ReadLine());
                attempts++;

                if (guess < numberToGuess)
                    Console.WriteLine(ReplaceTurkishCharacters("Daha büyük bir sayı deneyin."));
                else if (guess > numberToGuess)
                    Console.WriteLine(ReplaceTurkishCharacters("Daha küçük bir sayı deneyin."));
                else
                    Console.WriteLine(ReplaceTurkishCharacters($"Tebrikler! {attempts} denemede doğru tahmin ettiniz."));
            }
        }

        private void ChangeTerminalColors()
        {
            Console.WriteLine(ReplaceTurkishCharacters("1: Arka plan rengini değiştir"));
            Console.WriteLine(ReplaceTurkishCharacters("2: Yazı rengini değiştir"));
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine(ReplaceTurkishCharacters("Mevcut renkler: Siyah, Kırmızı, Yeşil, Mavi"));
                Console.Write(ReplaceTurkishCharacters("Renk seçiniz: "));
                string color = Console.ReadLine();
                if (color.ToLower() == "siyah")
                    Console.BackgroundColor = ConsoleColor.Black;
                else if (color.ToLower() == "kırmızı")
                    Console.BackgroundColor = ConsoleColor.Red;
                // Diğer renkler eklenebilir
                Console.Clear();
            }
            else if (choice == "2")
            {
                Console.WriteLine(ReplaceTurkishCharacters("Mevcut renkler: Beyaz, Sarı, Mavi, Yeşil"));
                Console.Write(ReplaceTurkishCharacters("Renk seçiniz: "));
                string color = Console.ReadLine();
                if (color.ToLower() == "beyaz")
                    Console.ForegroundColor = ConsoleColor.White;
                else if (color.ToLower() == "sarı")
                    Console.ForegroundColor = ConsoleColor.Yellow;
                // Diğer renkler eklenebilir
            }
        }

        private void ShowDateTime()
        {
            Console.WriteLine(ReplaceTurkishCharacters("Tarih ve Saat: ") + DateTime.Now);
        }

        private void CreateFolder()
        {
            Console.Write(ReplaceTurkishCharacters("Oluşturulacak klasörün adı: "));
            string folderName = Console.ReadLine();
            System.IO.Directory.CreateDirectory(@"0:\" + folderName);
            Console.WriteLine(ReplaceTurkishCharacters(folderName + " klasörü oluşturuldu."));
        }

        private void Calculator()
        {
            Console.WriteLine(ReplaceTurkishCharacters("Hangi işlemi yapmak istiyorsunuz? (+, -, *, /): "));
            string operation = Console.ReadLine();

            Console.Write(ReplaceTurkishCharacters("İlk sayı: "));
            double num1 = Convert.ToDouble(Console.ReadLine());

            Console.Write(ReplaceTurkishCharacters("İkinci sayı: "));
            double num2 = Convert.ToDouble(Console.ReadLine());

            double result = 0;
            switch (operation)
            {
                case "+":
                    result = num1 + num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "*":
                    result = num1 * num2;
                    break;
                case "/":
                    result = num1 / num2;
                    break;
                default:
                    Console.WriteLine(ReplaceTurkishCharacters("Geçersiz işlem!"));
                    return;
            }

            Console.WriteLine(ReplaceTurkishCharacters("Sonuç: ") + result);
        }

        private void ExitSystem()
        {
            Console.WriteLine(ReplaceTurkishCharacters("Çıkılıyor..."));
            Sys.Power.Shutdown();
        }

        private void PlaySnakeGame()
        {
            SnakeGame.Start();
        }




        









        public class SnakeGame
        {
            static int width = 20;
            static int height = 10;
            static List<(int x, int y)> snake;
            static (int x, int y) apple;
            static int score;
            static bool gameOver;
            static ConsoleKeyInfo keyInfo;
            static ConsoleKey currentKey = ConsoleKey.RightArrow;

            public static void Start()
            {
                do
                {
                    InitializeGame();
                    while (!gameOver)
                    {
                        if (Console.KeyAvailable)
                        {
                            keyInfo = Console.ReadKey(true);
                            currentKey = keyInfo.Key;
                        }
                        MoveSnake();
                        CheckCollision();
                        Draw();
                        Thread.Sleep(350); // Oyun hızını belirler
                    }

                    // Oyun bittiğinde kullanıcıdan yeni oyun iste
                    Console.Clear();
                    Console.WriteLine("Oyun Bitti! Skor: " + score);
                    Console.WriteLine("Yeni bir oyun oynamak ister misiniz? (E/H): ");
                    var response = Console.ReadKey(true);
                    gameOver = false; // Oyunu sıfırlıyoruz
                    if (response.Key != ConsoleKey.E) // 'E' dışında bir tuşa basıldığında oyundan çık
                    {
                        break;
                    }

                } while (true);
            }

            static void InitializeGame()
            {
                snake = new List<(int, int)>();
                snake.Add((width / 2, height / 2)); // Yılanın başlangıç konumu
                score = 0;
                gameOver = false;
                PlaceApple();
            }

            static void PlaceApple()
            {
                Random random = new Random();
                apple.x = random.Next(1, width - 1);
                apple.y = random.Next(1, height - 1);

                // Eğer elma yılanın üzerinde ise, yeni bir elma yerleştir
                while (snake.Contains((apple.x, apple.y)))
                {
                    apple.x = random.Next(1, width - 1);
                    apple.y = random.Next(1, height - 1);
                }
            }

            static void MoveSnake()
            {
                var head = snake[0]; // Yılanın başını al
                (int x, int y) newHead = head;

                switch (currentKey)
                {
                    case ConsoleKey.UpArrow:
                        newHead.y--; break;
                    case ConsoleKey.DownArrow:
                        newHead.y++; break;
                    case ConsoleKey.LeftArrow:
                        newHead.x--; break;
                    case ConsoleKey.RightArrow:
                        newHead.x++; break;
                }

                snake.Insert(0, newHead); // Yılanın başını güncelle
            }

            static void CheckCollision()
            {
                var head = snake[0];

                // Duvarlarla çarpışma
                if (head.x <= 0 || head.x >= width - 1 || head.y <= 0 || head.y >= height - 1)
                    gameOver = true;

                // Elma yeme durumu
                if (head.x == apple.x && head.y == apple.y)
                {
                    score++;
                    PlaceApple();
                }
                else
                {
                    snake.RemoveAt(snake.Count - 1); // Yılanın kuyruğunu çıkar
                }
            }

            static void Draw()
            {
                Console.Clear();

                // Oyun alanı çizimi
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        {
                            Console.Write("#");
                        }
                        else if (snake.Contains((x, y)))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("O");
                            Console.ResetColor();
                        }
                        else if (x == apple.x && y == apple.y)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("*");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("Skor: " + score);
            }
        }



        public class Stopwatch
        {
            private static bool isRunning = false;
            private static long startTime;
            private static long elapsedTime;

            public static void RunStopwatch()
            {
                Console.Clear();
                Console.WriteLine(ReplaceTurkishCharacters("Kronometre Uygulaması"));
                Console.WriteLine(ReplaceTurkishCharacters("Başlatmak için 'b', durdurmak için 'd' tuşlayın, çıkmak için 'q' tuşlayın."));
                Console.WriteLine(new string('=', 40));

                while (true)
                {
                    // Kullanıcı girişi
                    if (Console.KeyAvailable)
                    {
                        ConsoleKey key = Console.ReadKey(true).Key;

                        if (key == ConsoleKey.B) // Başlat
                        {
                            if (!isRunning)
                            {
                                isRunning = true;
                                startTime = GetCurrentTime();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(ReplaceTurkishCharacters("Kronometre başlatıldı."));
                            }
                        }
                        else if (key == ConsoleKey.D) // Durdur
                        {
                            if (isRunning)
                            {
                                isRunning = false;
                                elapsedTime += GetCurrentTime() - startTime;
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine(ReplaceTurkishCharacters($"Kronometre durduruldu. Süre: {FormatTime(elapsedTime)}"));
                            }
                        }
                        else if (key == ConsoleKey.Q) // Çıkış
                        {
                            Console.Clear();
                            break;
                        }
                    }

                    // Zamanı sürekli güncelle
                    if (isRunning)
                    {
                        long currentTime = GetCurrentTime();
                        long totalTime = elapsedTime + (currentTime - startTime);
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine(ReplaceTurkishCharacters($"Geçen Süre: {FormatTime(totalTime)}"));
                        Console.ForegroundColor = ConsoleColor.White; // Renkleri sıfırla
                    }

                    Thread.Sleep(100); // Güncellemeleri yavaşlat
                }
            }

            private static long GetCurrentTime()
            {
                return (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }

            private static string FormatTime(long milliseconds)
            {
                TimeSpan time = TimeSpan.FromMilliseconds(milliseconds);
                return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                    (int)time.TotalHours,
                    time.Minutes,
                    time.Seconds,
                    time.Milliseconds);
            }

            private static string ReplaceTurkishCharacters(string text)
            {
                return text
                    .Replace('ç', 'c').Replace('Ç', 'C')
                    .Replace('ş', 's').Replace('Ş', 'S')
                    .Replace('ı', 'i').Replace('İ', 'I')
                    .Replace('ğ', 'g').Replace('Ğ', 'G')
                    .Replace('ö', 'o').Replace('Ö', 'O')
                    .Replace('ü', 'u').Replace('Ü', 'U');
            }
        }






        private string ReplaceTurkishCharacters(string text)
        {
            return text
                .Replace('ç', 'c').Replace('Ç', 'C')
                .Replace('ş', 's').Replace('Ş', 'S')
                .Replace('ı', 'i').Replace('İ', 'I')
                .Replace('ğ', 'g').Replace('Ğ', 'G')
                .Replace('ö', 'o').Replace('Ö', 'O')
                .Replace('ü', 'u').Replace('Ü', 'U');
        }
    }
}