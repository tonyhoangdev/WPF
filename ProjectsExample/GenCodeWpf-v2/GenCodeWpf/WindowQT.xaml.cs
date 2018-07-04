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
using System.Windows.Shapes;

namespace GenCodeWpf
{
    /// <summary>
    /// Interaction logic for WindowQT.xaml
    /// </summary>
    public partial class WindowQT : Window
    {
        public WindowQT()
        {
            InitializeComponent();
        }

        private string GetString(RichTextBox rtb)
        {
            return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
        }

        private void SetString(RichTextBox rtb, string s)
        {
            rtb.Document.Blocks.Clear();
            rtb.Document.Blocks.Add(new Paragraph(new Run(s)));
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

        private string TAB(int n)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                sb.Append("    ");
            }

            return sb.ToString();
        }

        private void btnGenVO_Click(object sender, RoutedEventArgs e)
        {
            string rtb1 = GetString(richTextBox1);
            List<Variable> vars = GetVars(rtb1);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("#ifndef {0}_H", txtClass.Text.ToUpper()));
            sb.AppendLine(String.Format("#define {0}_H", txtClass.Text.ToUpper()));
            sb.AppendLine();
            sb.AppendLine(String.Format("#include <QObject>"));
            sb.AppendLine(String.Format("#include \"LogContext.h\""));
            sb.AppendLine();
            sb.AppendLine(String.Format("class {0} : public QObject", txtClass.Text));
            sb.AppendLine("{");
            sb.AppendLine(TAB(1) + "Q_OBJECT");

            // Properties declare
            foreach (Variable var in vars)
            {
                if (!var.IsPointer)
                {
                    sb.AppendLine(TAB(1) + String.Format("Q_PROPERTY({0} {1} READ {1} WRITE set{2} NOTIFY {1}Changed)", var.Type, var.Name, var.NameH));
                }
                else
                {
                    sb.AppendLine(TAB(1) + String.Format("Q_PROPERTY({0} {1} READ {1} CONSTANT)", var.Type, var.Name));
                }
            }

            // Public
            sb.AppendLine();
            sb.AppendLine("public:");
            sb.AppendLine(TAB(1) + String.Format("explicit {0}(QObject *parent = nullptr);", txtClass.Text));
            sb.AppendLine(TAB(1) + String.Format("~{0}();", txtClass.Text));

            // Get funcs
            sb.AppendLine();
            foreach (Variable var in vars)
            {
                sb.AppendLine(TAB(1) + String.Format("{0} {1}() const;", var.Type, var.Name));
            }

            // Public slots
            sb.AppendLine();
            sb.AppendLine("public slots:");
            foreach (Variable var in vars)
            {
                if (!var.IsPointer)
                {
                    sb.AppendLine(TAB(1) + String.Format("void set{0}({1} {2});", var.NameH, var.Type, var.Name));
                }
            }

            // Signals
            sb.AppendLine();
            sb.AppendLine("signals:");
            foreach (Variable var in vars)
            {
                if (!var.IsPointer)
                {
                    sb.AppendLine(TAB(1) + String.Format("void {0}Changed({1} {0});", var.Name, var.Type));
                }
            }

            // Variables private
            sb.AppendLine();
            sb.AppendLine("private:");
            foreach (Variable var in vars)
            {
                if (!var.IsPointer)
                {
                    sb.AppendLine(TAB(1) + String.Format("{0} m_{1};", var.Type, var.Name));
                }
            }

            // Variables public
            foreach (Variable var in vars)
            {
                if (var.IsPointer)
                {
                    sb.AppendLine();
                    sb.AppendLine("public:");
                    break;
                }
            }
            foreach (Variable var in vars)
            {
                if (var.IsPointer)
                {
                    sb.AppendLine(TAB(1) + String.Format("{0} m_{1};", var.Type, var.Name));
                }
            }


