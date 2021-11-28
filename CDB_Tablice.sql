create table StatusProjekta
(
	IDStatusProj int not null primary key identity(1,1),
	NazivStatProj nvarchar(50) not null
)

insert into dbo.StatusProjekta values ('U tijeku'),('Zavrsen'),('Odgoden')

create table PrioritetProjekta
(
	IDPrior int not null primary key identity(1,1),
	RazinaPrioriteta nvarchar(50)
)

insert into dbo.PrioritetProjekta values ('Critical'),('High'),('Medium'),('Low')

create table Projekt
(
	IDProjekt int not null primary key identity(1,1),
	NazivProjekt nvarchar(300) not null,
	OpisProjekt nvarchar(500) null,
	WebAdresaProjekta nvarchar(300) null,
	GodinaProjekt datetime null,
	CiljFRFinancije int null,
	CiljFRMaterijali nvarchar(max) null,
	StatusProjekta int not null foreign key references StatusProjekta(IDStatusProj), -- ('U tijeku'),('Zavrsen'),('Odgoden')
	PrioritetProjekta int not null foreign key references PrioritetProjekta(IDPrior) -- ('Critical'),('High'),('Medium'),('Low')
)

insert into dbo.Projekt values ('Test123','Testni projekt','tt@fpz.hr','2020','1','Dva kubika žala',1,1)

alter table Projekt
	ALTER COLUMN OpisProjekt nvarchar(Max) null 

create table Kompanija
(
	IDKompanija int not null primary key identity(1,1),
	NazivKompanija nvarchar(300) not null,
	OpisKompanija nvarchar(500) null,
	AdresaKompanija nvarchar(500) null,
	PoštanskiBrojKompanija nvarchar(50) null,
	DrzavaKompanija nvarchar(50) null,
	GradKompanija nvarchar(50) null,
	TelefonKompanija nvarchar(50) null
)

insert into dbo.Kompanija values ('Fakultet Prometnih Znanosti','Alma mater','Borongajska cesta','1000','Hrvatska','Zagreb','01 2457 740')

--select * from dbo.Kompanija

alter table Kompanija
	ALTER COLUMN OpisKompanija nvarchar(Max) null

create table KontaktKompanije
(
	IDKontakt int not null primary key identity(1,1),
	ImeKontakt nvarchar(50) not null,
	PrezimeKontakt nvarchar(50) not null,
	EmailKontakt nvarchar(50) not null,
	TelefonKontakt nvarchar(50) null,
	MobitelKontakt nvarchar(50) null,
	FunkcijaUKompaniji nvarchar(100) not null,
	OpisKontakta nvarchar(max) null, -- Sto ju/ga pali, na sto pada. Da li je dobar/los/zao. Slabosti/Prednosti,
	ZaposlenValidan bit not null default(1), -- Jos uvijek zaposlen u firmi
	IDKompanijaKontakta int not null foreign key references Kompanija(IDKompanija),
	ImePrezimeKontakta nvarchar(100)
)
--select IDKompanija from dbo.Kompanija where NazivKompanija = 'Fakultet Prometnih Znanosti'
insert into dbo.KontaktKompanije values ('Ured','Dekana','fpz@fpz.unizg.hr','+385 1 238 0350','','Dekan','Supreme commander',1,(select IDKompanija from dbo.Kompanija where NazivKompanija = 'Fakultet Prometnih Znanosti'),'Ured Dekana')
insert into dbo.KontaktKompanije values ('Studentska','služba','referada@fpz.unizg.hr','+385 1 238 0202','','Referada','Haron na rijeci Stiks',1,(select IDKompanija from dbo.Kompanija where NazivKompanija = 'Fakultet Prometnih Znanosti'),'Studentska služba')

--alter table KontaktKompanije
--	add ImePrezimeKontakta nvarchar(100)

create table StatusSuradnja -- ('Kontaktirani'),('Kontaktiraj ponovno'),('Poslan dopis'),('Dogovoren sastanak'),('Uspjesno'),('Neuspjesno'),('Neizvjesno')
(
	IDStatusSuradnja int not null primary key identity(1,1),
	StatusNaziv nvarchar(50) not null
)

