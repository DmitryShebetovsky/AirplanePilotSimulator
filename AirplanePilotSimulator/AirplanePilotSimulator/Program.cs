using System;
using System.Collections.Generic;
using System.Linq;

namespace AirplanePilotSimulator
{
    internal class Program
    {
       // delegate int FlyingSpeed();
        public class AircraftCrashed : Exception
        {
            public AircraftCrashed() { }
            public AircraftCrashed(string str):base(str) { }
        }

        public class UnsuitableToFlights : Exception
        {
            public UnsuitableToFlights() { }
            public UnsuitableToFlights(string str):base(str) { }
        }

        private class Airplane
        {
            public Airplane() { _pHeight = 0; _pSpeed = 0; }
            //добавить нового диспетчера
            public void SetDispatcher(string name)
            {
                if (_pDispetchers == null)
                    _pDispetchers = new List<Dispetcher>();
                _pDispetchers.Add(new Dispetcher(name));
            }
            // если лиспетчеров > 2 можем лететь
            private bool CanFly()
            {
                return _pDispetchers.Count >= 2;
            }
            //информация от диспетчеров
            private void InfoFromDisp(int s)
            {
                foreach (var disp in _pDispetchers)
                {
                    Console.WriteLine($"Рекомендуемая высота: {Dispetcher.RecommendedHeight(s)} штрафные очки: {GetPenPoint()}");
                   // Console.WriteLine($"Штрафные очки: {Dispetcher.}");
                    try
                    {
                        disp.PenaltyPoints(Dispetcher.RecommendedHeight(s), _pHeight, _pSpeed);
                    }
                    catch (AircraftCrashed ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (UnsuitableToFlights ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            // добавляем скорость и высоту во врме яполета
            public void Flying(int speed, int height)
            {
                if (CanFly())
                {
                    _pSpeed += speed;
                    _pHeight += height;

                    if (_pSpeed < 0)
                        _pSpeed = 0;
                    if (_pHeight < 0)
                        _pHeight = 0;
                    Console.WriteLine($"скорость: {_pSpeed}\t высота: {_pHeight}");
                    InfoFromDisp(GetSpeed());
                }
                else
                {
                    Console.WriteLine("Добавьте больше одного диспетчера!");
                }
            }
            private int GetSpeed() {
                return _pSpeed;
            }
            // показываем штрафные очки
            public int GetPenPoint()
            {
                return _pDispetchers.Sum(el => el.PenPoints);
            }
            // список диспетчеров
            private List<Dispetcher> _pDispetchers;
            // скорость и высота самолета
            private int _pSpeed;
            private int _pHeight;
        }
        private class Dispetcher
        {
            public Dispetcher(string name)
            {
                PenPoints = 0;
            }
            // рандомная корректировка погоды
            private static int Weather()
            {
                var rand = ((new Random()).Next(-200, 200));
                return rand;
            }
            // рекомендованная высота
            public static int RecommendedHeight(int speed)
            {
                if (speed <= 50) return 0;
                var result = 7 * speed - Weather();
                return result;
            }
            public void PenaltyPoints(int rec, int height, int speed)
            {
                var difference = height - rec;
                if(speed > 1000)
                {
                    PenPoints += 100;
                    Console.WriteLine("Немедленно уменьшите скорость!");
                }
                if (difference > 300 && difference < 600)
                {
                    PenPoints += 25;
                   // Console.WriteLine("Штрафные: " + PenPoints);
                }
                else if (difference > 600 && difference < 1000)
                {
                    PenPoints += 50;
                   // Console.WriteLine("Штрафные: " + PenPoints);
                }
                else if (difference > 1000)
                    throw new AircraftCrashed("Самолет разбился!");
                else if (PenPoints >= 1000)
                    throw new UnsuitableToFlights("Не подходит для рейсов!");
            }
            // штрафные
            public int PenPoints { private set; get; }
        }
        private static void Main(string[] args)
        {
            var plane = new Airplane();          
            while (true)
            {
                Console.WriteLine("1.Начать полет\n2.Добавить диспетчера\n3.Показать результат");
                var m = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
                switch (m)
                {
                    case 1:
                        Console.Clear();
                        ConsoleKeyInfo cki;
                        Console.TreatControlCAsInput = true;
                        do
                        {                          
                            Console.WriteLine(
                                "Нажмите Right: +50km\\h, Left: –50km\\h, Shift-Right: +150km\\h, Shift - Left: –150km\\h");
                            Console.WriteLine("Нажмите Up: +250 m, Down: –250 m, Shift-Up: +500 m, Shift-Down: –500m).");
                            Console.WriteLine("Нажмите Escape для выхода");
                            cki = Console.ReadKey();
                            // высота
                            if (((cki.Modifiers & ConsoleModifiers.Shift) != 0) && cki.Key == ConsoleKey.UpArrow)
                                plane.Flying(0, 500);
                            if (((cki.Modifiers & ConsoleModifiers.Shift) != 0) && cki.Key == ConsoleKey.DownArrow)
                                plane.Flying(0, -500);
                            if (((cki.Modifiers & ConsoleModifiers.Shift) == 0) && cki.Key == ConsoleKey.UpArrow)
                                plane.Flying(0, 250);
                            if (((cki.Modifiers & ConsoleModifiers.Shift) == 0) && cki.Key == ConsoleKey.DownArrow)
                                plane.Flying(0, -250);
                            // скорость
                            if (((cki.Modifiers & ConsoleModifiers.Shift) != 0) && cki.Key == ConsoleKey.RightArrow)
                                plane.Flying(150, 0);
                            if (((cki.Modifiers & ConsoleModifiers.Shift) != 0) && cki.Key == ConsoleKey.LeftArrow)
                                plane.Flying(-150, 0);
                            if (((cki.Modifiers & ConsoleModifiers.Shift) == 0) && cki.Key == ConsoleKey.RightArrow)
                                plane.Flying(50, 0);
                            if (((cki.Modifiers & ConsoleModifiers.Shift) == 0) && cki.Key == ConsoleKey.LeftArrow)
                                plane.Flying(-50, 0);
                        } while (cki.Key != ConsoleKey.Escape);
                        break;
                    case 2:
                        Console.WriteLine("Введите имя диспетчера: ");
                        plane.SetDispatcher(Console.ReadLine());
                        break;
                    case 3:
                        Console.Clear();
                        try
                        {
                            if (plane.GetPenPoint() > 0)
                                Console.WriteLine($"Штафные очки: \t {plane.GetPenPoint()}");
                            else
                            {
                                throw new Exception("Нет штрафных очков!");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                }
            }
        }
    }
}
