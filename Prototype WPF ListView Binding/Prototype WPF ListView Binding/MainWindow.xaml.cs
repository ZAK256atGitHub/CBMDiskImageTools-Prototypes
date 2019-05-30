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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data;

namespace Prototype_WPF_ListView_Binding
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListViewData listViewData = new ListViewData();
        public MainWindow()
        {
            InitializeComponent();

            GridView gv = new GridView();
            foreach (DataColumn c in listViewData.dirEntrys.Columns)
            {
                GridViewColumn gvColumn = new GridViewColumn();
                gvColumn.DisplayMemberBinding = new Binding(c.ColumnName);
                gvColumn.Header = c.ColumnName;
                gv.Columns.Add(gvColumn);
            }
            dirEntryListview.View = gv;
            dirEntryListview.DataContext = listViewData.dirEntrys;
            Binding bind = new Binding();
            dirEntryListview.SetBinding(ListView.ItemsSourceProperty, bind);
        }
    }
    public class ListViewData
    {
        private DataTable _dirEntrys;
        public ListViewData()
        {
            createData();
        }
        public DataTable dirEntrys
        {
            get { return _dirEntrys; }
        }
        private void createData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Size");
            dt.Columns.Add("Name");
            dt.Columns.Add("Type");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("1", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            dt.Rows.Add("1", "", " PRG<");
            dt.Rows.Add("0", "", " PRG<");
            _dirEntrys = dt;
        }
    }
}
