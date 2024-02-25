using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LosowanieOsoby
{
    public partial class MainPage : ContentPage
    {
        private List<Uczen> uczniowie = new List<Uczen>();
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "uczniowie.txt");
        private string ostatnieLosowaniaPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ostatnie_losowania.txt");

        private Queue<string> ostatnieLosowania = new Queue<string>();
        private int maxLiczbaOstatnichLosowan = 3;

        public Uczen SelectedUczen
        {
            get; set;
        }

        public MainPage()
        {
            InitializeComponent();

            if (File.Exists(filePath))
            {
                uczniowie = File.ReadAllLines(filePath)
                    .Select((line, index) => new Uczen { NumerPorzadkowy = index + 1, ImieNazwisko = line })
                    .ToList();
            }

            if (File.Exists(ostatnieLosowaniaPath))
            {
                ostatnieLosowania = new Queue<string>(File.ReadAllLines(ostatnieLosowaniaPath).Reverse());
            }

            RefreshListView();

            DodajButton.Clicked += DodajButton_Clicked;
            LosujButton.Clicked += LosujButton_Clicked;
            ZapiszButton.Clicked += ZapiszButton_Clicked;
            WczytajButton.Clicked += WczytajButton_Clicked;
            
            UsunButton.Clicked += UsunButton_Clicked;

            UczniowieListView.ItemSelected += UczniowieListView_ItemSelected;

            BindingContext = this;

          
            NavigationPage.SetTitleView(this, new Label { Text = "System losowania osoby do odpowiedzi", FontSize = 20, HorizontalOptions = LayoutOptions.CenterAndExpand });
        }

        private void DodajButton_Clicked(object sender, EventArgs e)
        {
            string uczestnik = UczestnikEntry.Text;
            if (!string.IsNullOrWhiteSpace(uczestnik))
            {
                uczniowie.Add(new Uczen { NumerPorzadkowy = uczniowie.Count + 1, ImieNazwisko = uczestnik });
                RefreshListView();
                UczestnikEntry.Text = string.Empty;
            }
        }

        

        private void UczniowieListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SelectedUczen = (Uczen)e.SelectedItem;
        }

        private void LosujButton_Clicked(object sender, EventArgs e)
        {
            if (uczniowie.Any(u => u.IsObecny))
            {
                List<Uczen> dostepniUczniowie = uczniowie
                    .Where(u => u.IsObecny && !ostatnieLosowania.Contains(u.ImieNazwisko))
                    .ToList();

                if (dostepniUczniowie.Any())
                {
                    Random random = new Random();
                    int index = random.Next(0, dostepniUczniowie.Count);
                    Uczen wylosowanyUczen = dostepniUczniowie[index];

                    ostatnieLosowania.Enqueue(wylosowanyUczen.ImieNazwisko);
                    if (ostatnieLosowania.Count > maxLiczbaOstatnichLosowan)
                    {
                        ostatnieLosowania.Dequeue();
                    }

                    RefreshListView();

                    DisplayAlert("Wylosowany uczestnik", $"{wylosowanyUczen.ImieNazwisko} (Numer porządkowy: {wylosowanyUczen.NumerPorzadkowy})", "OK");
                }
                else
                {
                    DisplayAlert("Błąd", "Wszyscy obecni uczniowie zostali już wylosowani w ostatnich trzech losowaniach.", "OK");
                }
            }
            else
            {
                DisplayAlert("Błąd", "Brak obecnych uczniów do losowania.", "OK");
            }
        }

        private void ZapiszButton_Clicked(object sender, EventArgs e)
        {
            File.WriteAllLines(filePath, uczniowie.Select(u => u.ImieNazwisko));
            File.WriteAllLines(ostatnieLosowaniaPath, ostatnieLosowania);
            DisplayAlert("Zapisano", "Lista uczniów i historia ostatnich losowań zostały zapisane.", "OK");
        }

        private void WczytajButton_Clicked(object sender, EventArgs e)
        {
            if (File.Exists(filePath))
            {
                uczniowie = File.ReadAllLines(filePath)
                    .Select((line, index) => new Uczen { NumerPorzadkowy = index + 1, ImieNazwisko = line })
                    .ToList();
                RefreshListView();
                DisplayAlert("Wczytano", "Lista uczniów została wczytana.", "OK");
            }
            else
            {
                DisplayAlert("Błąd", "Plik z listą uczniów nie istnieje.", "OK");
            }

            if (File.Exists(ostatnieLosowaniaPath))
            {
                ostatnieLosowania = new Queue<string>(File.ReadAllLines(ostatnieLosowaniaPath).Reverse());
            }
            else
            {
                ostatnieLosowania.Clear();
            }
        }

        private void RefreshListView()
        {
            UczniowieListView.ItemsSource = null;
            UczniowieListView.ItemsSource = uczniowie;
        }

        private async void UsunButton_Clicked(object sender, EventArgs e)
        {
            if (SelectedUczen != null)
            {
                bool result = await DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć ucznia {SelectedUczen.ImieNazwisko}?", "Tak", "Nie");

                if (result)
                {
                    uczniowie.Remove(SelectedUczen);
                    RefreshListView();
                    DisplayAlert("Sukces", "Uczeń został usunięty.", "OK");
                }
            }
            else
            {
                DisplayAlert("Błąd", "Nie wybrano ucznia do usunięcia.", "OK");
            }
        }

        private async void EdytujButton_Clicked(object sender, EventArgs e)
        {
            if (SelectedUczen != null)
            {
                string newName = await DisplayPromptAsync("Edytuj ucznia", "Wprowadź nowe imię i nazwisko", "OK", "Anuluj", SelectedUczen.ImieNazwisko);

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    SelectedUczen.ImieNazwisko = newName;
                    RefreshListView();
                    DisplayAlert("Sukces", "Uczeń został zaktualizowany.", "OK");
                }
            }
            else
            {
                DisplayAlert("Błąd", "Nie wybrano ucznia do edycji.", "OK");
            }
        }

    }

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
    }
}
