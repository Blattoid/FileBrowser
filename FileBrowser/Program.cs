using System;
using System.IO;
using System.Diagnostics;

//This 'solution' is a giant training ground for me to get the hang of C#. 
//Considering I started from nothing at all, it's pretty good.
namespace FileBrowser
{
    class Program
    {
        static void Main(string[] args)
        {
            float version = 2.0F;
            string appName = "File Browser";

            //Define a few variables for later use.
            string cmd;
            GlobalVariables.logFile = "\\log.txt";

            /*
            Legend for colours:
            Red: Error
            Blue: General information (cmd usage or help)
            White: Title/User input
            Green: ls (files)
            Magenta: ls(folders)
            Yellow: pwd
            */

            //Logging is disabled on startup.
            GlobalVariables.enableLogging = false;

            //Set executable path. This is used to make sure the log file is stored next to the executable.
            GlobalVariables.ExecutableLocation = Directory.GetCurrentDirectory();

            //Log when program starts. This happens regardless of whether or not enableLogging is true >:3
           File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), Environment.NewLine + DateTime.Now.ToString() + " [PROGRAM LAUNCHED] Current directory is '" + GlobalVariables.ExecutableLocation + "'." + Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = (appName + " V" + version);
            Console.WriteLine("-=" + appName + " Version " + version + "=-\nType 'help' for a list of commands.");

            //Infinite loop
            for (;;)
            {
                Console.ForegroundColor = ConsoleColor.White;
                //Prefix the Console.Readline():
                Console.Write("> ");
                cmd = Console.ReadLine();

                if (GlobalVariables.enableLogging == true)
                {
                    //Log user activity to file. :P
                    File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), DateTime.Now.ToString() + " Issued command '" + cmd + "'" + Environment.NewLine);
                }
                //Convert to uppercase
                cmd = cmd.ToUpper();

                if (cmd == "") { } //Do nothing since no command was entered.
                else if (cmd == "HELP")
                {
                    Help.main help = new Help.main();
                    help.displayHelp();
                }
                //Put commands here. 
                // || means 'or' in C#.
                else if (cmd == "LS" || cmd == "DIR")
                {
                    List.main ls = new List.main();
                    ls.list();
                }
                else if (cmd == "LEGEND")
                {
                    Legend.main legend = new Legend.main();
                    legend.legend();
                }
                else if (cmd.StartsWith("CD "))
                {
                    //Strip the command, leaving the argument.
                    cmd = cmd.Replace("CD ", "");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Cd.main cd = new Cd.main();
                    cd.cd(cmd);
                }
                else if (cmd == "CD")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Allows you to move into a folder.\nUsage: 'cd <folder>'");
                }
                else if (cmd == "PWD")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n" + Directory.GetCurrentDirectory());
                }
                else if (cmd == "READ")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Allows reading from a text file.\nUsage: 'read <file>'");
                }
                else if (cmd.StartsWith("READ "))
                {
                    //Strip cmd name from input, leaving argument behind.
                    cmd = cmd.Replace("READ ", "");

                    Read.main readfile = new Read.main();
                    readfile.readFile(cmd);
                }
                else if (cmd == "DATE" || cmd == "TIME")
                {
                    Console.WriteLine(DateTime.Now.ToString());
                }
                else if (cmd.StartsWith("PLAYSND "))
                {
                    //Strip cmd name from input, leaving argument behind.
                    cmd = cmd.Replace("PLAYSND ", "");

                    if (cmd == "")
                    {
                        Console.WriteLine("Allows playback of .wav files. Note that it can only play .wav's.\nUsage: 'playsnd <file.wav>");
                    }
                    else
                    {
                        Sound.main sound = new Sound.main();
                        sound.playSound(cmd);
                    }
                }
                else if (cmd == "STOPSND")
                {
                    Sound.main sound = new Sound.main();
                    sound.Stopsound();
                }
                else if (cmd == "PLAYSND")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Allows playback of .wav files. Note that it can only play .wav's.\nUsage: 'playsnd <file.wav>");
                }
                else if (cmd == "EXIT")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
                else if (cmd == "SHUTDOWN")
                {
                    Process.Start("shutdown", "/s /t 10");
                    Console.WriteLine("Shutting down in 10 seconds.");
                }
                else if (cmd == "DISABLE_LOGGING")
                {
                    Logging.main log = new Logging.main();
                    log.stopLogging();
                }
                else if (cmd == "ENABLE_LOGGING")
                {
                    Logging.main log = new Logging.main();
                    log.startLogging();
                }
                else if (cmd == "LOGGING_STATUS")
                {
                    Logging.main log = new Logging.main();
                    log.statusLogging();
                }
                else if (cmd == "LIST_DRIVES")
                {
                    ListDrives.main listdrives = new ListDrives.main();
                    listdrives.listdrives();
                }
                //Handles no argument provided
                else if (cmd == "COLOUR")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Customize the background colour of the window.\nUsage: 'colour <id 0-8>'");

                    Colour.main colour = new Colour.main();
                    colour.getListOfColours();
                }
                //Handles arguments
                else if (cmd.StartsWith("COLOUR"))
                {
                    //Strip cmd name from input, leaving argument behind.
                    cmd = cmd.Replace("COLOUR ", "");

                    Colour.main colour = new Colour.main();
                    colour.setBackgroundColour(cmd);
                }
                else
                {
                    if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), "Error: Unknown command." + Environment.NewLine); }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unrecognised command.");
                }
            }
        }
    }
}

