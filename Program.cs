using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

class Program
{
    static void Main()
    {
        while (true)
        {
            // Inicialização do jogo
            Console.WriteLine("***********************************************");
            Console.WriteLine("*                                             *");
            Console.WriteLine("*               Bem-vindo ao Jogo             *");
            Console.WriteLine("*                       :)                    *");
            Console.WriteLine("*                 de Futebol!                 *");
            Console.WriteLine("*                                             *");
            Console.WriteLine("***********************************************");
            Player player1 = InitializePlayer("Jogador 1");
            Player player2 = InitializePlayer("Jogador 2");

            // Sorteio para determinar quem começa
            Player currentPlayer = new Random().Next(2) == 0 ? player1 : player2;

            // Loop do jogo
            while (true)
            {
                Console.WriteLine($"\nVez de {currentPlayer.Name}");
                Console.WriteLine("Pressione qualquer tecla para jogar as cartas bonitão");
                Console.ReadKey();
                List<string> cards = GetRandomCards();
                PlayRound(currentPlayer, cards);

                // Verificar condições de vitória ou empate
                if (CheckGameEnd(player1, player2))
                {
                    DisplayGameResult(player1, player2);
                    break;
                }

                // Alternar para o próximo jogador
                currentPlayer = (currentPlayer == player1) ? player2 : player1;
            }

            // Perguntar se os jogadores querem jogar novamente
            Console.Write("\nDigite '0' para jogar novamente ou '-1' para sair: ");
            int choice = int.Parse(Console.ReadLine());
            if (choice == -1)
            {
                break; // Sai do loop principal e encerra o programa
            }
        }
    }

    static Player InitializePlayer(string playerName)
    {
        Console.Write($"Digite o nome do {playerName}: ");
        string name = Console.ReadLine();
        return new Player(name, 10); // Inicializa o jogador com 10 energias
    }

    static List<string> GetRandomCards()
    {
        List<string> cards = new List<string> { "Gol", "Pênalti", "Falta", "Cartão Amarelo", "Cartão Vermelho", "Energia" };
        return Enumerable.Range(0, 3).Select(_ => cards[new Random().Next(cards.Count)]).ToList();
    }

    static void PlayRound(Player player, List<string> cards)
    {
        Console.WriteLine($"Cartas de {player.Name}: {string.Join(", ", cards)}");

        // Se o jogador ficou sem energia, passa a vez para o adversário
        if (player.Energy <= 0)
        {
            Console.WriteLine($"{player.Name} ficou sem energia e passa a vez para o adversário.");
            SwitchTurn(player);
            return; // Sai da função para evitar processamento adicional quando a energia é zero
        }

        int countGol = cards.Count(c => c == "Gol");
        int countPenalti = cards.Count(c => c == "Pênalti");
        int countFalta = cards.Count(c => c == "Falta");
        int countAmarelo = cards.Count(c => c == "Cartão Amarelo");
        int countVermelho = cards.Count(c => c == "Cartão Vermelho");
        int countEnergia = cards.Count(c => c == "Energia");

        if (countGol == 3)
        {
            Console.WriteLine("TRÊS GOLS! O jogador marca 1 ponto.");
            player.Score += 1;
            SwitchTurn(player);
        }
        else if (countEnergia == 3)
        {
            Console.WriteLine("ENERGIA! O jogador ganha uma energia.");
            player.Energy += 1;
            SwitchTurn(player);
        }
        else if (countPenalti == 3)
        {
            Console.WriteLine("PÊNALTI! Escolha a direção (D/E/C): ");
            
            string direction = Console.ReadLine().ToUpper();

            Console.WriteLine("Adversário, escolha a direção de defesa (D/E/C): ");
            string defenderDirection = Console.ReadLine().ToUpper();

            if (direction != defenderDirection)
            {
                Console.WriteLine("GOL! O jogador marca 1 gol.");
                player.Goals += 1;
                SwitchTurn(player);
            }
            else
            {
                Console.WriteLine("DEFENDEU! Nenhum ponto marcado.");
                SwitchTurn(player);
            }
        }
        else if (countFalta == 3)
        {
            Console.WriteLine("FALTA! O jogador passa a vez para o adversário.");
            SwitchTurn(player);
        }
        else if (countAmarelo == 3)
        {
            Console.WriteLine("TRÊS CARTÕES AMARELOS! Tome cuidado meu amigo.");
            player.Energy -= 1;
            Console.WriteLine("No próximo cartão amarelo, perderá duas energias e será avisado logo abaixo.");
            player.YellowCard += 1;

            if (player.YellowCard == 2)
            {
                Console.WriteLine("Você atingiu 2 cartões amarelos. O contador foi resetado e você perderá duas energias.");
                player.YellowCard = 0;
                player.Energy -= 1;
            }

            SwitchTurn(player);
        }
        else if (countVermelho == 3)
        {
            Console.WriteLine("TRÊS CARTÕES VERMELHOS! O jogador perde duas energias e passa a vez para o adversário.");
            player.Energy -= 2;
            SwitchTurn(player);
        }
        else
        {
            if (player.Energy > 0)
            {
                int roundScore = countGol * 3 + countPenalti * 2 + countFalta * 1 + countAmarelo * 1 + countVermelho * 0 + countEnergia * 2;
                Console.WriteLine($"Pontuação da rodada: {roundScore} pontos.");
                player.Score += roundScore;
            }

            SwitchTurn(player);
        }
    }

