//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace sln_SingleApartment.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Lease
    {
        public int ID { get; set; }
        public Nullable<int> RoomID { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<int> PersonalRent { get; set; }
        public Nullable<int> MemberID { get; set; }
    
        public virtual Room Room { get; set; }
        public virtual tMember tMember { get; set; }
    }
}