//Namespaces are one honking great idea -- let's do more of those!
namespace Read
{
    class main
    {
        public void readFile(string filename)
        {
            try
            {
                //filesize is in bytes.
                FileInfo fileinfo = new FileInfo(@filename);
                Int32 filesize = Convert.ToInt32(fileinfo.Length);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("File is " + filesize + " bytes.");

                try
                {
                    //Open file for reading
                    if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Reading file...");}
                    string[] data = File.ReadAllLines(@filename);
                    Console.WriteLine("Contents of '" + filename + "':");
                    // Display the file contents by using a foreach loop.
                    Console.ForegroundColor = ConsoleColor.White;
                    
                    //Write each line of the file to the screen
                    foreach (string line in data){Console.WriteLine(line);}

                    if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Success." + Environment.NewLine);}
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("End of file.");
                }
                catch (FileNotFoundException)
                {
                    if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: File not found." + Environment.NewLine);}
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: File not found.");
                }
                catch (ArgumentException)
                {
                    if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Path uses illegal characters." + Environment.NewLine);}
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Path uses illegal characters.");
                }
                catch
                {
                    if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " An unknown error has occurred." + Environment.NewLine);}
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("An unknown error has occurred.");
                }
            }
            catch (OverflowException) { Console.WriteLine("File exceeds maximum size for reading."); }
            catch (FileNotFoundException)
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: File not found." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: File not found.");
            }
            catch (ArgumentException)
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Path uses illegal characters." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Path uses illegal characters.");
            }
            catch
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " An unknown error has occurred." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unknown error has occurred.");
            }
        }
    }
}

namespace List
{
    class main
    {
        public void list()
        {
            try
            {
                string file;
                //Files
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Listing files...");}
                Console.ForegroundColor = ConsoleColor.Green;
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
                foreach (string temp in files)
                {
                    file = Path.GetFileName(temp);
                    Console.WriteLine("<FILE> " + file);
                }
                //Folders
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Listing folders...");}
                Console.ForegroundColor = ConsoleColor.Magenta;
                string[] folders = Directory.GetDirectories(Directory.GetCurrentDirectory());
                foreach (string temp in folders)
                {
                    file = Path.GetFileName(temp);
                    Console.WriteLine(" <DIR> " + file);
                }
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Done." + Environment.NewLine);}
            }
            catch
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " An unknown error has occurred." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unknown error has occurred.");
            }
        }
    }
}

namespace Legend
{
    class main
    {
        public void legend()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nLegend for colours in commands:");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Red: Error");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Blue: General information");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("White: Title / User input / Data read from file");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Green: ls (Files)");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Magenta: ls (Folders)");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Yellow: pwd\n");
        }
    }
}

