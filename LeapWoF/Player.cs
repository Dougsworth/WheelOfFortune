namespace LeapWoF
{
    public class Player
    {
        public string Name { get; private set; }
        public int TotalMoney { get; private set; }
        public int RoundMoney { get; private set; }

        public Player(string name)
        {
            Name = name;
            TotalMoney = 0;
            RoundMoney = 0;
        }

        // Add money to the player's current round score.
        public void AddMoney(int money)
        {
            RoundMoney += money;
        }

        // Add the player's round score to their total score.
        public void AddToTotalMoney(int money)
        {
            TotalMoney += money;
        }

        // Reset the player's round score to zero.
        public void ResetRoundMoney()
        {
            RoundMoney = 0;
        }
    }
}