            // Ket thuc
            sb.AppendLine("};");
            sb.AppendLine();
            sb.AppendLine(String.Format("#endif // {0}_H", txtClass.Text.ToUpper()));
            SetString(richTextBox2, sb.ToString());
        }

        private string initVariable(string type, bool isPointer = false)
        {
            string s = "";

            switch (type)
            {
                case "bool":
                    s = "false";
                    break;

                default:
                    s = "0";
                    break;
            }

            if (isPointer)
            {
                s = type.Substring(0, type.IndexOf("*"));
            }

            return s;
        }

        private void btnGenSQL_Click(object sender, RoutedEventArgs e)
        {
            string rtb1 = GetString(richTextBox1);
            List<Variable> vars = GetVars(rtb1);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("#include \"{0}.h\"", txtClass.Text));
            sb.AppendLine();

            sb.AppendLine("////////////////////////////////////////////////////////////");
            sb.Append("// ");
            sb.AppendLine(txtClass.Text);
            sb.AppendLine("////////////////////////////////////////////////////////////");

            // constructor
            sb.Append(String.Format("{0}::{0}(QObject *parent) : QObject(parent)", txtClass.Text));

            foreach (Variable var in vars)
            {
                if (!var.IsPointer)
                {
                    sb.AppendLine(",");
                    sb.Append(TAB(1) + String.Format("m_{0}({1})", var.Name, initVariable(var.Type, var.IsPointer)));
                }
            }
            foreach (Variable var in vars)
            {
                if (var.IsPointer)
                {
                    sb.AppendLine(",");
                    sb.Append(TAB(1) + String.Format("m_{0}(new {1}())", var.Name, initVariable(var.Type, var.IsPointer)));
                }
            }

            sb.AppendLine();
            sb.AppendLine("{");
            sb.AppendLine("}");

            // de-constructor
            sb.AppendLine();
            sb.AppendLine(String.Format("{0}::~{0}()", txtClass.Text));
            sb.AppendLine("{");

            foreach (Variable var in vars)
            {
                if (var.IsPointer)
                {
                    sb.AppendLine(TAB(1) + String.Format("if (m_{0} != nullptr)", var.Name));
                    sb.AppendLine(TAB(1) + "{");
                    sb.AppendLine(TAB(2) + String.Format("delete m_{0};", var.Name));
                    sb.AppendLine(TAB(1) + "}");
                }
            }
            sb.AppendLine("}");

            // get funcs
            foreach (Variable var in vars)
            {
                sb.AppendLine();
                sb.AppendLine(String.Format("{0} {1}::{2}() const", var.Type, txtClass.Text, var.Name));
                sb.AppendLine("{");
                sb.AppendLine(TAB(1) + String.Format("return m_{0};", var.Name));
                sb.AppendLine("}");

                if (!var.IsPointer)
                {
                    sb.AppendLine();
                    sb.AppendLine(String.Format("void {0}::set{1}({2} {3})", txtClass.Text, var.NameH, var.Type, var.Name));
                    sb.AppendLine("{");
                    sb.AppendLine(TAB(1) + String.Format("if (m_{0} != {0})", var.Name));
                    sb.AppendLine(TAB(1) + "{");
                    sb.AppendLine(TAB(2) + String.Format("m_{0} = {0};", var.Name));
                    sb.AppendLine(TAB(2) + String.Format("emit {0}Changed(m_{0});", var.Name));
                    sb.AppendLine(TAB(1) + "}");
                    sb.AppendLine("}");
                }

            }

            // Ket thuc
            SetString(richTextBox2, sb.ToString());
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult s = MessageBox.Show("Save to file?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (s == MessageBoxResult.No) return;

            /// 
            btnGenVO_Click(this, e);
            FileStream fs = new FileStream(txtClass.Text.Trim() + ".h", FileMode.Create);
            TextRange tr = new TextRange(richTextBox2.Document.ContentStart, richTextBox2.Document.ContentEnd);
            tr.Save(fs, System.Windows.DataFormats.Text);
            fs.Close();

            ///
            btnGenSQL_Click(this, e);
            fs = new FileStream(txtClass.Text.Trim() + ".cpp", FileMode.Create);
            tr = new TextRange(richTextBox2.Document.ContentStart, richTextBox2.Document.ContentEnd);
            tr.Save(fs, System.Windows.DataFormats.Text);
            fs.Close();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            richTextBox2.SelectAll();
            richTextBox2.Copy();
        }
    }
}