namespace Cd
{
    class main
    {
        public void cd(string directory)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(@directory);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), DateTime.Now.ToString() + " Changing directory...");}
                    Directory.SetCurrentDirectory(directory);
                    if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Success." + Environment.NewLine);}
                    Console.WriteLine("Changed directory.");
                }
                else
                {
                    if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Not a directory." + Environment.NewLine);}
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Not a directory.");
                }
            }
            catch (DirectoryNotFoundException)
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Folder not found." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Folder not found.");
            }
            catch (IOException)
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Could not enter directory." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Could not enter directory.");
            }
            catch (ArgumentException)
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Path uses illegal characters." + Environment.NewLine);}

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Path uses illegal characters.");
            }
            catch
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " An unknown error has occurred." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unknown error has occurred.");
            }
        }
    }
}

namespace Colour
{
    class main
    {
        public void getListOfColours()
        {
            Console.WriteLine("0 = Black      1 = Blue");
            Console.WriteLine("2 = Green      3 = Aqua");
            Console.WriteLine("4 = Red        5 = Purple");
            Console.WriteLine("6 = Yellow     7 = White");
            Console.WriteLine("8 = Dark Grey");
        }
        public void setBackgroundColour(string input)
        {
            try
            {
               int alias = Int32.Parse(input);
                //Hideous way of checking if input is valid 
                if (alias == 0 | alias == 1 | alias == 2 | alias == 3 | alias == 4 | alias == 5 | alias == 6 | alias == 7 | alias == 8)
                {
                    //Yay! You did as told. :) Now let's reward you with a background colour.
                    if (alias == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Clear();
                    }
                    else if (alias == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.Clear();
                    }
                    else if (alias == 2)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.Clear();
                    }
                    else if (alias == 3)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.Clear();
                    }
                    else if (alias == 4)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.Clear();
                    }
                    else if (alias == 5)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        Console.Clear();
                    }
                    else if (alias == 6)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.Clear();
                    }
                    else if (alias == 7)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.Clear();
                    }
                    else if (alias == 8)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Clear();
                    }
                    else
                    {
                        if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Internal error." + Environment.NewLine); }
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: Internal error.");
                    }

                }
                else
                {
                    if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Invalid input. Must be 0-8." + Environment.NewLine); }
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Invalid input. Must be 0-8.");
                }
            }
            catch (FormatException)
            {
                if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Invalid input. Must be 0-8." + Environment.NewLine); }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Invalid input. Must be 0-8.");
            }
            catch (OverflowException)
            {
                if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: Overflow" + Environment.NewLine); }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: Overflow");
            }
            /*
            catch (Exception)
            {
                if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " An unknown error has occurred." + Environment.NewLine); }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unknown error has occurred.");
            }
            */
        }
    }
}

namespace Sound
{
    class main
    {
        public void playSound(string filepath)
        {
            try
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), DateTime.Now.ToString() + " Attempting to open file '" + filepath + "'...");}
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Starting playback of " + filepath);
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@filepath);
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Success. Starting playback...");}

                //The mystical play command O_O
                player.Play();

                //Newline :3
                if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), Environment.NewLine); }
            }
            catch (ArgumentException)
            {
                if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: File path contains illegal characters." + Environment.NewLine); }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: File path contains illegal characters.");
            }
            catch (InvalidOperationException)
            {
                if (GlobalVariables.enableLogging == true){ File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: File is corrupted or not a wav file." + Environment.NewLine); }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: File is corrupted or not a wav file.");
            }
            catch (FileNotFoundException)
            {
                if (GlobalVariables.enableLogging == true) {File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Error: File could not be found." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: File could not be found.");
            }
            catch
            {
                if (GlobalVariables.enableLogging == true){File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " An unknown error has occurred." + Environment.NewLine);}
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unknown error has occurred.");
            }
        }
        public void Stopsound()
        {
            if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), " Stopping playback of audio file." + Environment.NewLine);}
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Stopping playback of sound file.");
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.Stop();
        }

    }
}

