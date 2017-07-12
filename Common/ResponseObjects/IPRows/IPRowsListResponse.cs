using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ResponseObjects.IPRows
{
    public class IPRowsListResponse : ResponseBase
    {
        public IEnumerable<ServiceRow> Rows { get; set; } 
        protected IPRowsListResponse(IEnumerable<ServiceRow> rows) :base()
        {
            Rows = rows;
        }

        public static IPRowsListResponse Create(IEnumerable<ServiceRow> rows) =>
            new IPRowsListResponse(rows);
    }
}
