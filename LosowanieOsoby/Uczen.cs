using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosowanieOsoby
{
    public class Uczen
    {
        public int NumerPorzadkowy
        {
            get; set;
        }
        public string ImieNazwisko
        {
            get; set;
        }
        public bool IsObecny
        {
            get; set;
        }
        public string Klasa
        {
            get; set;
        }
        public override string ToString()
        {
            return $"{ImieNazwisko}, Klasa: {Klasa}";
        }
    }
}
