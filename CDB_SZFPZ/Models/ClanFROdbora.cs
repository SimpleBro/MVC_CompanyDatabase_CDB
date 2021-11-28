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
    
    public partial class ClanFROdbora
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClanFROdbora()
        {
            this.SharingIsCarings = new HashSet<SharingIsCaring>();
            this.Suradnjas = new HashSet<Suradnja>();
            this.Uploaded_File = new HashSet<Uploaded_File>();
        }
    
        public int IDClan { get; set; }
        public string ImeClana { get; set; }
        public string PrezimeClana { get; set; }
        public string EmailClana { get; set; }
        public string MobitelKontakt { get; set; }
        public Nullable<int> ModulClana { get; set; }
        public string KorisnickoImeClana { get; set; }
        public string LozinkaClana { get; set; }
        public int Uloga { get; set; }
        public int Aktivan { get; set; }
        public string ImePrezime { get; set; }
    
        public virtual AktivanClan AktivanClan { get; set; }
        public virtual ModuliFPZ ModuliFPZ { get; set; }
        public virtual UlogaUSustavu UlogaUSustavu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SharingIsCaring> SharingIsCarings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Suradnja> Suradnjas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Uploaded_File> Uploaded_File { get; set; }
    }
}