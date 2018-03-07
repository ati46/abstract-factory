using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UtilHelper
{
    public static object FromDbValue(object value)
    {
        if (value == DBNull.Value)
        {
            return null;
        }
        else
        {
            return value;
        }
    }

    public static object ToDbValue(object value)
    {
        if (value == null)
        {
            return DBNull.Value;
        }
        else
        {
            return value;
        }
    }
}

