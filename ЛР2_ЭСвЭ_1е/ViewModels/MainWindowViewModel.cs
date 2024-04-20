using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ЛР2_ЭСвЭ_1е.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        // Вложенный класс для хранения данных о вопросах, ответах и баллах
        public class QuestionAnswerPoints(string question, double firstType, double secondType, double thirdType) : ReactiveObject
        {
            [Reactive] public string Question { get; set; } = question;
            [Reactive] public bool Answer { get; set; } = false;
            [Reactive] public (double FirstType, double SecondType, double ThirdType) Points { get; set; } = (firstType, secondType, thirdType);
        }

        // Вопросы и баллы для продавца
        [Reactive] public ObservableCollection<QuestionAnswerPoints> Seller { get; set; } = [
            new ("предлагает товар энергично и напористо?",                     5,   0,   2.5),
            new ("не настойчив с клиентом?",                                    0,   10,  0),
            new ("не идет на уступки в вопросах цены?",                         10,  0,   0),
            new ("старается избегать возможных осложнений при работе?",         0,   5,   0),
            new ("уделяет большое внимание клиенту?",                            0,   2.5, 5),
            new ("компетентен, знает многое предмете продаж?",                  2.5, 0,   10),
            new ("испытывает чувство собственного преимущества перед клиентом?", 5,   0,   0),
            new ("открыт и честен с клиентами и коллегами?",                    0,   0,   5),
            new ("старается прислушиваться к мнению покупателя?",               0,   5,   2.5),
            new ("можно назвать честолюбивым?",                                 2.5, 0,   0),
            new ("во всем старается быть полезным покупателю?",                 0,   2.5, 0)
        ];

        //Вопросы и баллы для покупателя
        [Reactive] public ObservableCollection<QuestionAnswerPoints> Buyer { get; set; } = [
            new ("демонстрирует, что знает больше других?",                   6,   0,   0),
            new ("не интересуется мнением продавца?",                         7,   0,   0),
            new ("высокомерен в общении?",                                    4,   0,   0),
            new ("чрезвычайно придирчив при выборе товара?",                  3,   0,   0),
            new ("часто меняет решения по вариантам покупки?",                0,   3,   0),
            new ("задает продавцу неуместные вопросы?",                       0,   7,   0),
            new ("не может четко сформулировать, что его интересует?",        0,   6,   0),
            new ("полностью отдает инициативу при выборе товара продавцу?",   0,   4,   0),
            new ("всегда тщательно осматривает весь ассортимент?",            0,   0,   7),
            new ("внимательно выслушивает мнение продавца?",                  0,   0,   6),
            new ("свободно излагает свои идеи и вопросы по товару?",          0,   0,   4),
            new ("обладает гибкостью в вопросах подбора товара?",             0,   0,   3)
        ];

        // Матрица результатов
        private static readonly string[][] _performance = [
            ["Результат средний. взаимное соперничество", "Результат низкий. Продавец не может убедить покупателя", "Результат средний. Покупатель не доверяет продавцу"],
            ["Результат высокий. Продавец \"задавит\" покупателя", "Результат средний. Стороны не могут найти общий язык", "Результат средний. Продавец не может понять покупателя"],
            ["Результат средний. Покупатель получает информацию", "Результат низкий. Покупатель не получает ответы на вопросы", "Результат высокий. Взаимное уважение, четкое понимание цели"]
        ];

        [Reactive] public string? Result { get; set; } // Результат
        public void GetResult()
        {
            Func<(double, double, double), double> sum = (a) => a.Item1 + a.Item2 + a.Item3;
            Func<(double, double, double), (double, double, double)> normal = (a) => (a.Item1 / sum(a), a.Item2 / sum(a), a.Item3 / sum(a));

            (double FirstType, double SecondType, double ThirdType) seller;
            (double FirstType, double SecondType, double ThirdType) buyer;

            if (Seller.Any(x => x.Answer))
                seller = normal(Seller.Where(x => x.Answer)
                                      .Select(x => x.Points)
                                      .Aggregate((a, b) => (a.FirstType + b.FirstType,
                                                            a.SecondType + b.SecondType,
                                                            a.ThirdType + b.ThirdType)));
            else seller = (0, 0, 0);

            if (Buyer.Any(x => x.Answer))
                buyer = normal(Buyer.Where(x => x.Answer)
                                    .Select(x => x.Points)
                                    .Aggregate((a, b) => (a.FirstType + b.FirstType,
                                                          a.SecondType + b.SecondType,
                                                          a.ThirdType + b.ThirdType)));
            else buyer = (0, 0, 0);

            double[][] matrix = [
                [seller.FirstType * buyer.FirstType,    seller.SecondType * buyer.FirstType,    seller.ThirdType * buyer.FirstType],
                [seller.FirstType * buyer.SecondType,   seller.SecondType * buyer.SecondType,   seller.ThirdType * buyer.SecondType],
                [seller.FirstType * buyer.ThirdType,    seller.SecondType * buyer.ThirdType,    seller.ThirdType * buyer.ThirdType]
            ];

            var (i, j, value) = matrix.Select((x, xIndex) => (
                                                                 i: xIndex,
                                                                 sub: x.Select((y, yIndex) => (
                                                                     j: yIndex,
                                                                     value: y
                                                                 )).Aggregate((max, current) => max.value >= current.value ? max : current)
                                                             ))
                                      .Select(x => (x.i, x.sub.j, x.sub.value))
                                      .Aggregate((max, current) => max.value > current.value ? max : current);


            Result = _performance[i][j];
        }
    }
}