insert into dbo.StatusSuradnja values ('Kontaktirani'),('Kontaktiraj ponovno'),('Poslan dopis'),('Dogovoren sastanak'),('Uspjesno'),('Neuspjesno'),('Neizvjesno')

create table ModuliFPZ -- IKP/ITS/...
(
	IDModula int not null primary key identity(1,1),
	NazivModula nvarchar(100) not null
)

insert into dbo.ModuliFPZ values ('ITS'),('IK'),('Logistika'),('Cestovni'),('Gradski'),('Postanski'),('Vodni'),('Zracni'),('Kontrola leta'),('Civilni pilot'),('Vojni pilot')

create table UlogaUSustavu -- Admin/Korisnik -- admin moze brisati korisnike/stvarati nove admine.
(
	IDUUS int not null primary key identity(1,1),
	NazivUloge nvarchar(50) not null
)

insert into dbo.UlogaUSustavu values ('Admin'),('Clan')

create table AktivanClan
(
	IDAktivan int not null primary key identity(1,1),
	Aktivan nvarchar(2) not null
)

insert into dbo.AktivanClan values('Da'),('Ne')

create table ClanFROdbora -- Ja/Ti/On...
(
	IDClan int not null primary key identity(1,1),
	ImeClana nvarchar(50) not null,
	PrezimeClana nvarchar(50) not null,
	EmailClana nvarchar(50) not null,
	MobitelKontakt nvarchar(50) null,
	ModulClana int null foreign key references ModuliFPZ(IDModula),
	KorisnickoImeClana nvarchar(20) not null,
	LozinkaClana nvarchar(20) not null unique,
	Uloga int not null foreign key references UlogaUSustavu(IDUUS),
	Aktivan int not null foreign key references AktivanClan(IDAktivan),
	ImePrezime nvarchar(100)
)

insert into dbo.ClanFROdbora values ('test','test1','test.test@gmail.com','test',1,'test','test',1,1,'test test1')
--select * from dbo.ClanFROdbora

--alter table ClanFROdbora
--	add ImePrezime nvarchar(100)

create table TipSuradnje -- Materijalna/Financijska/Oboje
(
	IDTipSuradnja int not null primary key identity(1,1),
	TipSuradnja nvarchar(50) not null
)

insert into dbo.TipSuradnje values ('Financijska'),('Materijalna'),('Financijska i Materijalna')

create table Suradnja -- Projekt + Kompanija + Kontakt + ClanFROdbora
(
	IDSuradnja int not null primary key identity(1,1),
	KomentarSuradnja nvarchar(max) null,
	OdgovoranClan int not null foreign key references ClanFROdbora(IDClan),
	TipSuradnja int null foreign key references TipSuradnje(IDTipSuradnja),
	StatusSuradnja int null foreign key references StatusSuradnja(IDStatusSuradnja),
	KontaktKompanije int null foreign key references KontaktKompanije(IDKontakt),
	IDProjekt int not null foreign key references Projekt(IDProjekt),
	IDKompanija int not null foreign key references Kompanija(IDKompanija),
	DatumZadnjegKontakta datetime null default(getdate()),
	FinancijskaVrijednost int null,
	MaterijalnaVrijednost nvarchar(max) null
)

create table SharingIsCaring -- Podjeli primjer svog razgovora s kontaktom. Primjer za materijalni/financijski FR dopis
(
	IDSiC int not null primary key identity(1,1),
	DatumIzmjene datetime null default(getdate()),
	PrimjerDopisa nvarchar(max) null,
	OsobniKomentarDopisa nvarchar(max) null,
	OdgClan int not null foreign key references ClanFROdbora(IDClan),
	KontaktKompanije int null foreign key references KontaktKompanije(IDKontakt),
	IDKompanija int null foreign key references Kompanija(IDKompanija),
	VrstaDopisa int null foreign key references TipSuradnje(IDTipSuradnja)
)


-------------------------------------------
--delete from dbo.SharingIsCaring
--delete from dbo.Suradnja
--delete from dbo.KontaktKompanije
--delete from dbo.Kompanija
--delete from dbo.Projekt
--delete from dbo.ClanFROdbora
----------------------------------------------