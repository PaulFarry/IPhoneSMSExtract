using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace SMSConverter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var dbPath = @"d:\Users\Paul Farry\Desktop\sms.sqlite3";
            using (var dataFile = new StreamWriter(@"d:\output.txt", false))
            {
                //d:\Users\<user>\AppData\Roaming\Apple Computer\MobileSync\Backup\<identifier>\3d\3d0d7e5fb2ce288813306e4d4636395e047a3d28
                List<string> ImportedFiles = new List<string>();
                using (var connect = new SQLiteConnection($"Data Source={dbPath}"))
                {
                    connect.Open();
                    using (var fmd = connect.CreateCommand())
                    {
                        fmd.CommandText = @"select datetime(substr(date, 1, 9) + 978307200, 'unixepoch', 'localtime') as f_date, text, is_from_me, rowid, cache_has_attachments from message where handle_id=1 and rowid>=18067";
                        fmd.CommandType = CommandType.Text;
                        using (var r = fmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                var hasAttachments = r.GetInt32(4);
                                var rowId = r.GetInt32(3);
                                var isFromMe = r.GetInt32(2);
                                var displayDirection = (isFromMe == 0) ? ">>" : "<<";
                                var messageDate = r[0].ToString();
                                var message = r[1].ToString();
                                if (hasAttachments == 1)
                                {
                                    message = "<Image>";
                                }

                                dataFile.WriteLine($"{rowId}\t{messageDate}\t{displayDirection}\t{message}");
                            }
                        }
                    }

                }
                dataFile.Close();
            }

            //

            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new Form1());
        }
    }
}
