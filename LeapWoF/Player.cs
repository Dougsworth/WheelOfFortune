namespace LeapWoF
{
    public class Player
    {
        public string Name { get; private set; }
        public int TotalMoney { get; private set; }
        public int RoundMoney { get; private set; }



        public Player(string name)
        {
            Name = name;  // Added missing semicolon
            TotalMoney = 0;
            RoundMoney = 0;
        }



        public void AddMoney(int money)
        {
            RoundMoney += money;
            TotalMoney += money;
        }



        public void ResetRoundMoney()
        {
            RoundMoney = 0;
        }
    }
}