    static void SwitchTurn(Player player)
    {
        Console.WriteLine($"{player.Name}, sua pontuação: {player.Score}, suas energias: {player.Energy}, gols marcados: {player.Goals}");

        if (player.Energy <= 0)
        {
            Console.WriteLine($"{player.Name} ficou sem energia e passa a vez para o adversário.");
            return;
        }
    }

    static bool CheckGameEnd(Player player1, Player player2)
    {
        if (player1.Energy > 0 || player2.Energy > 0)
        {
            return false;
        }

        if (player1.Goals == player2.Goals && player1.Score == player2.Score)
        {
            Console.WriteLine("A partida empatou, pois ambos os jogadores têm a mesma pontuação e o mesmo número de gols!");
            return true;
        }

        if (player1.Goals == 0 && player2.Goals == 0 && player1.Score == player2.Score)
        {
            Console.WriteLine("A partida empatou, pois ambos os jogadores têm a mesma pontuação e nenhum deles marcou gols!");
            return true;
        }

        if (player1.Goals > player2.Goals)
        {
            Console.WriteLine($"{player1.Name} venceu com mais gols!");
            return true;
        }
        else if (player2.Goals > player1.Goals)
        {
            Console.WriteLine($"{player2.Name} venceu com mais gols!");
            return true;
        }
        else if (player1.Goals == 1 && player2.Goals == 0)
        {
            Console.WriteLine($"{player1.Name} venceu com um único gol, mesmo com menos pontos!");
            return true;
        }
        else if (player2.Goals == 1 && player1.Goals == 0)
        {
            Console.WriteLine($"{player2.Name} venceu com um único gol, mesmo com menos pontos!");
            return true;
        }
        else if (player1.Energy > 0 && player2.Energy > 0)
        {
            return false;
        }
        else if (player1.Goals == player2.Goals && player1.Score > player2.Score)
        {
            Console.WriteLine($"{player1.Name} venceu com igualdade de gols, mas maior pontuação!");
            return true;
        }
        else if (player2.Goals == player1.Goals && player2.Score > player1.Score)
        {
            Console.WriteLine($"{player2.Name} venceu com igualdade de gols, mas maior pontuação!");
            return true;
        }

        return false;
    }

    static void DisplayGameResult(Player player1, Player player2)
{
    if (player1.Score == player2.Score && player1.Goals == player2.Goals)
    {
        Console.WriteLine("A partida empatou, pois ambos os jogadores têm a mesma pontuação e o mesmo número de gols!");
    }
    else if ((player1.Goals == 0 && player2.Goals == 0) || (player1.Goals == player2.Goals && player1.Score == player2.Score))
    {
        Console.WriteLine("A partida empatou, pois ambos os jogadores têm a mesma pontuação e nenhum deles marcou gols!");
    }
    else if (player1.Goals > player2.Goals)
    {
        Console.WriteLine($"PARABÉNS {player1.Name}! Você venceu com {player1.Goals} gols e {player1.Score} pontos.");
        Console.WriteLine($"O seu adversário fez {player2.Goals} gols e {player2.Score} pontos.");
    }
    else if (player2.Goals > player1.Goals)
    {
        Console.WriteLine($"PARABÉNS {player2.Name}! Você venceu com {player2.Goals} gols e {player2.Score} pontos.");
        Console.WriteLine($"O seu adversário fez {player1.Goals} gols e {player1.Score} pontos.");
    }
    else if (player1.Goals == 1 && player2.Goals == 0)
    {
        Console.WriteLine($"PARABÉNS {player1.Name}! Você venceu com um único gol, mesmo com menos pontos!");
        Console.WriteLine($"O seu adversário fez {player2.Goals} gols e {player2.Score} pontos.");
    }
    else if (player2.Goals == 1 && player1.Goals == 0)
    {
        Console.WriteLine($"PARABÉNS {player2.Name}! Você venceu com um único gol, mesmo com menos pontos!");
        Console.WriteLine($"O seu adversário fez {player1.Goals} gols e {player1.Score} pontos.");
    }
    else if (player1.Goals == player2.Goals && player1.Score > player2.Score)
    {
        Console.WriteLine($"PARABÉNS {player1.Name}! Você venceu com igualdade de gols, mas maior pontuação!");
        Console.WriteLine($"O seu adversário fez {player2.Goals} gols e {player2.Score} pontos.");
    }
    else if (player2.Goals == player1.Goals && player2.Score > player1.Score)
    {
        Console.WriteLine($"PARABÉNS {player2.Name}! Você venceu com igualdade de gols, mas maior pontuação!");
        Console.WriteLine($"O seu adversário fez {player1.Goals} gols e {player1.Score} pontos.");
    }
}

}
class Player
{
    public string Name { get; }
    public int Energy { get; set; }
    public int Score { get; set; }
    public int Goals { get; set; }
    public int YellowCard { get; set; }

    public Player(string name, int energy)
    {
        Name = name;
        Energy = energy;
        Score = 0;
        Goals = 0;
        YellowCard = 0;
    }
}
