using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ServerConsole
{
    public class ColorsNames
    {
        public static string FilePathColors = Environment.CurrentDirectory;
        public DataTable colors=new DataTable("colors");
        private string colorsRgb;
        private string colorname;
        public static DataTable ColorsTable=new DataTable("colors");

        public ColorsNames()
        {
            FilePathColors= FilePathColors.Substring(0, FilePathColors.LastIndexOf("bin")) + "colors.Xml";
            DataColumn col1 = new DataColumn();
            col1.ColumnName = "Name";
            col1.DataType = typeof(string);
            col1.Unique = true;
            colors.Columns.Add(col1);
            DataColumn col2 = new DataColumn();
            col2.ColumnName = "R";
            col2.DataType = typeof(string);
            colors.Columns.Add(col2);
            col2 = new DataColumn();
            col2.ColumnName = "G";
            col2.DataType = typeof(string);
            colors.Columns.Add(col2);
            col2 = new DataColumn();
            col2.ColumnName = "B";
            col2.DataType = typeof(string);
            colors.Columns.Add(col2);
            colors.ReadXml(FilePathColors);
        }

        public string detectColor(string rgb)
        {
            string r, g, b,name="";
            int rNum, gNum, bNum,temp1=0,temp2=0,temp3=0;

            r = rgb.Substring(rgb.IndexOf("R") + 2, 3);
            Console.WriteLine("r "+r);
            g= rgb.Substring(rgb.IndexOf("G") + 2, 3);
            Console.WriteLine("g " + g);
            b =rgb.Substring(rgb.IndexOf("B") + 2, 3);
            Console.WriteLine("b " + b);
            
            if(r.Contains(','))
            {
                r = rgb.Substring(rgb.IndexOf("R") + 2, 2);
            }
            if (g.Contains(','))
            {
                g = rgb.Substring(rgb.IndexOf("G") + 2, 2);
            }
            if (b.Contains(']'))
            {
                b = rgb.Substring(rgb.IndexOf("B") + 2, 2);
                if (b.Contains(']'))
                    b = rgb.Substring(rgb.IndexOf("B") + 2, 1);
            }
            rNum = int.Parse(r);
            gNum = int.Parse(g);
            bNum = int.Parse(b);

            foreach(DataRow row in colors.Rows)
            {
                temp1 = int.Parse(row["R"].ToString());
                temp2 = int.Parse(row["G"].ToString());
                temp3 = int.Parse(row["B"].ToString());

                if (Math.Abs(rNum - temp1) <= 70)
                {
                    if(Math.Abs(gNum - temp2) <= 70)
                    {
                        if (Math.Abs(bNum - temp3) <= 70)
                        {
                            return row["Name"].ToString();
                        }
                    }
                }
            }

            return "Undefined Color "+rgb;
        }
    }
}
