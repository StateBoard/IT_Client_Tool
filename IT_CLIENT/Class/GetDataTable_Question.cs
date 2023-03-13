using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace IT_CLIENT.Class
{
    class GetDataTable_Question
    {
        public DataTable Get_First_Question()
        {
            DataTable table = new DataTable();
            table.Columns.Add("QNO", typeof(int));
            table.Columns.Add("Question", typeof(string));           
            table.Columns.Add("Answer", typeof(string));
            return table;
        }
    }
}
