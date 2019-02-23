using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mswebapi.Models
{
    public class GetMemberListResponse
    {
        public bool ok { get; set; }
        public string errMsg { get; set; }
        public List<MemberInfo> list { get; set; }

        public GetMemberListResponse()
        {
            this.ok = true;
            this.errMsg = "";
            this.list = new List<MemberInfo>();
        }
    }

    public class MemberInfo
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }
}
