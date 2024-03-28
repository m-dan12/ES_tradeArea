using ReactiveUI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ЛР2_ЭСвЭ_1е.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public static ObservableCollection<QuestionAnswerPoints> SellerQuestions = [
            new QuestionAnswerPoints("предлагает товар энергично и напористо?",                     5,   0,   2.5),
            new QuestionAnswerPoints("не настойчив с клиентом?",                                    0,   10,  0),
            new QuestionAnswerPoints("не идет на уступки в вопросах цены?",                         10,  0,   0),
            new QuestionAnswerPoints("старается избегать возможных осложнений при работе?",         0,   5,   0),
            new QuestionAnswerPoints("уделяет полное внимание клиенту?",                            0,   2.5, 5),
            new QuestionAnswerPoints("компетентен, знает многое предмете продаж?",                  2.5, 0,   10),
            new QuestionAnswerPoints("испытываетчувство собственного преимущества перед клиентом?", 5,   0,   0),
            new QuestionAnswerPoints("открыт и честен с клиентами и коллегами?",                    0,   0,   5),
            new QuestionAnswerPoints("старается прислушиваться к мнению покупателя?",               0,   5,   2.5),
            new QuestionAnswerPoints("можно назвать честолюбивым?",                                 2.5, 0,   0),
            new QuestionAnswerPoints("во всем старается быть полезным покупателю?",                 0,   2.5, 0)
        ];

        public static ObservableCollection<QuestionAnswerPoints> BuyerQuestions = [
            new QuestionAnswerPoints("демонстрирует, что знает больше других?",                   6,   0,   0),
            new QuestionAnswerPoints("не интересуется мнением продавца?",                         7,   0,   0),
            new QuestionAnswerPoints("высокомерен в общении?",                                    4,   0,   0),
            new QuestionAnswerPoints("чрезвычайно придирчив при выборе товара?",                  3,   0,   0),
            new QuestionAnswerPoints("часто меняет решения по вариантам покупки?",                0,   3,   0),
            new QuestionAnswerPoints("задает продавцу неуместные вопросы?",                       0,   7,   0),
            new QuestionAnswerPoints("не может четко сформулировать, что его интересует?",        0,   6,   0),
            new QuestionAnswerPoints("полностью отдает инициативу при выборе товара продавцу?",   0,   4,   0),
            new QuestionAnswerPoints("всегда тщательно осматривает весь ассортимент?",            0,   0,   7),
            new QuestionAnswerPoints("внимательно выслушивает мнение продавца?",                  0,   0,   6),
            new QuestionAnswerPoints("свободно излагает свои идеи и вопросы по товару?",          0,   0,   4),
            new QuestionAnswerPoints("обладает гибкостью в вопросах подбора товара?",             0,   0,   3)
        ];

        private static readonly string[][] _performance = [
            ["Результат средний. взаимное соперничество", "Результат низкий. Продавец не может убедить покупателя", "Результат средний. Покупатель не доверяет продавцу"],
            ["Результат высокий. Продавец \"задавит\" покупателя", "Результат средний. Стороны не могут найти общий язык", "Результат средний. Продавец не может понять покупателя"],
            ["Результат средний. Покупатель получает информацию", "Результат низкий. Покупатель не получает ответы на вопросы", "Результат высокий. Взаимное уважение, четкое понимание цели"]
        ];

        public class QuestionAnswerPoints : ReactiveObject
        {
            private string _question;
            public string Question
            {
                get => _question;
                set => this.RaiseAndSetIfChanged(ref _question, value);
            }
            
            private bool _answer;
            public bool Answer
            {
                get => _answer;
                set => this.RaiseAndSetIfChanged(ref _answer, value);
            }
            
            private (double FirstType, double SecondType, double ThirdType) _points;
            public (double FirstType, double SecondType, double ThirdType) Points
            {
                get => _points;
                set => this.RaiseAndSetIfChanged(ref _points, value);
            }
            
            public QuestionAnswerPoints(string question, double firstType, double secondType, double thirdType)
            {
                _question = question;
                _answer = false;
                _points = (firstType, secondType, thirdType);
            }
        }

        private ObservableCollection<QuestionAnswerPoints> _seller;
        public ObservableCollection<QuestionAnswerPoints> Seller
        {
            get => _seller;
            set
            {
                _seller = value;
                OnPropertyChanged();
            }
        }
        
        private ObservableCollection<QuestionAnswerPoints> _buyer;
        public ObservableCollection<QuestionAnswerPoints> Buyer
        {
            get => _buyer;
            set
            {
                _buyer = value;
                OnPropertyChanged();
            }
        }
        
        private string _result;
        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }
        public void getResult()
        {
            (double FirstType, double SecondType, double ThirdType) seller = (0, 0, 0);
            foreach(QuestionAnswerPoints QAPSeller in Seller)
                if (QAPSeller.Answer) seller = QAPSeller.Points;
            double sellerSum = seller.FirstType + seller.SecondType + seller.ThirdType;

            (double FirstType, double SecondType, double ThirdType) buyer = (0, 0, 0);
            foreach(QuestionAnswerPoints QAPBuyer in Buyer)
                if (QAPBuyer.Answer) buyer = QAPBuyer.Points;
            double buyerSum = buyer.FirstType + buyer.SecondType + buyer.ThirdType;

            double[][] matrix = [
                [seller.FirstType * buyer.FirstType,    seller.SecondType * buyer.FirstType,    seller.ThirdType * buyer.FirstType],
                [seller.FirstType * buyer.SecondType,   seller.SecondType * buyer.SecondType,   seller.ThirdType * buyer.SecondType],
                [seller.FirstType * buyer.ThirdType,    seller.SecondType * buyer.ThirdType,    seller.ThirdType * buyer.ThirdType]
            ];
            (int i, int j, double value) max = (1, 1, 0);
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix.Length; j++)
                    if (max.value < matrix[i][j])
                    {
                        max.i = i;
                        max.j = j;
                        max.value = matrix[i][j];
                    }
            Result = _performance[max.i][max.j];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public MainWindowViewModel()
        {
            Seller = SellerQuestions;
            Buyer = BuyerQuestions;
        }
    }
}