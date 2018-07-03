using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GenCodeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            txtNameSpace.Text = "GenCodeWpf";
            txtClass.Text = "_Person";
            SetString(richTextBox1, "string HoTen\nDateTime NgaySinh\nstring DiaChi");


        }

        const string TAB1 = "\t";
        const string TAB2 = "\t\t";
        const string TAB3 = "\t\t\t";
        const string TAB4 = "\t\t\t\t";
        const string TAB5 = "\t\t\t\t\t";
        const string TAB6 = "\t\t\t\t\t\t";
        const string TAB7 = "\t\t\t\t\t\t\t";
        const string TAB8 = "\t\t\t\t\t\t\t\t";

        private string GetString(RichTextBox rtb)
        {
            return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
        }

        private void SetString(RichTextBox rtb, string s)
        {
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run(s)));
        }


        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            richTextBox2.SelectAll();
            richTextBox2.Copy();
        }

        private List<Variable> GetVars(string s)
        {
            List<Variable> vars = new List<Variable>();

            foreach (string propertie in s.Split('\n'))
            {

                char[] v = propertie.Trim(' ', '\r').ToCharArray();

                for (int i = 0; i < v.Length; i++)
                {
                    if (v[i] == ' ')
                    {
                        vars.Add(new Variable(new string(v, 0, i), new string(v, i + 1, v.Length - 1 - i)));
                        break;
                    }
                }
            }

            return vars;
        }


        private void btnGenVO_Click(object sender, RoutedEventArgs e)
        {
            string rtb1 = GetString(richTextBox1);
            List<Variable> vars = GetVars(rtb1);

            string s = "using System;\n\n" +
                "namespace " + txtNameSpace.Text + "\n{\n" +
                "\tpublic class " + txtClass.Text + "\n\t{\n";

            s += TAB2 + "#region Fields\n";
            foreach (Variable v in vars)
                s += v.Declare;

            s += TAB2 + "#endregion\n\n";

            s += TAB2 + "#region Properties";
            foreach (Variable v in vars)
                s += v.GetSet;

            s += TAB2 + "#endregion\n";

            s += TAB1 + "}\n}\n";

            // Ket thuc
            SetString(richTextBox2, s);

        }

        private void btnGenSQL_Click(object sender, RoutedEventArgs e)
        {
            string rtb1 = GetString(richTextBox1);
            List<Variable> vars = GetVars(rtb1);

            string nameTable = "";
            char[] t = txtClass.Text.Trim().ToCharArray();
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == '_')
                {
                    nameTable = new string(t, i + 1, t.Length - 1 - i);
                    break;
                }
            }

            if (nameTable == "") return;

            string s = "using System;\nusing System.Data;\nusing MySql.Data.MySqlClient;\n\n" +
                "namespace " + txtNameSpace.Text + "\n{\n" +
                "\tclass SQL_" + nameTable + "\n\t{\n";

            s += TAB2 + "DatabaseAccess da = new DatabaseAccess();\n\n";

            // Ham 1 Lay du lieu tu bang
            s += TAB2 + "// Lay du lieu tu bang\n";
            s += TAB2 + string.Format("public {0} {1}()\n\t\t{{\n", "DataSet", "GetDataSet");

            s += string.Format(
                TAB3 + "string str = \"select * from {0}\";\n" +
                TAB3 + "MySqlCommand cmd = new MySqlCommand(str, da.Conn);\n" +
                TAB3 + "return da.executeQuery(cmd);\n" +
                TAB2 + "}}\n\n",
                nameTable);

            // Ham 2 Lay du lieu tu bang = condition
            s += TAB2 + "// Lay du lieu tu bang = condition\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "DataSet", "GetDataSetEqual", "string", "condition");
            s += string.Format(
               TAB3 + "string str = \"select * from {0} where id = @Id\";\n" +
               TAB3 + "MySqlCommand cmd = new MySqlCommand(str, da.Conn);\n" +
               TAB3 + "cmd.Parameters.Add(\"@Id\", MySqlDbType.String).Value = condition;\n" +
               TAB3 + "return da.executeQuery(cmd);\n" +
               TAB2 + "}}\n\n",
               nameTable, "condition");

            // Ham 3 Lay du lieu tu bang >= condition
            s += TAB2 + "// Lay du lieu tu bang >= condition\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "DataSet", "GetDataSetGreat", "string", "condition");
            s += string.Format(
               TAB3 + "string str = \"select * from {0} where id >= @Id order by id asc\";\n" +
               TAB3 + "MySqlCommand cmd = new MySqlCommand(str, da.Conn);\n" +
               TAB3 + "cmd.Parameters.Add(\"@Id\", MySqlDbType.String).Value = condition;\n" +
               TAB3 + "return da.executeQuery(cmd);\n" +
               TAB2 + "}}\n\n",
               nameTable, "condition");

            //// Ham 4
            //nameClass += string.Format("\t\tpublic {0} {1}{2}({3} {4})\n\t\t{{\n", "DataSet", "GetDataSetFrom", "Huyen", "string", "condition");
            //nameClass += string.Format(
            //   "\t\t\tstring str = \"select * from {0} where ma{1} = \" + {2};\n" +
            //   "\t\t\treturn da.executeQuery(str);\n" +
            //   "\t\t}}\n\n",
            //   nameTable, "Huyen", "condition"
            //   );

            // Ham 5 Them du lieu
            s += TAB2 + "// Them du lieu\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "void", "Add", "VO_" + nameTable, "vo");
            s += string.Format(
               TAB3 + "string str = \"insert into {0} values (", nameTable);



            foreach (Variable v in vars)
            {
                s += "@" + v.Name + ", ";

            }

            s = s.Substring(0, s.Length - 2);

            s += ")\";\n";

            s += TAB3 + "MySqlCommand cmd = new MySqlCommand(str, da.Conn);\n";

            foreach (Variable v in vars)
            {
                s += TAB3 + "cmd.Parameters.Add(\"@" + v.Name + "\", MySqlDbType." + v.TypeH + ").Value = vo." + v.Name + ";\n";

            }


            s += TAB3 + "da.executeNonQuery(cmd);\n" +
              TAB2 + "}\n\n";

            // Ham 6 Edit du lieu
            s += TAB2 + "// Sua du lieu\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "void", "Edit", "VO_" + nameTable, "vo");
            s += string.Format(
               TAB3 + "string str = \"update {0} \";\n", nameTable);
            s += TAB2 + "str +=  \"set ";

            for (int i = 1; i < vars.Count; i++)
            {

                s += vars[i].Var + " = @" + vars[i].Name + ", ";
            }

            s = s.Substring(0, s.Length - 2);

            s += " \";\n";

            s += TAB3 + "str += \"where " + vars[0].Var + " = @" + vars[0].Name + "\";\n";

            s += TAB3 + "MySqlCommand cmd = new MySqlCommand(str, da.Conn);\n";
            foreach (Variable v in vars)
            {
                s += TAB3 + "cmd.Parameters.Add(\"@" + v.Name + "\", MySqlDbType." + v.TypeH + ").Value = vo." + v.Name + ";\n";

            }

            s += TAB3 + "da.executeNonQuery(cmd);\n" +
              TAB2 + "}\n\n";

            // Ham 7 Delete du lieu
            s += TAB2 + "// Delete du lieu\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "void", "Delete", "VO_" + nameTable, "vo");
            s += string.Format(
               TAB3 + "string str = \"delete from {0} ", nameTable);
            s += "where " + vars[0].Var + " = @" + vars[0].Name + "\";\n";
            s += TAB3 + "MySqlCommand cmd = new MySqlCommand(str, da.Conn);\n";
           
            s += TAB3 + "cmd.Parameters.Add(\"@" + vars[0].Name + "\", MySqlDbType." + vars[0].TypeH + ").Value = vo." + vars[0].Name + ";\n";
           
            s += TAB3 + "da.executeNonQuery(cmd);\n" +
             TAB2 + "}\n\n";

            // Ham 8 Dem so luong ban ghi
            s += TAB2 + "// Dem so luong ban ghi\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "int", "CountRow", "", "");
            s += TAB3 + "return GetDataSet().Tables[0].Rows.Count;\n" +
             TAB2 + "\t\t}\n\n";

            // Ham 9 Dem so luong ban ghi = condition
            s += TAB2 + "// Dem so luong ban ghi = condition\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "int", "CountRowEqual", "string", "condition");
            s += TAB3 + "return GetDataSetEqual(condition).Tables[0].Rows.Count;\n" +
             TAB2 + "}\n\n";

            // Ham 10 Dem so luong ban ghi >= condition
            s += TAB2 + "// Dem so luong ban ghi >= condition\n";
            s += TAB2 + "DataSet ds;\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "int", "CountRowGreat", "string", "condition");
            s += TAB3 + "ds = GetDataSetGreat(condition);\n";
            s += TAB3 + "return ds.Tables[0].Rows.Count;\n" +
            TAB2 + "}\n\n";

            // Ham 11 Get row id
            s += TAB2 + "// Get row id\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "string", "GetRowId", "int", "row");
            s += TAB3 + "return ds.Tables[0].Rows[row][0].ToString();\n" +
            TAB2 + "}\n\n";

            s += TAB1 + "}\n}\n";
            // Ket thuc
            SetString(richTextBox2, s);

        }

        private void btnGenBUS_Click(object sender, RoutedEventArgs e)
        {
            string rtb1 = GetString(richTextBox1);
            List<Variable> vars = GetVars(rtb1);

            string nameTable = "";
            char[] t = txtClass.Text.Trim().ToCharArray();
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == '_')
                {
                    nameTable = new string(t, i + 1, t.Length - 1 - i);
                    break;
                }
            }

            if (nameTable == "") return;

            string s = "using System;\nusing System.Data;\n\n" +
                "namespace " + txtNameSpace.Text + "\n{\n" +
                "\tpublic class BUS_" + nameTable + "\n\t{\n";

            s += TAB2 + string.Format("SQL_{0} sql = new SQL_{0}();\n\n", nameTable);

            // Ham 1 Lay du lieu tu bang
            s += TAB2 + "// Lay du lieu tu bang\n";
            s += TAB2 + string.Format("public {0} {1}()\n\t\t{{\n", "DataSet", "GetDataSet");

            s += string.Format(
                TAB3 + "return sql.GetDataSet();\n" +
                TAB2 + "}}\n\n"
                );

            // Ham 2 Lay du lieu tu bang = condition
            s += TAB2 + "// Lay du lieu tu bang = condition\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "DataSet", "GetDataSetEqual", "string", "condition");
            s += string.Format(
              TAB3 + "return sql.GetDataSetEqual(condition);\n" +
              TAB2 + "}}\n\n"
               );

            // Ham 3 Lay du lieu tu bang >= condition
            s += TAB2 + "// Lay du lieu tu bang >= condition\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "DataSet", "GetDataSetGreat", "string", "condition");
            s += string.Format(
               TAB3 + "return sql.GetDataSetGreat(condition);\n" +
               TAB2 + "}}\n\n"
               );

            //// Ham 4
            //nameClass += string.Format("\t\tpublic {0} {1}{2}({3} {4})\n\t\t{{\n", "DataSet", "GetDataSetFrom", "Huyen", "string", "condition");
            //nameClass += string.Format(
            //   "\t\t\treturn sql.GetDataSetFrom{0}(condition);\n" +
            //   "\t\t}}\n\n",
            //  "Huyen"
            //   );

            // Ham 5 Them du lieu
            s += TAB2 + "// Them du lieu\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "void", "Add", "VO_" + nameTable, "vo");
            s += string.Format(
                TAB3 + "if (sql.CountRowEqual(vo.{0}) == 0)\n" +
                TAB4 + "sql.Add(vo);\n" +
                TAB3 + "else\n" +
                TAB4 + "sql.Edit(vo);\n" +
                TAB2 + "}}\n\n",
                vars[0].Name
                );

            // Ham 6 Edit du lieu
            s += TAB2 + "// Sua du lieu\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "void", "Edit", "VO_" + nameTable, "vo");
            s += TAB3 + "sql.Edit(vo);\n" +
              TAB2 + "}\n\n";

            // Ham 7 Delete du lieu
            s += TAB2 + "// Delete du lieu\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "void", "Delete", "VO_" + nameTable, "vo");
            s += TAB3 + "sql.Delete(vo);\n" +
             TAB2 + "}\n\n";

            // Ham 8 Dem so luong ban ghi
            s += TAB2 + "// Dem so luong ban ghi\n";
            s += TAB2 + string.Format("public {0} {1}()\n\t\t{{\n", "int", "CountRow");
            s += TAB3 + "return sql.CountRow();\n" +
             TAB2 + "}\n\n";

            // Ham 9 Dem so luong ban ghi = condition
            s += TAB2 + "// Dem so luong ban ghi = condition\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "int", "CountRowEqual", "string", "condition");
            s += TAB3 + "return sql.CountRowEqual(condition);\n" +
             TAB2 + "}\n\n";

            // Ham 10 Dem so luong ban ghi >= condition
            s += TAB2 + "// Dem so luong ban ghi >= condition\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "int", "CountRowGreat", "string", "condition");
            s += TAB3 + "return sql.CountRowGreat(condition);\n" +
              TAB2 + "}\n\n";

            // Ham 11 Get row id
            s += TAB2 + "// Get row id\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3})\n\t\t{{\n", "string", "GetRowId", "int", "row");
            s += TAB3 + "return sql.GetRowId(row);\n" +
              TAB2 + "}\n\n";

            // Ham 12 Tinh ID moi
            s += TAB2 + "// Ham 12 Tinh ID moi\n";
            s += TAB2 + string.Format("public {0} {1}({2} {3}, {4} {5})\n\t\t{{\n", "void", "TinhId", "ref string", "id", "ref string", "id_setup");

            s += TAB3 + "int count = sql.CountRowGreat(id_setup);\n\n";
            s += TAB3 + "string data_max = \"\";\n\n";
            s += TAB3 + "if(count != 0)\n" + TAB3 + "{\n";
            s += TAB4 + "string[] data_id = new string[count];\n\n";
            s += TAB4 + "for (int i = 0; i < count; i++)\n" + TAB4 + "{\n";
            s += TAB5 + "data_id[i] = sql.GetRowId(i);\n" + TAB4 + "}\n\n";
            s += TAB4 + "data_max = data_id[count - 1];\n\n";
            s += TAB4 + "if(string.Equals(id_setup, data_id[0]))\n" + TAB4 + "{\n";
            s += TAB5 + "if(string.Equals(id_setup, data_max))\n";
            s += TAB6 + "id = (int.Parse(id_setup) + 1).ToString();\n";
            s += TAB5 + "else\n" + TAB5 + "{\n";
            s += TAB6 + "for (int i = 0; i < count - 1; i++)\n" + TAB6 + "{\n";
            s += TAB7 + "if ((int.Parse(data_id[i + 1]) - int.Parse(data_id[i])) != 1)\n" + TAB7 + "{\n";
            s += TAB8 + "id = (int.Parse(data_id[i]) + 1).ToString();\n" + TAB8 + "break;\n" + TAB7 + "}\n\n";
            s += TAB7 + "id = (int.Parse(data_max) + 1).ToString();\n" + TAB6 + "}\n\n" + TAB5 + "}\n" + TAB4 + "}\n";
            s += TAB4 + "else\n" + TAB4 + "{\n";
            s += TAB5 + "id = id_setup;\n" + TAB4 + "}\n\n";
            s += TAB4 + "data_id = null; // Giai phong bo nho\n" + TAB3 + "}\n";
            s += TAB3 + "else\n" + TAB3 + "{\n";
            s += TAB4 + "id = id_setup;\n" + TAB3 + "}\n\n";

            s += "\t\t}\n\n";
            // Ket thuc
            SetString(richTextBox2, s + "\t}\n}\n");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult s = MessageBox.Show("Save to file?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (s == MessageBoxResult.No) return;

            string nameTable = "";
            char[] t = txtClass.Text.Trim().ToCharArray();
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == '_')
                {
                    nameTable = new string(t, i + 1, t.Length - 1 - i);
                    break;
                }
            }

            if (nameTable == "") return;


            /// 
            btnGenVO_Click(this, e);
            FileStream fs = new FileStream("VO_" + nameTable + ".cs", FileMode.Create);
            TextRange tr = new TextRange(richTextBox2.Document.ContentStart, richTextBox2.Document.ContentEnd);
            tr.Save(fs, System.Windows.DataFormats.Text);
            fs.Close();

            ///
            btnGenSQL_Click(this, e);
            fs = new FileStream("SQL_" + nameTable + ".cs", FileMode.Create);
            tr = new TextRange(richTextBox2.Document.ContentStart, richTextBox2.Document.ContentEnd);
            tr.Save(fs, System.Windows.DataFormats.Text);
            fs.Close();

            ///
            btnGenBUS_Click(this, e);
            fs = new FileStream("BUS_" + nameTable + ".cs", FileMode.Create);
            tr = new TextRange(richTextBox2.Document.ContentStart, richTextBox2.Document.ContentEnd);
            tr.Save(fs, System.Windows.DataFormats.Text);
            fs.Close();

            /// 
            btnDatabaseAccess_Click(this, e);
            fs = new FileStream("DatabaseAccess.cs", FileMode.Create);
            tr = new TextRange(richTextBox2.Document.ContentStart, richTextBox2.Document.ContentEnd);
            tr.Save(fs, System.Windows.DataFormats.Text);
            fs.Close();

            ///
            btnAppConfig_Click(this, e);
            fs = new FileStream("App.config", FileMode.Create);
            tr = new TextRange(richTextBox2.Document.ContentStart, richTextBox2.Document.ContentEnd);
            tr.Save(fs, System.Windows.DataFormats.Text);
            fs.Close();


        }

        private void btnAppConfig_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNameSpace.Text;
            string s1 = "";
            foreach (string t in name.Split('_'))
            {
                s1 += t.ToLower();
            }

            //
            string s = "<?xml version = \"1.0\" encoding = \"utf-8\" ?>\n";
            s += "<configuration>\n";
            s += TAB1 + "<startup>\n";
            s += TAB2 + "<supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.5.3\" />\n";
            s += TAB1 + "</startup>\n";
            s += TAB1 + "<connectionStrings>\n";
            s += TAB2 + "<add name=\"strConnection\" connectionString=\"server = localhost; port = 3310; user = root; password =; database = " + s1 + "; charset = utf8\"/>\n";
            s += TAB1 + "</connectionStrings>\n\n";
            s += "</configuration>";

            // Ket thuc
            SetString(richTextBox2, s);
        }

        private void btnDatabaseAccess_Click(object sender, RoutedEventArgs e)
        {
            //
            string s = "using System;\n";
            s += "using MySql.Data.MySqlClient; // Ket noi Mysql\n";
            s += "using System.Configuration; // get ConfigurationManager\n";
            s += "using System.Data; // Dataset\n";
            s += "using System.Windows; // MessageBox\n\n";

            s += "namespace " + txtNameSpace.Text + "\n{\n";
            s += TAB1 + "public class DatabaseAccess\n" + TAB1 + "{\n";

            // Ham 0 tao chuoi ket noi
            s += TAB2 + "// Connection strings\n";
            s += TAB2 + "private MySqlConnection _conn;\n\n";

            s += TAB2 + "// Constructor\n";
            s += TAB2 + "public DatabaseAccess()\n" + TAB2 + "{\n";
            s += TAB3 + "_conn = new MySqlConnection(ConfigurationManager.ConnectionStrings[\"strConnection\"].ConnectionString);\n";
            s += TAB2 + "}\n\n";

            s += TAB2 + "// Properties\n";
            s += TAB2 + " public  MySqlConnection Conn\n" + TAB2 + "{\n";
            s += TAB3 + "get { return _conn; }\n";
            s += TAB2 + "}\n\n";

            // Ham 1
            s += TAB2 + "// Open Connection\n";
            s += TAB2 + "private MySqlConnection OpenConnection()\n" + TAB2 + "{\n";
            s += TAB3 + "if (_conn.State == ConnectionState.Closed || _conn.State == ConnectionState.Broken)\n";
            s += TAB4 + "_conn.Open();\n";
            s += TAB3 + "return _conn;\n" + TAB2 + "}\n\n";

            // Ham 2
            s += TAB2 + "// Close Connection\n";
            s += TAB2 + "private MySqlConnection CloseConnection()\n" + TAB2 + "{\n";
            s += TAB3 + "if (_conn.State == ConnectionState.Open)\n";
            s += TAB4 + "_conn.Close();\n";
            s += TAB3 + "return _conn;\n" + TAB2 + "}\n\n";

            // Ham 3 
            s += TAB2 + "// Run Select Command\n";
            s += TAB2 + "public DataSet executeQuery(MySqlCommand cmd)\n" + TAB2 + "{\n\n";
            s += TAB3 + "DataSet ds = new DataSet();\n\n";
            s += TAB3 + "// Mo ket noi\n";
            s += TAB3 + "OpenConnection();\n\n";
            s += TAB3 + "// Chay lenh cmd dua du lieu vao dataset\n";
            s += TAB3 + "MySqlDataAdapter da = new MySqlDataAdapter(cmd);\n\n";
            s += TAB3 + "try\n" + TAB3 + "{\n";
            s += TAB4 + "da.Fill(ds);\n" + TAB3 + "}\n";
            s += TAB3 + "catch (MySqlException ex)\n" + TAB3 + "{\n";
            s += TAB4 + "MessageBox.Show(\"Kiem tra loi: \" + ex.ErrorCode);\n" + TAB3 + "}\n\n";

            s += TAB3 + "// Dong ket noi\n";
            s += TAB3 + "CloseConnection();\n\n";

            s += TAB3 + "// return gia tri\n";
            s += TAB3 + "return ds;\n" + TAB2 + "}\n\n";

            // Ham 4
            s += TAB2 + "// Run Insert, Edit, Delete Command\n";
            s += TAB2 + "public void executeNonQuery(MySqlCommand cmd)\n" + TAB2 + "{\n\n";
            s += TAB3 + "// Mo ket noi\n";
            s += TAB3 + "OpenConnection();\n\n";
            s += TAB3 + "// Chay lenh\n";
            s += TAB3 + "try\n" + TAB3 + "{\n";
            s += TAB4 + "cmd.ExecuteNonQuery();\n" + TAB3 + "}\n";
            s += TAB3 + "catch (MySqlException ex)\n" + TAB3 + "{\n";
            s += TAB4 + "MessageBox.Show(\"Kiem tra loi: \" + ex.ErrorCode);\n" + TAB3 + "}\n\n";

            s += TAB3 + "// Dong ket noi\n";
            s += TAB3 + "CloseConnection();\n" + TAB2 + "}\n\n";

            //
            s += TAB1 + "}\n}\n";

            // Ket thuc
            SetString(richTextBox2, s);
        }
    }
}

