//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CDB_SZFPZ.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AktivanClan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AktivanClan()
        {
            this.ClanFROdboras = new HashSet<ClanFROdbora>();
        }
    
        public int IDAktivan { get; set; }
        public string Aktivan { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClanFROdbora> ClanFROdboras { get; set; }
    }
}