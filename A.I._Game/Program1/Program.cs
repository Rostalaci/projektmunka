using System;
using System.Linq;

class TicTacToe
{
    static char[] board = new char[9];
    static char human = 'X';
    static char computer = 'O';

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        bool playAgain = true;

        while (playAgain)
        {
            InitBoard();
            Console.Clear();
            Console.WriteLine("=== Tic-Tac-Toe (Amőba) ===");
            Console.WriteLine("1) Két játékos");
            Console.WriteLine("2) Játékos vs. Számítógép (AI)");
            Console.Write("Válassz módot (1 vagy 2): ");
            var mode = Console.ReadLine();

            if (mode == "1")
                PlayTwoPlayers();
            else
                PlayVsComputer();

            Console.WriteLine();
            Console.Write("Szeretnél egy új kört kezdeni? (i/n): ");
            string ans = Console.ReadLine()?.Trim().ToLower() ?? "";
            playAgain = ans == "i" || ans == "igen";
        }

        Console.WriteLine("Köszönöm, hogy játszottál! Nyomj egy gombot a kilépéshez...");
        Console.ReadKey();
    }

    static void InitBoard()
    {
        for (int i = 0; i < 9; i++) board[i] = (char)('1' + i);
    }

    static void DrawBoard()
    {
        Console.Clear();
        Console.WriteLine("Amőba tábla (helyek: 1-9)");
        Console.WriteLine();
        Console.WriteLine($" {board[0]} | {board[1]} | {board[2]} ");
        Console.WriteLine("---+---+---");
        Console.WriteLine($" {board[3]} | {board[4]} | {board[5]} ");
        Console.WriteLine("---+---+---");
        Console.WriteLine($" {board[6]} | {board[7]} | {board[8]} ");
        Console.WriteLine();
    }

    static bool IsWinner(char[] b, char player)
    {
        int[,] wins = {
            {0,1,2}, {3,4,5}, {6,7,8}, // sorok
            {0,3,6}, {1,4,7}, {2,5,8}, // oszlopok
            {0,4,8}, {2,4,6}           // átlók
        };

        for (int i = 0; i < wins.GetLength(0); i++)
        {
            if (b[wins[i, 0]] == player && b[wins[i, 1]] == player && b[wins[i, 2]] == player)
                return true;
        }
        return false;
    }

    static bool IsBoardFull(char[] b)
    {
        return b.All(c => c == 'X' || c == 'O');
    }

    static void PlayTwoPlayers()
    {
        InitBoard();
        char current = 'X';
        while (true)
        {
            DrawBoard();
            Console.WriteLine($"Játékos {current} lép.");
            int pos = ReadMove();
            if (board[pos] == 'X' || board[pos] == 'O')
            {
                Console.WriteLine("Ide már léptek. Nyomj egy gombot és próbáld újra...");
                Console.ReadKey();
                continue;
            }
            board[pos] = current;

            if (IsWinner(board, current))
            {
                DrawBoard();
                Console.WriteLine($"Gratulálok! Játékos {current} nyert!");
                break;
            }

            if (IsBoardFull(board))
            {
                DrawBoard();
                Console.WriteLine("Döntetlen!");
                break;
            }

            current = (current == 'X') ? 'O' : 'X';
        }
    }

    static void PlayVsComputer()
    {
        InitBoard();
        Console.Write("Szeretnél Te kezdeni? (i/n): ");
        var s = Console.ReadLine();
        bool humanStarts = s != null && s.Trim().ToLower().StartsWith("i");

        char current = humanStarts ? human : computer;

        while (true)
        {
            DrawBoard();

            if (current == human)
            {
                Console.WriteLine("A Te lépésed.");
                int pos = ReadMove();
                if (board[pos] == 'X' || board[pos] == 'O')
                {
                    Console.WriteLine("Ide már léptek. Nyomj egy gombot és próbáld újra...");
                    Console.ReadKey();
                    continue;
                }
                board[pos] = human;
            }
            else
            {
                Console.WriteLine("A számítógép gondolkodik...");
                int best = GetBestMove(board, computer);
                board[best] = computer;
            }

            if (IsWinner(board, current))
            {
                DrawBoard();
                if (current == human) Console.WriteLine("Gratulálok! Te nyertél!");
                else Console.WriteLine("A számítógép nyert. Próbáld újra!");
                break;
            }

            if (IsBoardFull(board))
            {
                DrawBoard();
                Console.WriteLine("Döntetlen!");
                break;
            }

            current = (current == 'X' || current == human) ? computer : human;
        }
    }

    static int ReadMove()
    {
        while (true)
        {
            Console.Write("Válassz mezőt (1-9): ");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out int n)) { Console.WriteLine("Érvénytelen szám."); continue; }
            if (n < 1 || n > 9) { Console.WriteLine("A számnak 1 és 9 között kell lennie."); continue; }
            return n - 1;
        }
    }

    static int GetBestMove(char[] b, char player)
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < 9; i++)
        {
            if (b[i] != 'X' && b[i] != 'O')
            {
                char[] copy = (char[])b.Clone();
                copy[i] = player;
                int score = Minimax(copy, false, player);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        if (bestMove == -1)
            bestMove = Enumerable.Range(0, 9).First(i => b[i] != 'X' && b[i] != 'O');

        return bestMove;
    }

    static int Minimax(char[] b, bool isMaximizing, char maximizingPlayer)
    {
        char minimizingPlayer = (maximizingPlayer == 'X') ? 'O' : 'X';

        if (IsWinner(b, maximizingPlayer)) return +10;
        if (IsWinner(b, minimizingPlayer)) return -10;
        if (IsBoardFull(b)) return 0;

        if (isMaximizing)
        {
            int best = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (b[i] != 'X' && b[i] != 'O')
                {
                    char[] copy = (char[])b.Clone();
                    copy[i] = maximizingPlayer;
                    int val = Minimax(copy, false, maximizingPlayer);
                    best = Math.Max(best, val);
                }
            }
            return best - 1;
        }
        else
        {
            int best = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (b[i] != 'X' && b[i] != 'O')
                {
                    char[] copy = (char[])b.Clone();
                    copy[i] = minimizingPlayer;
                    int val = Minimax(copy, true, maximizingPlayer);
                    best = Math.Min(best, val);
                }
            }
            return best + 1;
        }
    }
}
