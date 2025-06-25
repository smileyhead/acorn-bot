using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using System.Globalization;

namespace Acorn.Commands_Text
{
    public static class DiskCommand
    {
        [Command("disk")]
        public static async ValueTask TextOnlyAsync(TextCommandContext context)
        {
            Console.WriteLine("Returning disk information.");

            FileInfo quotesInfo = new FileInfo(Program.quotesPath);
            DirectoryInfo backupsInfo = new DirectoryInfo(Program.backupsPath);
            DriveInfo driveInfo = new DriveInfo(DriveInfo.GetDrives().FirstOrDefault().Name); //The computer I'm writing this for only has a single drive in it.
                                                                                              //If anyone is forking this bot, this might need to be changed.
            double driveFull = (driveInfo.TotalSize - driveInfo.AvailableFreeSpace) / (double)driveInfo.TotalSize;

            string answer = $"Quotes database size: **{SizeSuffix(quotesInfo.Length)}**\nThere ";
            if (backupsInfo.GetFiles().Count() == 1) answer += "is **1** backup";
            else answer += $"are **{backupsInfo.GetFiles().Count()}** backups";
            if (backupsInfo.GetFiles().Count() == 0) answer += ".\n";
            else answer += $", totalling **{SizeSuffix(DirSize(backupsInfo))}**.";
            answer += $"\nThere is **{SizeSuffix(driveInfo.AvailableFreeSpace)}** free space left on the disk (**{(driveFull * 100).ToString("N2",
                  CultureInfo.CreateSpecificCulture("en-US"))}% ** full).";

            await context.RespondAsync(answer);
        }


        //Credit for the following: https://stackoverflow.com/a/14488941
        static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static string SizeSuffix(Int64 value, int decimalPlaces = 2)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }


        //Credit for the following: https://stackoverflow.com/a/468131
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }
    }
}
