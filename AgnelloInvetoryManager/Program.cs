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
                Console.WriteLine("\n--- VINHERIA - SISTEMA DE ESTOQUE ---");
                Console.WriteLine("[1] Cadastrar vinho");
                Console.WriteLine("[2] Consultar estoque");
                Console.WriteLine("[3] Entrada de vinho no estoque");
                Console.WriteLine("[4] Saída de vinho do estoque");
                Console.WriteLine("[5] Alertas (Estoque Baixo/Validade)");
                Console.WriteLine("[0] Sair");
                Console.Write("Escolha uma opção: ");
                var opcao = Console.ReadLine();

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
                        return;
                    default:
                        Console.WriteLine("Opção inválida");
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
                return data;
            }
        }

        static string LerTexto(string mensagem, bool obrigatorio = true)
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
                return entrada;
            }
        }

        static void CadastrarVinho()
        {
            string nome = LerTexto("Nome: ");
            string tipo = LerTexto("Tipo (Tinto/Branco/Rosé/Espumante): ");
            int safra = LerInteiro("Safra (ano): ", 1000, DateTime.Now.Year + 1);
            string fornecedor = LerTexto("Fornecedor: ");
            string regiao = LerTexto("Região de Origem: ");
            int quantidade = LerInteiro("Quantidade inicial: ", 0);
            DateTime validade = LerData("Validade (YYYY-MM-DD): ");

            if (estoque.Any(x => x.Nome == nome && x.Safra == safra && x.Fornecedor == fornecedor))
            {
                Console.WriteLine("Já existe este vinho cadastrado com esta safra e fornecedor!");
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

            Console.WriteLine("Vinho cadastrado com sucesso!");
        }

        static void ConsultarEstoque()
        {
            if (estoque.Count == 0)
            {
                Console.WriteLine("Estoque vazio.");
                return;
            }

            Console.WriteLine("\n--- ESTOQUE DE VINHOS ---");
            foreach (var vinho in estoque)
            {
                Console.WriteLine($"ID:{vinho.Id} | {vinho.Nome} ({vinho.Tipo}) | Safra: {vinho.Safra} | Fornecedor: {vinho.Fornecedor} | Região: {vinho.Regiao} | Qtde: {vinho.Quantidade} | Validade: {vinho.Validade.ToShortDateString()}");
            }
        }

        static void EntradaEstoque()
        {
            ConsultarEstoque();
            int id = LerInteiro("Digite o ID do vinho para entrada: ");
            var vinho = estoque.FirstOrDefault(x => x.Id == id);

            if (vinho == null)
            {
                Console.WriteLine("Vinho não encontrado!");
                return;
            }
            int qtd = LerInteiro("Quantidade de entrada: ", 1);
            vinho.Quantidade += qtd;
            Console.WriteLine("Entrada registrada!");
        }

        static void SaidaEstoque()
        {
            ConsultarEstoque();
            int id = LerInteiro("Digite o ID do vinho para saída: ");
            var vinho = estoque.FirstOrDefault(x => x.Id == id);

            if (vinho == null)
            {
                Console.WriteLine("Vinho não encontrado!");
                return;
            }
            int qtd = LerInteiro("Quantidade de saída: ", 1);
            if (vinho.Quantidade < qtd)
            {
                Console.WriteLine("Erro: Quantidade insuficiente no estoque!");
                return;
            }
            vinho.Quantidade -= qtd;
            Console.WriteLine("Saída registrada!");
        }

        static void Alertas()
        {
            int minimoEstoque = 5;
            var estoqueBaixo = estoque.Where(x => x.Quantidade <= minimoEstoque).ToList();
            var proximosVencer = estoque.Where(x => (x.Validade - DateTime.Now).TotalDays <= 30).ToList();

            Console.WriteLine("--- ALERTAS ---");
            if (estoqueBaixo.Count == 0 && proximosVencer.Count == 0)
                Console.WriteLine("Nenhum alerta no momento.");

            if (estoqueBaixo.Count > 0)
            {
                Console.WriteLine("Vinhos com estoque baixo:");
                foreach (var v in estoqueBaixo)
                    Console.WriteLine($"{v.Nome} (ID:{v.Id}) - Qtde: {v.Quantidade}");
            }

            if (proximosVencer.Count > 0)
            {
                Console.WriteLine("Vinhos próximos da validade:");
                foreach (var v in proximosVencer)
                    Console.WriteLine($"{v.Nome} (ID:{v.Id}) - Validade: {v.Validade.ToShortDateString()}");
            }
        }
    }
}