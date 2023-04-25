using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    class Program
    {
        static void Main(string[] args)

        {
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("Projekt w C# Tomasz Pielecki Dzienik Ocen Dla Uczelni");
            Console.WriteLine("------------------------------------------------");
            string[] imona = { "Grzegorz", "Marek", "Iwona", "Jola", "Tomek", "Wojtek" };
            string[] nazwiska = { "Kowalski", "Nowak", "Marchewka", "Zaręba", "Młotek", "Kleszcz" };
            string[] przedmioty = { "Programowanie Obiektowe", "Sieci", "Grafika Rastrowa", "Angielski", "Aplikacje WWW" };
            string[] jednostki = { "IISI", "KIK", "ICIS" };
            string[] tytul = { "dr", "dr inż.", "prof.", "prof. inż.", "inż." };
            Random rnd = new Random();

            Wydzial w = new Wydzial();

            {
                // przedmioty
                przedmioty.ToList().ForEach(a => w.Dodaj(
                    new Przedmiot(a, "Informatyka", "Inżynieria Oprogramowania", rnd.Next(1, 7), rnd.Next(1, 3) * 15)
                ));
                // studenci
                for (int i = 1000; i < 1020; i++)
                    w.Dodaj(new Student(
                        imona[rnd.Next(imona.Length)],
                        nazwiska[rnd.Next(nazwiska.Length)],
                        DateTime.Now.AddYears(-27).AddDays(rnd.Next(365 * 4)),
                        "Informatyka", "Inżynieria Oprogramowania", rnd.Next(1, 4), rnd.Next(1, 10), i
                    ));
                //jednostki
                jednostki.ToList().ForEach(a =>
                {
                    var j = new Jednostka(a, "Dąbrowskiego 69");
                    w.Dodaj(j);
                    for (int i = 0; i < 3; i++)
                        j.DodajWykladowce(new Wykladowca(
                            imona[rnd.Next(imona.Length)],
                            nazwiska[rnd.Next(nazwiska.Length)],
                            DateTime.Now.AddYears(-47).AddDays(rnd.Next(365 * 14)),
                            tytul[rnd.Next(tytul.Length)],
                            ""
                        ));
                }
                );

                //oceny

                for (int i = 0; i < 200; i++)
                    w.WystawOcene(
                        rnd.Next(1000, 1020),
                        przedmioty[rnd.Next(przedmioty.Length)],
                        rnd.Next(2, 5),
                        DateTime.Now.AddDays(-rnd.Next(2, 360 * 3))
                    );
            }

            // szuka liste i wypisuje
            //w.Filtruj<Student>(s => s.grupa == 4).ForEach(s=>s.WypiszInfo(false));
            // wypisuje studentów z ocenami
            //w.Info<Student>(true, s => s.srednia > 0);
            // dodaje studenta
            //w.Dodaj(new Student("Ela", "Nowa", null, "IO", "Informatyka", 2, 4, 123));


            Console.WriteLine("\nStudenci:");
            w.Info<Student>();
            Console.WriteLine("\nJednostki i pracownicy:");
            w.Info<Jednostka>(true);
            Console.WriteLine("\nPrzedmioty:");
            w.Info<Przedmiot>();
            Console.WriteLine("\n\nStudenci lat 1-2:");
            w.Info<Student>(false, s => s.rok >= 1 && s.rok <= 2);


            {
                Console.WriteLine("\n\nOceny z Sieci roku 2:");
                var p = w.Znajdz<Przedmiot>(a => a.nazwa == "Sieci");
                p.WypiszInfo(false);
                w.Filtruj<Student>(s => s.rok == 2)
                    .ForEach(s => Console.WriteLine($"{s.nrIndeksu}: {s[p]}"));
            }

            Console.WriteLine("\n\nPrzedmioty rok 1:");
            w.Info<Przedmiot>(false, a => a.rok == 1);


            {
                var wyk = new Wykladowca("<Jacek>", nazwiska[0], DateTime.Now, tytul[0], "");
                var j = w.Znajdz<Jednostka>(a => a.nazwa == "KIK").DodajWykladowce(wyk);
                Console.WriteLine("\n\nJednosti:");
                w.Info<Jednostka>(true);
                Console.WriteLine(w.PrzeniesWykladowce(wyk, "KIK", "IISI"));
                Console.WriteLine("\n\nJednosti:");
                w.Info<Jednostka>(true);
            }
            Console.ReadKey();
        }
    }


    public class Jednostka : IInfo
    {
        List<Wykladowca> wykladowcy = new List<Wykladowca>();

        public string nazwa { get; set; }
        public string adres { get; set; }

        public Jednostka(string nazwa, string adres)
        {
            this.adres = adres;
            this.nazwa = nazwa;
        }

        public void WypiszInfo(bool wiecej)
        {
            Console.WriteLine($"{nazwa} {adres}");
            if (wiecej)
                InfoWykladowcy();
        }

        public void InfoWykladowcy()
        {
            Console.WriteLine($"Wykładowcy ({wykladowcy.Count}):");
            wykladowcy.ForEach(a => Console.WriteLine($"\t{a}"));
        }

        public bool DodajWykladowce(Wykladowca w)
        {
            if (wykladowcy.Contains(w))
                return false;
            wykladowcy.Add(w);
            return true;
        }

        public Wykladowca this[string imie, string nazwisko]
        {
            get { return wykladowcy.FirstOrDefault(w => w.imie == imie && w.nazwisko == nazwisko); }
        }

        public bool UsunWykladowce(Wykladowca w)
        {
            return wykladowcy.Remove(w);
        }

        public override string ToString()
        {
            return $"{nazwa} {adres}";
        }
    }

    public class OcenaKoncowa : IInfo
    {
        public Przedmiot przedmiot { get; set; }
        public float wartosc { get; set; }
        public DateTime data { get; set; }

        public OcenaKoncowa(Przedmiot przedmiot, float wartosc = 2.0f, DateTime? data = null)
        {
            this.przedmiot = przedmiot;
            this.wartosc = wartosc;
            if (data == null)
                this.data = DateTime.Now;
            else
                this.data = (DateTime)data;

        }

        public override string ToString()
        {
            return $"{przedmiot.nazwa} {wartosc} {data.ToShortDateString()}";
        }

        public void WypiszInfo(bool wiecej)
        {
            Console.WriteLine(this);
        }
    }

    public class Osoba : IInfo
    {
        public string imie { get; set; }
        public string nazwisko { get; set; }
        public DateTime dataUrodzenia { get; set; }
        public int lat { get { return DateTime.Now.Year - dataUrodzenia.Year; } }

        public Osoba(string imie, string nazwisko, DateTime? dataUrodzenia = null)
        {
            this.imie = imie;
            this.nazwisko = nazwisko;
            this.dataUrodzenia = dataUrodzenia != null ? (DateTime)dataUrodzenia : DateTime.Now;
        }

        public override string ToString()
        {
            return $"{imie} {nazwisko} {dataUrodzenia.ToShortDateString()} ({lat})";
        }

        public virtual void WypiszInfo(bool wiecej)
        {
            Console.WriteLine(this);
        }
    }

    public class Przedmiot : IInfo
    {
        public string nazwa { get; set; }
        public string kierunek { get; set; }
        public string specjalnosc { get; set; }
        public int semestr { get; set; }
        public int liczbaGodzin { get; set; }
        public int rok { get { return (semestr + 1) / 2; } }

        public Przedmiot(string nazwa, string kierunek, string specjalnosc, int semestr, int liczbaGodzin)
        {
            this.nazwa = nazwa;
            this.kierunek = kierunek;
            this.specjalnosc = specjalnosc;
            this.semestr = semestr;
            this.liczbaGodzin = liczbaGodzin;
        }

        public override string ToString()
        {
            return $"{nazwa} {specjalnosc}/{kierunek}  sem.{semestr} rok.{rok} ({liczbaGodzin} godzin)";
        }

        public void WypiszInfo(bool wiecej)
        {
            Console.WriteLine(this);
        }
    }

    public class Student : Osoba
    {
        List<OcenaKoncowa> oceny = new List<OcenaKoncowa>();
        public string kierunek { get; set; }
        public string specjalnosc { get; set; }
        public int rok { get; set; }
        public int grupa { get; set; }
        public int nrIndeksu { get; set; }
        public float srednia
        {
            get
            {
                float s = 0;
                oceny.ForEach(a => s += a.wartosc);
                return oceny.Count == 0 ? 0 : s / oceny.Count;
            }
        }


        public Student(string imie, string nazwisko, DateTime dataUrodzenia,
            string kierunek, string specjalnosc, int rok, int grupa, int nrIndeksu)
            : base(imie, nazwisko, dataUrodzenia)
        {
            this.kierunek = kierunek;
            this.specjalnosc = specjalnosc;
            this.rok = rok;
            this.grupa = grupa;
            this.nrIndeksu = nrIndeksu;
        }

        public override string ToString()
        {
            return $"{nrIndeksu} {base.ToString()} g.{grupa} rok {rok} {specjalnosc}/{kierunek}";
        }

        public void InfoOceny()
        {
            Console.WriteLine($"oceny ({oceny.Count}):");
            oceny.ForEach(a => Console.WriteLine($"\t{a}"));
        }

        public override void WypiszInfo(bool wiecej)
        {
            Console.WriteLine($"{this} \nśrednia ocen:{srednia}");
            if (wiecej)
                InfoOceny();
        }

        public bool DodajOcene(Przedmiot p, float ocena, DateTime? data = null)
        {
            if (this[p] != null)
                return false;
            this.oceny.Add(new OcenaKoncowa(p, ocena, data));
            return true;
        }

        public OcenaKoncowa this[Przedmiot p]
        {
            get { return oceny.SingleOrDefault(a => a.przedmiot == p); }
        }

    }

    public class Wydzial
    {
        List<Przedmiot> przedmioty = new List<Przedmiot>();
        List<Student> studenci = new List<Student>();
        List<Jednostka> jednostki = new List<Jednostka>();

        List<T> list<T>()
        {
            if (typeof(T) == typeof(Przedmiot))
                return przedmioty as List<T>;
            if (typeof(T) == typeof(Student))
                return studenci as List<T>;
            if (typeof(T) == typeof(Jednostka))
                return jednostki as List<T>;
            return null;
        }

        public bool Dodaj<T>(T obj)
        {
            var l = list<T>();
            if (l == null || l.Contains(obj))
                return false;
            l.Add(obj);
            return true;
        }

        public T Znajdz<T>(Func<T, bool> filter) where T : class
        {
            var l = list<T>();
            if (l == null)
                return null;
            return l.SingleOrDefault(filter);
        }

        public List<T> Filtruj<T>(Func<T, bool> filter)
        {
            var l = list<T>();
            if (l == null)
                return new List<T>();
            return l.Where(filter).ToList();
        }

        public void Info<T>(bool wiecej = false, Func<T, bool> filter = null) where T : IInfo
        {
            var l = list<T>();
            if (l == null)
                return;
            if (filter != null)
                l = l.Where(filter).ToList();
            l.ForEach(a => a.WypiszInfo(wiecej));
        }

        public bool WystawOcene(int nrIndesu, string przedmiot, float ocena, DateTime? data = null)
        {
            var s = this.Znajdz<Student>(a => a.nrIndeksu == nrIndesu);
            var p = this.Znajdz<Przedmiot>(a => a.nazwa == przedmiot);
            if (s == null || p == null)
                return false;
            return s.DodajOcene(p, ocena, data);
        }

        public bool PrzeniesWykladowce(Wykladowca w, string obecnaJednostak, string nowaJednostak)
        {
            var j1 = Znajdz<Jednostka>(j => j.nazwa == obecnaJednostak);
            var j2 = Znajdz<Jednostka>(j => j.nazwa == nowaJednostak);
            if (j1 == null || j2 == null)
                return false;
            return j1.UsunWykladowce(w) && j2.DodajWykladowce(w);
        }
    }

    public class Wykladowca : Osoba
    {
        public string tytulNaukowy { get; set; }
        public string stanowisko { get; set; }

        public Wykladowca(string imie, string nazwisko, DateTime dataUrodzenia, string tytul, string stanowsiko)
            : base(imie, nazwisko, dataUrodzenia)
        {
            this.tytulNaukowy = tytul;
            this.stanowisko = stanowisko;
        }

        public override string ToString()
        {
            return $"{tytulNaukowy} {base.ToString()} {stanowisko}";
        }
    }

    public interface IInfo
    {
        void WypiszInfo(bool wiecej = false);
    }
}
