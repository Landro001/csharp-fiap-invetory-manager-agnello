using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VinheriaConsole
{
    class Vinho
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public int Safra { get; set; }
        public string Fornecedor { get; set; }
        public string Regiao { get; set; }
        public int Quantidade { get; set; }
        public DateTime Validade { get; set; }
    }

    class Program
    {
        static List<Vinho> estoque = new List<Vinho>();
        static int ultimoId = 1;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n--- VINHERIA - SISTEMA DE ESTOQUE ---");
                Console.WriteLine("[1] Cadastrar vinho");
                Console.WriteLine("[2] Consultar estoque");
                Console.WriteLine("[3] Entrada de vinho no estoque");
                Console.WriteLine("[4] Saída de vinho do estoque");
                Console.WriteLine("[5] Alertas (Estoque Baixo/Validade)");
                Console.WriteLine("[0] Sair");
                Console.Write("Escolha uma opção: ");
                string opcao = Console.ReadLine()?.Trim();

                switch (opcao)
                {
                    case "1":
                        CadastrarVinho();
                        break;
                    case "2":
                        ConsultarEstoque();
                        break;
                    case "3":
                        EntradaEstoque();
                        break;
                    case "4":
                        SaidaEstoque();
                        break;
                    case "5":
                        Alertas();
                        break;
                    case "0":
                        Console.WriteLine("\nEncerrando o sistema...");
                        Thread.Sleep(1000);
                        return;
                    default:
                        Console.WriteLine("\nOpção inválida. Aperte qualquer tecla para continuar.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static int LerInteiro(string mensagem, int? minimo = null, int? maximo = null)
        {
            int valor;
            while (true)
            {
                Console.Write(mensagem);
                string entrada = Console.ReadLine();
                if (!int.TryParse(entrada, out valor))
                {
                    Console.WriteLine("Valor inválido. Digite um número inteiro.");
                    continue;
                }
                if (minimo.HasValue && valor < minimo.Value)
                {
                    Console.WriteLine($"O valor deve ser maior ou igual a {minimo.Value}.");
                    continue;
                }
                if (maximo.HasValue && valor > maximo.Value)
                {
                    Console.WriteLine($"O valor deve ser menor ou igual a {maximo.Value}.");
                    continue;
                }
                return valor;
            }
        }

        static DateTime LerData(string mensagem)
        {
            DateTime data;
            while (true)
            {
                Console.Write(mensagem);
                string entrada = Console.ReadLine();
                if (!DateTime.TryParse(entrada, out data))
                {
                    Console.WriteLine("Data inválida. Digite no formato YYYY-MM-DD.");
                    continue;
                }
                if (data < DateTime.Today)
                {
                    Console.WriteLine("Data não pode ser anterior à data de hoje.");
                    continue;
                }
                return data;
            }
        }

        static string LerTexto(string mensagem, bool obrigatorio = true, int maxLength = 100)
        {
            string entrada;
            while (true)
            {
                Console.Write(mensagem);
                entrada = Console.ReadLine();
                if (obrigatorio && string.IsNullOrWhiteSpace(entrada))
                {
                    Console.WriteLine("Campo obrigatório.");
                    continue;
                }
                if (entrada.Length > maxLength)
                {
                    Console.WriteLine($"O texto deve ter no máximo {maxLength} caracteres.");
                    continue;
                }
                return entrada.Trim();
            }
        }

        static void CadastrarVinho()
        {
            Console.Clear();
            Console.WriteLine("--- Cadastro de Vinho ---");
            string nome = LerTexto("Nome: ");
            string tipo = LerTexto("Tipo (Tinto/Branco/Rosé/Espumante): ");
            int safra = LerInteiro($"Safra (ano): ", 1000, DateTime.Now.Year + 1);
            string fornecedor = LerTexto("Fornecedor: ");
            string regiao = LerTexto("Região de Origem: ");
            int quantidade = LerInteiro("Quantidade inicial: ", 0);
            DateTime validade = LerData("Validade (YYYY-MM-DD): ");

            if (estoque.Any(x =>
                string.Equals(x.Nome, nome, StringComparison.OrdinalIgnoreCase) &&
                x.Safra == safra &&
                string.Equals(x.Fornecedor, fornecedor, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("\nJá existe este vinho cadastrado com esta safra e fornecedor!");
                EsperarRetorno();
                return;
            }

            estoque.Add(new Vinho
            {
                Id = ultimoId++,
                Nome = nome,
                Tipo = tipo,
                Safra = safra,
                Fornecedor = fornecedor,
                Regiao = regiao,
                Quantidade = quantidade,
                Validade = validade
            });

            Console.WriteLine("\nVinho cadastrado com sucesso!");
            EsperarRetorno();
        }

        static void ConsultarEstoque()
        {
            Console.Clear();
            Console.WriteLine("--- Estoque de Vinhos ---");

            if (estoque.Count == 0)
            {
                Console.WriteLine("Estoque vazio.");
            }
            else
            {
                Console.WriteLine("ID | Nome           | Tipo      | Safra | Fornecedor    | Região        | Qtde | Validade");
                Console.WriteLine(new string('-', 90));
                foreach (var vinho in estoque)
                {
                    Console.WriteLine($"{vinho.Id,2} | {vinho.Nome,-14} | {vinho.Tipo,-9} | {vinho.Safra,5} | {vinho.Fornecedor,-13} | {vinho.Regiao,-13} | {vinho.Quantidade,4} | {vinho.Validade.ToShortDateString()}");
                }
            }
            EsperarRetorno();
        }

        static void EntradaEstoque()
        {
            if (!TemEstoqueCadastrado()) return;
            Console.Clear();
            Console.WriteLine("--- Entrada de Vinho no Estoque ---\n");
            ExibirVinhosResumido();

            int id = LerInteiro("Digite o ID do vinho para entrada: ");
            var vinho = estoque.FirstOrDefault(x => x.Id == id);

            if (vinho == null)
            {
                Console.WriteLine("Vinho não encontrado!");
                EsperarRetorno();
                return;
            }
            int qtd = LerInteiro("Quantidade de entrada: ", 1);
            vinho.Quantidade += qtd;
            Console.WriteLine("Entrada registrada!");
            EsperarRetorno();
        }

        static void SaidaEstoque()
        {
            if (!TemEstoqueCadastrado()) return;
            Console.Clear();
            Console.WriteLine("--- Saída de Vinho do Estoque ---\n");
            ExibirVinhosResumido();

            int id = LerInteiro("Digite o ID do vinho para saída: ");
            var vinho = estoque.FirstOrDefault(x => x.Id == id);

            if (vinho == null)
            {
                Console.WriteLine("Vinho não encontrado!");
                EsperarRetorno();
                return;
            }
            if (vinho.Quantidade == 0)
            {
                Console.WriteLine("Não há estoque disponível para este vinho.");
                EsperarRetorno();
                return;
            }
            int qtd = LerInteiro("Quantidade de saída: ", 1);
            if (vinho.Quantidade < qtd)
            {
                Console.WriteLine("Erro: Quantidade insuficiente no estoque!");
                EsperarRetorno();
                return;
            }
            vinho.Quantidade -= qtd;
            Console.WriteLine("Saída registrada!");
            EsperarRetorno();
        }

        static void Alertas()
        {
            Console.Clear();
            Console.WriteLine("--- ALERTAS ---");
            int minimoEstoque = 5;
            var estoqueBaixo = estoque.Where(x => x.Quantidade <= minimoEstoque).ToList();
            var proximosVencer = estoque.Where(x => (x.Validade - DateTime.Today).TotalDays <= 30).ToList();

            if (estoqueBaixo.Count == 0 && proximosVencer.Count == 0)
            {
                Console.WriteLine("Nenhum alerta no momento.");
            }
            else
            {
                if (estoqueBaixo.Count > 0)
                {
                    Console.WriteLine("\nVinhos com estoque baixo:");
                    foreach (var v in estoqueBaixo)
                        Console.WriteLine($"- {v.Nome} (ID: {v.Id}) | Quantidade: {v.Quantidade}");
                }

                if (proximosVencer.Count > 0)
                {
                    Console.WriteLine("\nVinhos próximos da validade:");
                    foreach (var v in proximosVencer)
                        Console.WriteLine($"- {v.Nome} (ID: {v.Id}) | Validade: {v.Validade.ToShortDateString()}");
                }
            }
            EsperarRetorno();
        }

        static bool TemEstoqueCadastrado()
        {
            if (estoque.Count == 0)
            {
                Console.WriteLine("Não há vinhos cadastrados no estoque ainda.");
                EsperarRetorno();
                return false;
            }
            return true;
        }

        static void ExibirVinhosResumido()
        {
            Console.WriteLine("ID | Nome               | Qtde | Safra | Validade");
            Console.WriteLine(new string('-', 55));
            foreach (var v in estoque)
            {
                Console.WriteLine($"{v.Id,2} | {v.Nome,-18} | {v.Quantidade,4} | {v.Safra,5} | {v.Validade.ToShortDateString()}");
            }
            Console.WriteLine();
        }

        static void EsperarRetorno()
        {
            Console.Write("\nAperte qualquer tecla para continuar...");
            Console.ReadKey();
        }
    }
}