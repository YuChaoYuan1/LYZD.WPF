using LYZD.DAL;
using LYZD.ViewModel.CodeTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LYZD.WPF.View.Windows
{
    /// <summary>
    /// Window_AddNewCode.xaml 的交互逻辑
    /// </summary>
    public partial class Window_AddNewCode : Window
    {
        public Window_AddNewCode(CodeTreeNode CodeNode)
        {
            InitializeComponent();
            nodeTemp = CodeNode;
            DataContext = CodeNode;
        }


        private CodeTreeNode nodeTemp { get; set; }

        private void Click_Enter(object sender, RoutedEventArgs e)
        {
            if (CheckAndSaveCode())
            {
                DialogResult = true;
                Close();
            }
        }
        private bool CheckAndSaveCode()
        {
            string nameTemp = textBoxName.Text;
            string valueTemp = textBoxValue.Text;
            int intTemp = 0;

            if (string.IsNullOrEmpty(nameTemp) || string.IsNullOrEmpty(valueTemp))
            {
                MessageBox.Show("新添加的编码名称或值不能为空!!");
            }

             if (!int.TryParse(valueTemp, out intTemp))
            {
                MessageBox.Show("新添加的编码值必须为数字!!");
            }
            else
            {
                // CodeTreeNode 123 = nodeTemp.Children.FirstOrDefault(item => item. == nameTemp);
                CodeTreeNode nodeTemp1 = nodeTemp.Children.FirstOrDefault(item => item.CODE_NAME == nameTemp);
                CodeTreeNode nodeTemp2 = nodeTemp.Children.FirstOrDefault(item => item.CODE_VALUE == valueTemp);
                if (nodeTemp1 != null || nodeTemp2 != null)
                {
                    MessageBox.Show("编码名称或编码值已经存在,请输入新的值!!");
                }
                else
                {
                    nodeTemp.Children.Add(new CodeTreeNode()
                    {
                        CODE_LEVEL = nodeTemp.CODE_LEVEL + 1,
                        CODE_PARENT = nodeTemp.CODE_TYPE,
                        CODE_NAME = nameTemp,
                        CODE_VALUE = valueTemp,
                        CODE_ENABLED = true,
                        FlagChanged = true
                    });
                    nodeTemp.SaveCode();
                    Dictionary<string, string> dictionary = CodeDictionary.GetLayer2(nodeTemp.CODE_TYPE);
                    dictionary.Add(nameTemp, valueTemp);
                    return true;
                }
            }
            return false;
        }

        private void Click_Quit(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
