using System;
using System.Collections.Generic;
using LeapWoF.Interfaces;



namespace LeapWoF
{
    public class GameManager
    {
        private IInputProvider inputProvider;
        private IOutputProvider outputProvider;
        private readonly string Puzzle = "Hello world";
        private string PuzzleDisplay;
        public List<string> GuessedLetters = new List<string>();
        public List<Player> Players { get; private set; } = new List<Player>();



        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider()) { }



        public GameManager(IInputProvider input, IOutputProvider output)
        {
            inputProvider = input ?? throw new ArgumentNullException(nameof(input));
            outputProvider = output ?? throw new ArgumentNullException(nameof(output));
        }



        public void StartGame()
        {
            InitPlayers();
            WelcomePlayer();
            while (true)
            {
                foreach (var player in Players)
                {
                    outputProvider.WriteLine($"{player.Name}'s turn!");
                    PlayTurn(player);
                }
            }
        }



        public void InitPlayers()
        {
            for (int i = 0; i < 3; i++)
            {
                outputProvider.WriteLine($"Enter Player Name {i + 1}: ");
                string playerName = inputProvider.Read();
                Players.Add(new Player(playerName));
            }
        }



        private void DisplayPlayerInfo()
        {
            foreach (var player in Players)
            {
                outputProvider.WriteLine($"{player.Name}: Total Money: ${player.TotalMoney}, Round Money: ${player.RoundMoney}");
            }
        }



        private void WelcomePlayer()
        {
            outputProvider.WriteLine("Welcome to Wheel of Fortune!");
            PuzzleDisplay = new string('_', Puzzle.Length);
            ShowPuzzle();
        }



        private void PlayTurn(Player currentPlayer)
        {
            outputProvider.WriteLine("1: Guess a letter | 2: Solve the puzzle");
            var choice = inputProvider.Read();
            if (choice == "1") GuessLetter(currentPlayer);
            else if (choice == "2") SolvePuzzle();

            // Display the player's money after each turn.
            DisplayPlayerInfo();
        }



        private void ShowPuzzle()
        {
            outputProvider.WriteLine(PuzzleDisplay);
        }



        private void GuessLetter(Player currentPlayer)
        {
            outputProvider.Write("Guess a letter: ");
            var letter = inputProvider.Read().ToUpper();



            if (GuessedLetters.Contains(letter))
            {
                outputProvider.WriteLine("Already guessed this letter.");
                return;
            }



            bool letterFound = false;
            for (int i = 0; i < Puzzle.Length; i++)
            {
                if (Puzzle[i].ToString().ToUpper() == letter)
                {
                    PuzzleDisplay = PuzzleDisplay.Remove(i, 1).Insert(i, Puzzle[i].ToString());
                    letterFound = true;
                }
            }



            if (letterFound)
            {
                outputProvider.WriteLine($"Correct! '{letter}' is in the puzzle.");
                currentPlayer.AddMoney(100);
            }
            else
            {
                outputProvider.WriteLine($"Sorry, '{letter}' is not in the puzzle.");
            }



            GuessedLetters.Add(letter);
            ShowPuzzle();
        }



        private void SolvePuzzle()
        {
            outputProvider.Write("Your solution: ");
            var solution = inputProvider.Read();
            if (solution.Equals(Puzzle, StringComparison.OrdinalIgnoreCase))
            {
                outputProvider.WriteLine("You solved it!");
                Environment.Exit(0);
            }
            else
            {
                outputProvider.WriteLine("Try again.");
            }
        }
    }
}