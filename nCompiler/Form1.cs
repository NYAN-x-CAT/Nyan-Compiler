using FastColoredTextBoxNS;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Windows.Forms;

namespace nCompiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboLang.SelectedIndex = 0;
            comboType.SelectedIndex = 0;
            comboFrame.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxReferences.Items.Count == 0)
                {
                    MessageBox.Show("No references!", "Nyan Compiler", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBox.Text))
                {
                    MessageBox.Show("Empty code!", "Nyan Compiler", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                using (SaveFileDialog saveFile = new SaveFileDialog())
                {
                    if (comboType.SelectedIndex == 0)
                        saveFile.Filter = "Executable (*.exe)|*.exe";
                    else
                        saveFile.Filter = "Library (*.dll)|*.dll";

                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        switch (comboLang.Text)
                        {
                            case "C#":
                                {
                                    Compiler(new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", comboFrame.Text } }),
                                        txtBox.Text, GetReference(),
                                        saveFile.FileName);
                                    break;
                                }

                            case "VB.NET":
                                {
                                    Compiler(new VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", comboFrame.Text } }),
                                        txtBox.Text, GetReference(),
                                        saveFile.FileName);
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nyan Compiler", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private string[] GetReference()
        {
            List<string> reference = new List<string>();
            foreach (string r in listBoxReferences.Items)
            {
                reference.Add(r);
            }
            return reference.ToArray();
        }

        private void Compiler(CodeDomProvider codeDomProvider, string source, string[] referencedAssemblies, string output)
        {
            try
            {
                var compilerOptions = $"/target:{comboType.Text} /platform:anycpu /optimize+";

                var compilerParameters = new CompilerParameters(referencedAssemblies)
                {
                    GenerateExecutable = true,
                    GenerateInMemory = false,
                    CompilerOptions = compilerOptions,
                    TreatWarningsAsErrors = false,
                    IncludeDebugInformation = false,
                    OutputAssembly = output,
                };
                var compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, source);

                if (compilerResults.Errors.Count > 0)
                {
                    foreach (CompilerError compilerError in compilerResults.Errors)
                    {
                        throw new Exception(string.Format("{0}\nLine: {1}", compilerError.ErrorText, compilerError.Line));
                    }
                }
                else
                {
                    MessageBox.Show("Done!", "Nyan Compiler", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string reference = Interaction.InputBox("Add Reference", "References", "");
            if (string.IsNullOrEmpty(reference))
                return;
            else
            {
                foreach (string item in listBoxReferences.Items)
                {
                    if (item == reference)
                    {
                        return;
                    }
                }
                listBoxReferences.Items.Add(reference);
            }
        }

        private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxReferences.SelectedItems.Count == 1)
            {
                listBoxReferences.Items.Remove(listBoxReferences.SelectedItem);
            }
        }

        private void ComboLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboLang.SelectedIndex == 0)
            {
                txtBox.Language = Language.CSharp;
                txtBox.Text = txtBox.Text = @"// github.com/NYAN-x-CAT

using System;
using System.Windows.Forms;

namespace NyanCompiler
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                MessageBox.Show(""Hello World"");
            }
            catch { }
        }
    }
}";
            }
            else
            {
                txtBox.Language = Language.VB;
                txtBox.Text = @"' github.com/NYAN-x-CAT

Imports System
Imports System.Windows.Forms

    Public Class Program
        Public Shared Sub Main()
            Try
                MessageBox.Show(""Hello World"")
            Catch
            End Try
        End Sub
    End Class

";
            }
        }

        private void WordWraoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtBox.WordWrap)
            {
                txtBox.WordWrap = false;
                wordWraoToolStripMenuItem.Checked = false;
            }
            else
            {
                txtBox.WordWrap = true;
                wordWraoToolStripMenuItem.Checked = true;
            }
        }
    }
}
