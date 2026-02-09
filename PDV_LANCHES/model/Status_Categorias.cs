using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using PDV_LANCHES.Views;
using ServidorLanches.model;
using System.Security.RightsManagement;

public class Status_Categorias
{
    private static Status_Categorias _instancia;
    private static readonly object _lock = new();

    private readonly HomeAdministrativoController _homeAdministrativoController;

    private readonly HomeController _homeController = new HomeController(); 




    public List<TerminalCaixa> caixasTerminaisAbertos { get; private set; }
    public TerminalCaixa caixaterminalSelecionado { get; set; }
    public Caixa caixa { get; set; }




    public List<CategoriaProduto> CategoriaProdutos { get; private set; }
    public List<TipoStatusPedido> TipoStatusPedido { get; private set; }
    public List<FormaDePagamento> FormaDePagamentos { get; private set; }
    public string teste { get; private set; }

    //GetAllCuponsDesconto
    private bool _carregado = false;

    private Status_Categorias()
    {
         _homeAdministrativoController = new HomeAdministrativoController();
        _homeController = new HomeController();
        CategoriaProdutos = new();
        TipoStatusPedido = new();
        FormaDePagamentos = new();

        //caixas 
        caixasTerminaisAbertos = new();
        caixaterminalSelecionado = null; 
        caixa = null; 

    }

    public static Status_Categorias Instancia
    {
        get
        {
            lock (_lock)
            {
                _instancia ??= new Status_Categorias();
                return _instancia;
            }
        }
    }

    public async Task CarregarAsync()
    {
        if (_carregado) return;

        var todosTipos  = await _homeAdministrativoController.getAllStatus();
        if(todosTipos != null)
        {
            TipoStatusPedido = todosTipos.Where(p => p.ativo).ToList();
        }


        var todasCategorias = await _homeAdministrativoController.getAllCategoria();
        if(todasCategorias != null)
        {
            CategoriaProdutos = todasCategorias.Where(p => p.ativo).ToList();
        }


        var pagamentos = await _homeAdministrativoController.getAllFormasDePagamentos();
        if (pagamentos != null)
        {
            FormaDePagamentos = pagamentos.Where(p => p.Ativo).ToList(); // Apenas as ativas
        }


       


        var caixasTerminaisAbertosTodos = await _homeController.GetAllTerminaisCaixa(); 
        if (caixasTerminaisAbertosTodos != null)
        {
            caixasTerminaisAbertos = caixasTerminaisAbertosTodos.Where(tc => tc.status == "ATIVO").ToList(); 
        }

        _carregado = true;
    }

    public async Task<Caixa> iniciarCaixaNovo(TerminalCaixa terminalCaixa, Usuario usuarioLogado, Decimal valorInicial = 0)
    {
        caixa = new Caixa();
        caixa.idTerminal = terminalCaixa.id;
        caixa.idUsuario = usuarioLogado.Id;
        caixa.dataAbertura = DateTime.Now;
        caixa.valorInicial = valorInicial;
        caixa.status = "ABERTO";

        if (caixa == null) return null;

        try
        {
            var caixaProcessado = await _homeController.AbrirCaixa(caixa);

            if (caixaProcessado != null)
            {
                this.caixa = caixaProcessado;
                return caixa;
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Caixa>    ObterDadosFechamento()
    {
        if (caixa == null) return null;

        try
        {
            // Pede ao servidor para calcular os totais com base nos pedidos
            var caixaProcessado = await _homeController.FecharCaixa(caixa);

            if (caixaProcessado != null)
            {
                this.caixa = caixaProcessado;
                return this.caixa;
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> salvarCaixa()
    {
        if (caixa == null) return false;

        try
        {
            var caixaProcessado = await _homeController.SalvarCaixa(caixa);

            if (caixaProcessado)
            {
                LimparCaixa();
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void LimparCaixa()
    {
        caixa = null;
        caixaterminalSelecionado = null;
    }






    public async Task RecarregarTudoAsync()
    {
        _carregado = false; 
        await CarregarAsync(); 
    }
}
