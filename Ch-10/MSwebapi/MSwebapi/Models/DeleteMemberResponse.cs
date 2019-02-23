using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mswebapi.Models
{
    public class DeleteMemberResponse
    {
        public bool ok { get; set; }
        public string errMsg { get; set; }

        public DeleteMemberResponse()
        {
            this.ok = true;
            this.errMsg = "";
        }
    }
}
