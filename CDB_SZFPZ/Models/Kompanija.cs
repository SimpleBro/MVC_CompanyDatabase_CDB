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
    
    public partial class Kompanija
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kompanija()
        {
            this.KontaktKompanijes = new HashSet<KontaktKompanije>();
            this.SharingIsCarings = new HashSet<SharingIsCaring>();
            this.Suradnjas = new HashSet<Suradnja>();
            this.Uploaded_File = new HashSet<Uploaded_File>();
        }
    
        public int IDKompanija { get; set; }
        public string NazivKompanija { get; set; }
        public string OpisKompanija { get; set; }
        public string AdresaKompanija { get; set; }
        public string PoštanskiBrojKompanija { get; set; }
        public string DrzavaKompanija { get; set; }
        public string GradKompanija { get; set; }
        public string TelefonKompanija { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KontaktKompanije> KontaktKompanijes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SharingIsCaring> SharingIsCarings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Suradnja> Suradnjas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Uploaded_File> Uploaded_File { get; set; }
    }
}
