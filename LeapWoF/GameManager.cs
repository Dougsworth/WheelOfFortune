using System;
using System.Collections.Generic;
using System.Linq;
using LeapWoF.Interfaces;

namespace LeapWoF
{
    // This class manages the game logic for a Wheel of Fortune-like game.
    public class GameManager
    {
        // This is an interface for getting input from the user (e.g., console, GUI, etc.)
        private IInputProvider inputProvider;

        // This is an interface for displaying output to the user (e.g., console, GUI, etc.)
        //This accepts input from the user and display output to the user
        private IOutputProvider outputProvider;

        // This string holds the current puzzle or phrase that players are trying to guess.
        private string TemporaryPuzzle;
        protected string[] hiddenPuzzle = new string[1];



        // This list keeps track of the letters that players have guessed.
        public List<string> charGuessList = new List<string>();

        // This property represents the current state of the game (e.g., waiting to start, in progress, or over).
        public GameState GameState { get; private set; }

        // Default constructor: Initializes the game manager with default console-based input and output providers.
        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider()) { }

        // Overloaded constructor: Allows for custom input and output providers.
        public GameManager(IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            // Check if the provided input and output providers are null and throw exceptions if they are.
            if (inputProvider == null)
                throw new ArgumentNullException(nameof(inputProvider));
            if (outputProvider == null)
                throw new ArgumentNullException(nameof(outputProvider));

            // Initialize the input and output providers.
            this.inputProvider = inputProvider;
            this.outputProvider = outputProvider;

            // Set the initial game state to "WaitingToStart".
            GameState = GameState.WaitingToStart;
        }

        // This method starts and manages the game.
        public void StartGame()
        {
            // Initialize the game.
            InitGame();

            // Main game loop.
            while (true)
            {
                // Execute a single turn of the game.
                PerformSingleTurn();

                // Check the game state and act accordingly.
                if (GameState == GameState.RoundOver)
                {
                    StartNewRound();
                    continue;
                }

                // End the game if the state is "GameOver".
                if (GameState == GameState.GameOver)
                {
                    outputProvider.WriteLine("Game over");
                    break;
                }
            }
        }

        // This method starts a new round of the game.
        public void StartNewRound()
        {
            // Set a new puzzle (in this case, always "Hello world").
            TemporaryPuzzle = "Hello world";

            // Update the game state to indicate that a new round has started.
            GameState = GameState.RoundStarted;
        }

        // This method executes a single turn of the game.
        public void PerformSingleTurn()
        {
            // Clear the output and display the current puzzle.
            outputProvider.Clear();
            DrawPuzzle();

            // Prompt the player to either spin the wheel or solve the puzzle.
            outputProvider.WriteLine("Type 1 to spin, 2 to solve");
            GameState = GameState.WaitingForUserInput;

            // Get the player's choice.
            var action = inputProvider.Read();

            // Act based on the player's choice.
            switch (action)
            {
                case "1":
                    Spin();
                    break;
                case "2":
                    Solve();
                    break;
            }
        }

        // This method displays the current puzzle to the player.
        private void DrawPuzzle()
        {
            //outputProvider.WriteLine("The puzzle is:");
            outputProvider.WriteLine("Lets start guessing the word!");
            string hiddenPuzzle = "";

            foreach (char c in TemporaryPuzzle)

            {
                // If the character is a space or has been guessed, display it.
                if (c == ' ' || charGuessList.Contains(c.ToString()))
                {
                    hiddenPuzzle += c;
                }
                // Otherwise, hide it with an underscore.
                else
                {
                    hiddenPuzzle += '_';
                }
            }


            //outputProvider.WriteLine(TemporaryPuzzle);
            outputProvider.WriteLine();
            outputProvider.WriteLine(hiddenPuzzle);
        }

        // This method simulates spinning a wheel and then prompts the player to guess a letter.
        public void Spin()
        {
            outputProvider.WriteLine("Spinning the wheel...");
            // TODO: Add logic for spinning the wheel and determining outcomes.
            GuessLetter();
        }

        // This method prompts the player to solve the puzzle.
        public void Solve()
        {
            outputProvider.Write("Please enter your solution:");
            var guess = inputProvider.Read();
            // TODO: Add logic to check if the player's solution is correct.
        }

        // This method prompts the player to guess a letter and adds the guessed letter to the list.
        public void GuessLetter()
        {
            outputProvider.Write("Please guess a letter: ");
            var guess = inputProvider.Read();

            // Check if the guess is a single letter
            if (guess.Length != 1 || !char.IsLetter(guess[0]))
            {
                outputProvider.WriteLine("Please enter a single letter.");
                return;
            }

            // Check if the letter has already been guessed
            if (charGuessList.Contains(guess))
            {
                outputProvider.WriteLine("You've already guessed this letter.");
                return;
            }

            // Add the new guess to the list of guessed letters
            charGuessList.Add(guess);

            // Check if the guessed letter is in the puzzle and provide feedback
            if (TemporaryPuzzle.Contains(guess, StringComparison.OrdinalIgnoreCase))
            {
                outputProvider.WriteLine($"Correct! '{guess}' is in the puzzle.");
            }
            else
            {
                outputProvider.WriteLine($"Sorry, '{guess}' is not in the puzzle.");
            }

            // Update the displayed puzzle
            DrawPuzzle();
        }

        // This method initializes the game, displays a welcome message, and starts a new round.
        public void InitGame()
        {
            outputProvider.WriteLine("Welcome to Wheel of Fortune!");
            StartNewRound();
        }
    }
}