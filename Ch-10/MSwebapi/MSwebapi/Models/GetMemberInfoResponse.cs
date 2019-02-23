using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mswebapi.Models
{
    public class GetMemberInfoResponse
    {
        public bool ok { get; set; }
        public string errMsg { get; set; }
        public MemberInfo data { get; set; }

        public GetMemberInfoResponse()
        {
            this.ok = true;
            this.errMsg = "";
            this.data = new MemberInfo();
        }
    }
}
