using System;
using System.Collections.Generic;
using LeapWoF.Interfaces;

namespace LeapWoF
{
    public class GameManager
    {
        private IInputProvider inputProvider;
        private IOutputProvider outputProvider;

        private readonly string TemporaryPuzzle = "Hello world";
        private string DisplayPuzzle;

        public List<string> charGuessList = new List<string>();

        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider()) { }

        public GameManager(IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            this.inputProvider = inputProvider ?? throw new ArgumentNullException(nameof(inputProvider));
            this.outputProvider = outputProvider ?? throw new ArgumentNullException(nameof(outputProvider));
        }

        public void StartGame()
        {
            InitGame();
            while (true)
            {
                PerformSingleTurn();
            }
        }

        private void InitGame()
        {
            outputProvider.WriteLine("Welcome to Wheel of Fortune!");
            DisplayPuzzle = new string('_', TemporaryPuzzle.Length);
            DrawPuzzle();
        }

        private void PerformSingleTurn()
        {
            outputProvider.WriteLine("Type 1 to guess a letter, 2 to solve the puzzle");
            var action = inputProvider.Read();
            switch (action)
            {
                case "1":
                    GuessLetter();
                    break;
                case "2":
                    Solve();
                    break;
            }
        }

        private void DrawPuzzle()
        {
            outputProvider.WriteLine(DisplayPuzzle);
        }

        private void GuessLetter()
        {
            outputProvider.Write("Please guess a letter: ");
            var guess = inputProvider.Read().ToUpper();

            if (guess.Length != 1 || !char.IsLetter(guess[0]))
            {
                outputProvider.WriteLine("Please enter a single letter.");
                return;
            }

            if (charGuessList.Contains(guess))
            {
                outputProvider.WriteLine("You've already guessed this letter.");
                return;
            }

            charGuessList.Add(guess);

            bool letterFound = false;
            for (int i = 0; i < TemporaryPuzzle.Length; i++)
            {
                if (TemporaryPuzzle[i].ToString().ToUpper() == guess)
                {
                    DisplayPuzzle = DisplayPuzzle.Remove(i, 1).Insert(i, TemporaryPuzzle[i].ToString());
                    letterFound = true;
                }
            }

            if (letterFound)
            {
                outputProvider.WriteLine($"Correct! '{guess}' is in the puzzle.");
            }
            else
            {
                outputProvider.WriteLine($"Sorry, '{guess}' is not in the puzzle.");
            }

            DrawPuzzle();
        }

        private void Solve()
        {
            outputProvider.Write("Please enter your solution: ");
            var guess = inputProvider.Read();
            if (guess.Equals(TemporaryPuzzle, StringComparison.OrdinalIgnoreCase))
            {
                outputProvider.WriteLine("Congratulations! You've solved the puzzle :))))).");
                Environment.Exit(0);  // Exit the game after solving the puzzle
            }
            else
            {
                outputProvider.WriteLine("Incorrect solution. Try again.");
            }
        }
    }
}
