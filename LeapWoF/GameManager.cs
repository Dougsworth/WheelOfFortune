using System;
using System.Collections.Generic;
using System.Linq;
using LeapWoF.Interfaces;

namespace LeapWoF
{
    public class GameManager
    {
        // Fields to handle user input and output.
        private IInputProvider inputProvider;
        private IOutputProvider outputProvider;

        // The puzzle string that players will try to guess.
        private readonly string Puzzle = "Hello world";

        // This string represents the puzzle as shown to the player, with unsolved letters replaced by underscores.
        private string PuzzleDisplay;

        // A list to keep track of letters the player has already guessed.
        public List<string> GuessedLetters = new List<string>();

        // A list to keep track of all players.
        public List<Player> Players { get; private set; } = new List<Player>();

        // Current round of the game.
        private int currentRound = 1;

        // Maximum number of rounds in the game.
        private const int MAX_ROUNDS = 3;

        // Possible outcomes when spinning the wheel.
        private object[] spinValues = { 100, 200, 300, 400, "Lose a Turn", "Bankrupt" };

        // Default constructor: Initializes the game with console-based input and output providers.
        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider()) { }

        // Overloaded constructor: Allows for custom input and output providers.
        public GameManager(IInputProvider input, IOutputProvider output)
        {
            inputProvider = input ?? throw new ArgumentNullException(nameof(input));
            outputProvider = output ?? throw new ArgumentNullException(nameof(output));
        }

        // This method starts the game. It displays a welcome message, initializes players, and then enters a loop to handle player turns.
        public void StartGame()
        {
            ExplainRules();
            InitPlayers();
            WelcomePlayer();
            while (currentRound <= MAX_ROUNDS)
            {
                foreach (var player in Players)
                {
                    outputProvider.WriteLine($"{player.Name}'s turn!");
                    PlayTurn(player);
                }
                EndRound();
            }
            EndGame();
        }
        private void ExplainRules()
        {
            outputProvider.WriteLine("Welcome to Wheel of Fortune!");
            outputProvider.WriteLine("Here are the rules:");
            outputProvider.WriteLine("1. Players take turns guessing letters or solving the puzzle.");
            outputProvider.WriteLine("2. Spinning the wheel can result in a dollar amount, 'Lose a Turn', or 'Bankrupt'.");
            outputProvider.WriteLine("3. Correct letter guesses earn the spun dollar amount for each occurrence in the puzzle.");
            outputProvider.WriteLine("4. Incorrect guesses or already guessed letters end the player's turn.");
            outputProvider.WriteLine("5. Solving the puzzle correctly ends the round, and the solver earns their round money.");
            outputProvider.WriteLine("6. Incorrect solutions end the player's turn.");
            outputProvider.WriteLine("7. The game consists of 3 rounds.");
            outputProvider.WriteLine("8. The player with the most money at the end of the game wins.");
            outputProvider.WriteLine("Good luck to all players!");
        }
        // Initialize players by asking for their names.
        public void InitPlayers()
        {
            for (int i = 0; i < 3; i++)
            {
                outputProvider.WriteLine($"Enter Player Name {i + 1}: ");
                string playerName = inputProvider.Read();
                Players.Add(new Player(playerName));
            }
        }

        // Display a welcome message and initialize the puzzle display.
        private void WelcomePlayer()
        {
            outputProvider.WriteLine("Welcome to Wheel of Fortune!");
            PuzzleDisplay = new string('_', Puzzle.Length);
            ShowPuzzle();
        }

        // Handle a single turn in the game, allowing the player to guess a letter or attempt to solve the puzzle.
        private void PlayTurn(Player currentPlayer)
        {
            outputProvider.WriteLine("1: Guess a letter | 2: Solve the puzzle");
            var choice = inputProvider.Read();
            if (choice == "1") GuessLetter(currentPlayer);
            else if (choice == "2") SolvePuzzle();
            DisplayPlayerInfo();
        }

        // Display the current state of the puzzle to the player.
        private void ShowPuzzle()
        {
            outputProvider.WriteLine(PuzzleDisplay);
        }

        // Simulate spinning the wheel to determine the dollar value or other outcomes.
        private int SpinWheel()
        {
            Random rnd = new Random();
            int index = rnd.Next(spinValues.Length);
            var spinResult = spinValues[index];

            if (spinResult.ToString() == "Lose a Turn")
            {
                outputProvider.WriteLine("You lost your turn!");
                return -1;
            }
            else if (spinResult.ToString() == "Bankrupt")
            {
                outputProvider.WriteLine("Bankrupt! You lost your round money.");
                return -2;
            }
            else
            {
                outputProvider.WriteLine($"You spun ${spinResult}!");
                return (int)spinResult;
            }
        }

        // Handle the logic for when a player tries to guess a letter.
        private void GuessLetter(Player currentPlayer)
        {
            int spinResult = SpinWheel();

            // If the player loses a turn, exit this method.
            if (spinResult == -1) return;

            // If the player goes bankrupt, reset their round money and exit this method.
            if (spinResult == -2)
            {
                currentPlayer.ResetRoundMoney();
                return;
            }

            outputProvider.Write("Guess a letter: ");
            var letter = inputProvider.Read().ToUpper();

            // Check if the player has already guessed this letter.
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

            // Provide feedback to the player based on whether the guessed letter was in the puzzle.
            if (letterFound)
            {
                outputProvider.WriteLine($"Correct! '{letter}' is in the puzzle.");
                currentPlayer.AddMoney(spinResult);
            }
            else
            {
                outputProvider.WriteLine($"Sorry, '{letter}' is not in the puzzle.");
            }

            // Add the guessed letter to the list of guessed letters.
            GuessedLetters.Add(letter);
            ShowPuzzle();
        }

        // Handle the logic for when a player tries to solve the puzzle.
        private void SolvePuzzle()
        {
            outputProvider.Write("Your solution: ");
            var solution = inputProvider.Read();
            if (solution.Equals(Puzzle, StringComparison.OrdinalIgnoreCase))
            {
                outputProvider.WriteLine("You solved it!");
                EndRound();
            }
            else
            {
                outputProvider.WriteLine("Try again.");
            }
        }

        // End the current round, update player scores, and reset necessary game state.
        private void EndRound()
        {
            Player roundWinner = Players.OrderByDescending(p => p.RoundMoney).First();
            roundWinner.AddToTotalMoney(roundWinner.RoundMoney);

            foreach (var player in Players)
            {
                if (player != roundWinner)
                {
                    player.ResetRoundMoney();
                }
            }

            currentRound++;
            GuessedLetters.Clear();
            PuzzleDisplay = new string('_', Puzzle.Length);
            ShowPuzzle();
        }

        // End the game and declare the winner.
        private void EndGame()
        {
            Player gameWinner = Players.OrderByDescending(p => p.TotalMoney).First();
            outputProvider.WriteLine($"{gameWinner.Name} wins with ${gameWinner.TotalMoney}!");
            Environment.Exit(0);
        }

        // Display the current scores of all players.
        private void DisplayPlayerInfo()
        {
            foreach (var player in Players)
            {
                outputProvider.WriteLine($"{player.Name}: Total Money: ${player.TotalMoney}, Round Money: ${player.RoundMoney}");
            }
        }
    }
}