class Variable
{
    string _name;
    string _nameH;
    string _type;
    string _typeH;


    string _attr;
    string _var;

    public Variable(string type, string name)
    {
        _name = name;
        _type = type;

        _attr = string.Format("_{0}{1}", char.ToLower(name[0]), name.Substring(1));
        _var = string.Format("{0}{1}", char.ToLower(name[0]), name.Substring(1));

        _typeH = string.Format("{0}{1}", char.ToUpper(type[0]), type.Substring(1));
        NameH = string.Format("{0}{1}", char.ToUpper(name[0]), name.Substring(1));
        if (type == "byte[]") _typeH = "LongBlob";
        if (type == "int") _typeH = "Int32";

        //  if(type == )

    }
    public string Declare
    {
        get { return "\t\t" + _type + " " + _attr + ";\n"; }
    }

    public string GetSet
    {
        get
        {
            return string.Format(
                "\n\t\tpublic {0} {1}\n\t\t{{\n" +
                "\t\t\tget {{ return {2}; }}\n" +
                "\t\t\tset {{ {2} = value; }}\n\t\t}}\n",
                _type, Name, _attr
                );
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    public string Var
    {
        get
        {
            return _var;
        }

        set
        {
            _var = value;
        }
    }

    public string Type
    {
        get { return _type; }
        set { _type = value; }
    }

    public string TypeH
    {
        get { return _typeH; }
        set { _typeH = value; }
    }

    public string NameH { get => _nameH; set => _nameH = value; }
}

