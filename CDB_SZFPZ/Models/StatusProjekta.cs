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
    
    public partial class StatusProjekta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StatusProjekta()
        {
            this.Projekts = new HashSet<Projekt>();
        }
    
        public int IDStatusProj { get; set; }
        public string NazivStatProj { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Projekt> Projekts { get; set; }
    }
}
