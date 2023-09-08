using System;
using System.Collections.Generic;
using LeapWoF.Interfaces;

namespace LeapWoF
{
    public class GameManager
    {
        // Fields to handle user input and output. These can be console-based or any other implementation of the interfaces.
        private IInputProvider inputProvider;
        private IOutputProvider outputProvider;

        // The puzzle string that players will try to guess.
        private readonly string Puzzle = "Hello world";
        // This string represents the puzzle as shown to the player, with unsolved letters replaced by underscores.
        private string PuzzleDisplay;
        // A list to keep track of letters the player has already guessed.
        public List<string> GuessedLetters = new List<string>();

        // Default constructor: Initializes the game with console-based input and output providers.
        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider()) { }

        // Overloaded constructor: Allows for custom input and output providers.
        public GameManager(IInputProvider input, IOutputProvider output)
        {
            // If the provided inputProvider is null, throw an exception.
            inputProvider = input ?? throw new ArgumentNullException(nameof(input));
            // If the provided outputProvider is null, throw an exception.
            outputProvider = output ?? throw new ArgumentNullException(nameof(output));
        }

        // This method starts the game. It displays a welcome message and then enters a loop to handle player turns.
        public void StartGame()
        {
            WelcomePlayer();
            // Infinite loop to keep the game running until the player decides to exit or solves the puzzle.
            while (true) PlayTurn();
        }

        // Display a welcome message and initialize the puzzle display.
        private void WelcomePlayer()
        {
            outputProvider.WriteLine("Welcome to Wheel of Fortune!");
            // Initialize the PuzzleDisplay with underscores for each letter in the Puzzle.
            PuzzleDisplay = new string('_', Puzzle.Length);
            ShowPuzzle();
        }

        // Handle a single turn in the game, allowing the player to guess a letter or attempt to solve the puzzle.
        private void PlayTurn()
        {
            outputProvider.WriteLine("1: Guess a letter | 2: Solve the puzzle");
            var choice = inputProvider.Read();
            if (choice == "1") GuessLetter();
            else if (choice == "2") SolvePuzzle();
        }

        // Display the current state of the puzzle to the player.
        private void ShowPuzzle()
        {
            outputProvider.WriteLine(PuzzleDisplay);
        }

        // Handle the logic for when a player tries to guess a letter.
        private void GuessLetter()
        {
            outputProvider.Write("Guess a letter: ");
            var letter = inputProvider.Read().ToUpper();

            // Check if the player has already guessed this letter.
            if (GuessedLetters.Contains(letter))
            {
                outputProvider.WriteLine("Already guessed this letter.");
                return;
            }

            // Add the guessed letter to the list of guessed letters.
            GuessedLetters.Add(letter);
            UpdatePuzzleDisplay(letter);
        }

        // Update the displayed puzzle based on the player's letter guess.
        private void UpdatePuzzleDisplay(string letter)
        {
            bool found = false;
            // Iterate through the Puzzle to check if the guessed letter is present.
            for (int i = 0; i < Puzzle.Length; i++)
            {
                if (Puzzle[i].ToString().ToUpper() == letter)
                {
                    // If the letter is found, update the PuzzleDisplay to show the letter in the correct position.
                    PuzzleDisplay = PuzzleDisplay.Remove(i, 1).Insert(i, Puzzle[i].ToString());
                    found = true;
                }
            }
            // Provide feedback to the player based on whether the guessed letter was in the puzzle.
            outputProvider.WriteLine(found ? $"Found '{letter}'!" : $"No '{letter}' found.");
            ShowPuzzle();
        }

        // Handle the logic for when a player tries to solve the puzzle.
        private void SolvePuzzle()
        {
            outputProvider.Write("Your solution: ");
            var solution = inputProvider.Read();
            // Check if the player's solution matches the Puzzle, ignoring case.
            if (solution.Equals(Puzzle, StringComparison.OrdinalIgnoreCase))
            {
                outputProvider.WriteLine("You solved it!");
                Environment.Exit(0);  // Exit the game after solving the puzzle.
            }
            else
            {
                outputProvider.WriteLine("Try again.");
            }
        }
    }
}