namespace Help
{
    class main
    {
        public void displayHelp()
        {
            //Trickery to make text different colours on the same line. The code is however, very ugly to look at :/
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Cyan ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("indicates a command that requires an argument.\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Yellow ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("is a command which needs no argument.\n");

            Console.Write("Commands: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("cd");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("date");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("enable_logging");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("disable_logging");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("logging_status");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("shutdown");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("clear");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("dir\n");
            //newline
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("list_drives");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("exit");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("legend");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("colour");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("ls");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("pwd");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("read");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("playsnd");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("stopsnd");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(", ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("time" + Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Typing a command that requires a parameter outright gives further help.");
        }
    }
}

namespace ListDrives
{
    class main
    {
        public void listdrives()
        {

            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                DriveInfo[] myDrives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in myDrives)
                {
                    if (drive.IsReady == true)
                    {
                        //Remember to look up the proper values
                        int kilobyte_size = 1024;
                        int megabyte_size = 1024000;
                        int gigabyte_size = 1024000000;
                        //Int64 is needed due to the numbers being way too big for 32 bit .-.
                        Int64 terabyte_size = Convert.ToInt64(1024000000000);
                        string size;

                        try
                        {
                            //Calculate readable drive size. Uses kilobytes, megabytes, gigabytes and even terabytes.
                            if (drive.TotalSize > terabyte_size)
                            {
                                //Yikes.
                                float formatted_size = drive.TotalSize / terabyte_size;
                                size = (formatted_size.ToString() + " TB");
                            }
                            else if (drive.TotalSize > gigabyte_size)
                            {
                                //Expectable
                                float formatted_size = drive.TotalSize / gigabyte_size;
                                size = (formatted_size.ToString() + " GB");
                            }
                            else if (drive.TotalSize > megabyte_size)
                            {
                                //Uncommon.
                                float formatted_size = drive.TotalSize / megabyte_size;
                                size = (formatted_size.ToString() + " MB");
                            }
                            else if (drive.TotalSize > kilobyte_size)
                            {
                                //Rare. Must be using a floppy drive or something.
                                float formatted_size = drive.TotalSize / kilobyte_size;
                                size = (formatted_size.ToString() + " KB");
                            }
                            else
                            {
                                //Simply return the size in bytes. We tried.
                                size = (drive.TotalSize + " B");
                            }
                        }
                        catch (DivideByZeroException) { size = "ERR"; }

                        //Calculate free space percentage. Uses sorcery to convert long data type into int.
                        double freespacepercentage = 100 * (double)drive.TotalFreeSpace / drive.TotalSize;
                        //Round to 1 decimal place
                        freespacepercentage = Convert.ToDouble(freespacepercentage.ToString("n1"));
                        Console.WriteLine(drive.Name + " (" + drive.VolumeLabel + ") " + freespacepercentage + "% free of " + size + ".");
                        if (GlobalVariables.enableLogging == true) { File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), drive.Name + " (" + drive.VolumeLabel + ") " + freespacepercentage + "% free of " + size + "." + Environment.NewLine); }
                    }
                    else { Console.WriteLine(drive.Name); }
                }
            }
            catch (Exception) { throw; }
        }
    }
}

namespace Logging
{
    class main
    {
        public void stopLogging()
        {
                File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), DateTime.Now.ToString() + " Disabling logging. Goodbye." + Environment.NewLine);
                GlobalVariables.enableLogging = false;
        }
        public void startLogging()
        {
            File.AppendAllText((GlobalVariables.ExecutableLocation + GlobalVariables.logFile), DateTime.Now.ToString() + " Logging enabled. Hello!" + Environment.NewLine);
            GlobalVariables.enableLogging = true;
        }
        public void statusLogging()
        {
            if (GlobalVariables.enableLogging == true){Console.WriteLine("Logging is currently ENABLED.");}
            else{Console.WriteLine("Logging is currently DISABLED.");}
        }
    }
}


//I know, terrible idea. Screw you. I can do what I want.
public static class GlobalVariables
{
    public static string ExecutableLocation;
    public static string logFile;
    public static bool enableLogging;
}