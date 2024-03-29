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
    
    public partial class Uploaded_File
    {
        public int ID_File { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public string Content_Type { get; set; }
        public string File_Extension { get; set; }
        public byte[] File_Content { get; set; }
        public Nullable<System.DateTime> Upload_Date { get; set; }
        public Nullable<int> Clan { get; set; }
        public Nullable<int> Projekt { get; set; }
        public Nullable<int> SIC { get; set; }
        public Nullable<int> Kompanija { get; set; }
        public Nullable<int> Kontakt { get; set; }
        public Nullable<int> Suradnja { get; set; }
    
        public virtual ClanFROdbora ClanFROdbora { get; set; }
        public virtual Kompanija Kompanija1 { get; set; }
        public virtual KontaktKompanije KontaktKompanije { get; set; }
        public virtual Projekt Projekt1 { get; set; }
        public virtual SharingIsCaring SharingIsCaring { get; set; }
        public virtual Suradnja Suradnja1 { get; set; }
    }
}
